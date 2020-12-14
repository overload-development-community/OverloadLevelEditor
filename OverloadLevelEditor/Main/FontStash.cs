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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace OverloadLevelEditor
{
	public class FontStash : IDisposable
	{
		#region FontStash.dll
		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern IntPtr sth_create( int cachew, int cacheh );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern void sth_delete( IntPtr stash );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern int sth_add_font( IntPtr stash, [MarshalAs( UnmanagedType.LPStr )] string path );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern int sth_add_font_from_memory( IntPtr stash, [In] byte[] buffer );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern int sth_add_bitmap_font( IntPtr stash, int ascent, int descent, int line_gap );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern int sth_add_glyph_for_codepoint( IntPtr stash, int idx, uint id, uint codepoint, short size, short basev, int x, int y, int w, int h, float xoffset, float yoffset, float xadvance );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern int sth_add_glyph_for_char( IntPtr stash, int idx, uint id, [MarshalAs( UnmanagedType.LPStr )] string s, short size, short basev, int x, int y, int w, int h, float xoffset, float yoffset, float xadvance );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern void sth_begin_draw( IntPtr stash );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern void sth_end_draw( IntPtr stash );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern void sth_draw_text( IntPtr stash, int idx, float size, float x, float y, [MarshalAs( UnmanagedType.LPStr )] string s, out float dx );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern void sth_dim_text( IntPtr stash, int idx, float size, [MarshalAs( UnmanagedType.LPStr )] string s, out float minx, out float miny, out float maxx, out float maxy );

		[DllImport( "FontStash.dll", CallingConvention = CallingConvention.Cdecl )]
		static extern void sth_vmetrics( IntPtr stash, int idx, float size, out float ascender, out float descender, out float lineh );
		#endregion

		bool m_disposed;
		IntPtr m_stash;

		public FontStash( int cache_width, int cache_height )
		{
			m_stash = sth_create( cache_width, cache_height );
			if( m_stash == IntPtr.Zero ) {
				throw new Exception( "Failed to create font stash" );
			}
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		~FontStash()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			if( m_disposed )
				return;

			if( disposing ) {
				// Free other managed objects that implement IDisposable only
			}

			// Release any unmanaged objects set the object references to null
			if( m_stash != IntPtr.Zero ) {
				sth_delete( m_stash );
				m_stash = IntPtr.Zero;
			}

			m_disposed = true;
		}

		public int AddFont( string path )
		{
			return sth_add_font( m_stash, path );
		}

		public int AddFontFromMemory( byte[] buffer )
		{
			return sth_add_font_from_memory( m_stash, buffer );
		}

		public int AddBitmapFont( int ascent, int descent, int line_gap )
		{
			return sth_add_bitmap_font( m_stash, ascent, descent, line_gap );
		}

		public int AddGlyphForCodePoint( int font_index, uint gl_texture_id, uint codepoint, short size, short base_value, int x, int y, int w, int h, float x_offset, float y_offset, float x_advance )
		{
			return sth_add_glyph_for_codepoint( m_stash, font_index, gl_texture_id, codepoint, size, base_value, x, y, w, h, x_offset, y_offset, x_advance );
		}

		public int AddGlyphForChar( int font_index, uint gl_texture_id, string s, short size, short base_value, int x, int y, int w, int h, float x_offset, float y_offset, float x_advance )
		{
			return sth_add_glyph_for_char( m_stash, font_index, gl_texture_id, s, size, base_value, x, y, w, h, x_offset, y_offset, x_advance );
		}

		public void BeginDraw()
		{
			sth_begin_draw( m_stash );
		}

		public void EndDraw()
		{
			sth_end_draw( m_stash );
		}

		public void DrawText( int font_index, float size, float x, float y, string s, out float dx )
		{
			sth_draw_text( m_stash, font_index, size, x, y, s, out dx );
		}

		public void DimText( int font_index, float size, string s, out float min_x, out float min_y, out float max_x, out float max_y )
		{
			sth_dim_text( m_stash, font_index, size, s, out min_x, out min_y, out max_x, out max_y );
		}

		public void VMetrics( int font_index, float size, out float ascender, out float descender, out float lineh )
		{
			sth_vmetrics( m_stash, font_index, size, out ascender, out descender, out lineh );
		}
	}
}
