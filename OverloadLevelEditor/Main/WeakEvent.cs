/*
THE COMPUTER CODE CONTAINED HEREIN IS THE SOLE PROPERTY OF REVIVAL
PRODUCTIONS, LLC ("REVIVAL").  REVIVAL, IN DISTRIBUTING THE CODE TO
END-USERS, AND SUBJECT TO ALL OF THE TERMS AND CONDITIONS HEREIN, GRANTS A
ROYALTY-FREE, PERPETUAL LICENSE TO SUCH END-USERS FOR USE BY SUCH END-USERS
IN USING, DISPLAYING,  AND CREATING DERIVATIVE WORKS THEREOF, SO LONG AS
SUCH USE, DISPLAY OR CREATION IS FOR NON-COMMERCIAL, ROYALTY OR REVENUE
FREE PURPOSES.  IN NO EVENT SHALL THE END-USER USE THE COMPUTER CODE
CONTAINED HEREIN FOR REVENUE-BEARING PURPOSES.  THE END-USER UNDERSTANDS
AND AGREES TO THE TERMS HEREIN AND ACCEPTS THE SAME BY USE OF THIS FILE.  
COPYRIGHT 2015-2020 REVIVAL PRODUCTIONS, LLC.  ALL RIGHTS RESERVED.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// WeakEvents are like regular events in C# but without risking the
// memory leak that you get from events if you don't remember to call -=
namespace WeakEvent
{
	public class WeakEventSource<TEventArgs> where TEventArgs : EventArgs
	{
		private readonly List<WeakDelegate> _handlers;

		public WeakEventSource()
		{
			_handlers = new List<WeakDelegate>();
		}

		public void Raise( object sender, TEventArgs e )
		{
			lock( _handlers ) {
				_handlers.RemoveAll( h => !h.Invoke( sender, e ) );
			}
		}

		public void Subscribe( EventHandler<TEventArgs> handler )
		{
			var weakHandlers = handler
				 .GetInvocationList()
				 .Select( d => new WeakDelegate( d ) )
				 .ToList();

			lock( _handlers ) {
				_handlers.AddRange( weakHandlers );
			}
		}

		public void Unsubscribe( EventHandler<TEventArgs> handler )
		{
			lock( _handlers ) {
				int index = _handlers.FindIndex( h => h.IsMatch( handler ) );
				if( index >= 0 )
					_handlers.RemoveAt( index );
			}
		}

		class WeakDelegate
		{
			#region Open handler generation and cache

			private delegate void OpenEventHandler( object target, object sender, TEventArgs e );

			// ReSharper disable once StaticMemberInGenericType (by design)
			private static readonly ConcurrentDictionary<MethodInfo, OpenEventHandler> _openHandlerCache =
				 new ConcurrentDictionary<MethodInfo, OpenEventHandler>();

			private static OpenEventHandler CreateOpenHandler( MethodInfo method )
			{
				var target = Expression.Parameter( typeof( object ), "target" );
				var sender = Expression.Parameter( typeof( object ), "sender" );
				var e = Expression.Parameter( typeof( TEventArgs ), "e" );

				if( method.IsStatic ) {
					var expr = Expression.Lambda<OpenEventHandler>(
						 Expression.Call(
							  method,
							  sender, e ),
						 target, sender, e );
					return expr.Compile();
				} else {
					var expr = Expression.Lambda<OpenEventHandler>(
						 Expression.Call(
							  Expression.Convert( target, method.DeclaringType ),
							  method,
							  sender, e ),
						 target, sender, e );
					return expr.Compile();
				}
			}

			#endregion

			private readonly WeakReference _weakTarget;
			private readonly MethodInfo _method;
			private readonly OpenEventHandler _openHandler;

			public WeakDelegate( Delegate handler )
			{
				_weakTarget = handler.Target != null ? new WeakReference( handler.Target ) : null;
				_method = handler.Method;
				_openHandler = _openHandlerCache.GetOrAdd( _method, CreateOpenHandler );
			}

			public bool Invoke( object sender, TEventArgs e )
			{
				object target = null;
				if( _weakTarget != null ) {
					target = _weakTarget.Target;
					if( target == null )
						return false;
				}
				_openHandler( target, sender, e );
				return true;
			}

			public bool IsMatch( EventHandler<TEventArgs> handler )
			{
				return ReferenceEquals( handler.Target, _weakTarget?.Target )
					 && handler.Method.Equals( _method );
			}
		}
	}
}