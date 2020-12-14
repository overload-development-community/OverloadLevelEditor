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

#if !PUBLIC_RELEASE
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

#if UNITY_STANDALONE || UNITY_EDITOR
using UniRx;
#else
using System.Reactive;
using System.Reactive.Linq;
#endif

#pragma warning disable 219 // warning CS0219: The variable `xyz' is assigned but its value is never used


public class EditorToEditorSyncRx : IDisposable
{
#if UNITY_EDITOR
	static readonly ushort SendToPort = 11000;
	static readonly ushort ListenToPort = 11001;
#else
	static readonly ushort SendToPort = 11001;
	static readonly ushort ListenToPort = 11000;
#endif

	#region Instance
	static EditorToEditorSyncRx s_instance = null;
	public static EditorToEditorSyncRx Instance
	{
		get
		{
			if( s_instance == null ) {
				s_instance = new EditorToEditorSyncRx();
			}
			return s_instance;
		}
	}
	#endregion

	Socket m_sending_socket;
	IPEndPoint m_sending_endpoint;
	IObservable<byte[]> m_active_receive_observable = null;

	public EditorToEditorSyncRx()
	{
		m_sending_socket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
		m_sending_endpoint = new IPEndPoint( IPAddress.Parse( "127.0.0.1" ), SendToPort );

		// Create the observable for listening - it will only be active when there
		// are subscribers (it is a "Hot Observable").
		m_active_receive_observable = Observable.Create<byte[]>( observer => {

			var listener = new UdpClient( ListenToPort );
			Action cleanup = new Action( () => {

				m_active_receive_observable = null;

				if( listener == null )
					return;

				// Mark the listener null so we know we are cleaning up
				var curr_listener = listener;
				listener = null;

				// Close up shop - this will trigger the callback to trigger
				curr_listener.Close();
			} );

			IAsyncResult inflight_async_result = null;
			IPEndPoint active_endpoint = new IPEndPoint( IPAddress.Any, ListenToPort );

			Action<IAsyncResult> receiveCallback = null;
			receiveCallback = new Action<IAsyncResult>( ( ar ) => {
				if( listener == null ) {
					// This happens when the cleanup action is executing, we just want to immediately bail out
					return;
				}

				try {
					// Finish up the read
					byte[] packet_data = listener.EndReceive( ar, ref active_endpoint );

					// Let the user know of the data
					observer.OnNext( packet_data );

					// Start the next receive
					inflight_async_result = listener.BeginReceive( new AsyncCallback( receiveCallback ), null );
				} catch( Exception e ) {
					// Report the error out
					observer.OnError( e );
					cleanup();
				}
			} );

			// Start the listening process
			try {
				inflight_async_result = listener.BeginReceive( new AsyncCallback( receiveCallback ), null );
			} catch( Exception ex ) {
				observer.OnError( ex );
				cleanup();
			}

			// Return the action that will get called when the user wants
			// to stop observing this stream. Our job here is to tear everything
			// down.
#if UNITY_STANDALONE || UNITY_EDITOR
			return Disposable.Create( cleanup );
#else
			return cleanup;
#endif
		} )
		// Turn this observable into a 'hot' observable, so that it is only alive if there are active subscribers
		.Publish()
		.RefCount();
	}

	public void Dispose()
	{
		s_instance = null;

		m_sending_socket.Close();
		m_sending_socket = null;

		m_active_receive_observable = null;
   }
	
	/// <summary>
	/// Send a full buffer of bytes to the destination
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public bool SendRaw( byte[] data )
	{
		return SendRaw( data, 0, data.Length );
	}

	/// <summary>
	/// Send a subset of a buffer of bytes to the destination
	/// </summary>
	/// <param name="data"></param>
	/// <param name="offset"></param>
	/// <param name="size"></param>
	/// <returns></returns>
	public bool SendRaw( byte[] data, int offset, int size )
	{
		try {
			m_sending_socket.SendTo( data, offset, size, SocketFlags.None, m_sending_endpoint );
			return true;
		} catch( Exception ) {
			return false;
		}
	}

	/// <summary>
	/// Access an IObservable for incoming raw packet data.
	/// </summary>
	/// <returns></returns>
	public IObservable<byte[]> ObserveIncomingRawPackets()
	{
		return m_active_receive_observable;
	}
}

#endif //!PUBLIC_RELEASE
