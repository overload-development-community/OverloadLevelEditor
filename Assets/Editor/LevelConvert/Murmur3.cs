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
using UnityEngine;

public static class Murmur3Helper
{
	public static ulong RotateLeft(this ulong original, int bits)
	{
		return (original << bits) | (original >> (64 - bits));
	}

	public static ulong RotateRight(this ulong original, int bits)
	{
		return (original >> bits) | (original << (64 - bits));
	}

	public static ulong GetUInt64(this byte[] bb, int pos)
	{
		return BitConverter.ToUInt64(bb, pos);
	}
}

class Murmur3
{
	// 128 bit output, 64 bit platform version 
	public static ulong READ_SIZE = 16;
	static ulong C1 = 0x87c37b91114253d5L;
	static ulong C2 = 0x4cf5ad432745937fL;

	ulong length;
	uint seed = 0; // if want to start with a seed, create a constructor
	ulong h1;
	ulong h2;
	byte[] m_bufferInProgress = new byte[READ_SIZE];
	int m_bufferInProgressSize = 0;

	public Murmur3()
	{
		Begin();
	}

	private void MixBody(ulong k1, ulong k2)
	{
		h1 ^= MixKey1(k1);

		h1 = h1.RotateLeft(27);
		h1 += h2;
		h1 = h1 * 5 + 0x52dce729;

		h2 ^= MixKey2(k2);

		h2 = h2.RotateLeft(31);
		h2 += h1;
		h2 = h2 * 5 + 0x38495ab5;
	}

	private static ulong MixKey1(ulong k1)
	{
		k1 *= C1;
		k1 = k1.RotateLeft(31);
		k1 *= C2;
		return k1;
	}

	private static ulong MixKey2(ulong k2)
	{
		k2 *= C2;
		k2 = k2.RotateLeft(33);
		k2 *= C1;
		return k2;
	}

	private static ulong MixFinal(ulong k)
	{
		// avalanche bits

		k ^= k >> 33;
		k *= 0xff51afd7ed558ccdL;
		k ^= k >> 33;
		k *= 0xc4ceb9fe1a85ec53L;
		k ^= k >> 33;
		return k;
	}

	public void Begin()
	{
		this.h1 = seed;
		this.length = 0L;
	}

	void CommitFullReadBuffer(byte[] data, int offset)
	{
		ulong k1 = data.GetUInt64(offset);
		offset += 8;

		ulong k2 = data.GetUInt64(offset);
		offset += 8;

		this.length += READ_SIZE;

		MixBody(k1, k2);
	}

	public void AddBytes(byte[] data)
	{
		int bufferOffset = 0;

		if (m_bufferInProgressSize > 0) {
			// Fill in-progress buffer 
			int availBuffer = (int)READ_SIZE - m_bufferInProgressSize;
			int availData = data.Length - bufferOffset;
			if (availData < availBuffer) {
				// Can fit all into available buffer
				for (int i = 0; i < availData; ++i) {
					m_bufferInProgress[m_bufferInProgressSize + i] = data[i + bufferOffset];
				}
				m_bufferInProgressSize += availData;
				return;
			}

			// Fill in what we can
			for (int i = 0; i < availBuffer; ++i) {
				m_bufferInProgress[m_bufferInProgressSize + i] = data[i + bufferOffset];
			}
			bufferOffset += availBuffer;
			m_bufferInProgressSize = 0;
			CommitFullReadBuffer(m_bufferInProgress, 0);
		}

		// Commit full chunks of buffer
		while ((data.Length - bufferOffset) >= (int)READ_SIZE) {
			CommitFullReadBuffer(data, bufferOffset);
			bufferOffset += (int)READ_SIZE;
		}

		// Move remainder into in-progress buffer
		int remainder = data.Length - bufferOffset;
		for (int i = 0; i < remainder; ++i) {
			m_bufferInProgress[i] = data[bufferOffset + i];
		}
		m_bufferInProgressSize = remainder;
	}

	public void Finish()
	{
		if (m_bufferInProgressSize == 0) {
			return;
		}

		this.length += (ulong)m_bufferInProgressSize;
		ulong k1 = 0;
		ulong k2 = 0;

		// little endian (x86) processing
		switch (m_bufferInProgressSize) {
			case 15:
				k2 ^= (ulong)m_bufferInProgress[14] << 48; // fall through
				goto case 14;
			case 14:
				k2 ^= (ulong)m_bufferInProgress[13] << 40; // fall through
				goto case 13;
			case 13:
				k2 ^= (ulong)m_bufferInProgress[12] << 32; // fall through
				goto case 12;
			case 12:
				k2 ^= (ulong)m_bufferInProgress[11] << 24; // fall through
				goto case 11;
			case 11:
				k2 ^= (ulong)m_bufferInProgress[10] << 16; // fall through
				goto case 10;
			case 10:
				k2 ^= (ulong)m_bufferInProgress[9] << 8; // fall through
				goto case 9;
			case 9:
				k2 ^= (ulong)m_bufferInProgress[8]; // fall through
				goto case 8;
			case 8:
				k1 ^= m_bufferInProgress.GetUInt64(0);
				break;
			case 7:
				k1 ^= (ulong)m_bufferInProgress[6] << 48; // fall through
				goto case 6;
			case 6:
				k1 ^= (ulong)m_bufferInProgress[5] << 40; // fall through
				goto case 5;
			case 5:
				k1 ^= (ulong)m_bufferInProgress[4] << 32; // fall through
				goto case 4;
			case 4:
				k1 ^= (ulong)m_bufferInProgress[3] << 24; // fall through
				goto case 3;
			case 3:
				k1 ^= (ulong)m_bufferInProgress[2] << 16; // fall through
				goto case 2;
			case 2:
				k1 ^= (ulong)m_bufferInProgress[1] << 8; // fall through
				goto case 1;
			case 1:
				k1 ^= (ulong)m_bufferInProgress[0]; // fall through
				break;
			default:
				throw new Exception("Something went wrong with remaining bytes calculation.");
		}

		h1 ^= MixKey1(k1);
		h2 ^= MixKey2(k2);
		m_bufferInProgressSize = 0;
	}

	public byte[] Hash
	{
		get
		{
			h1 ^= length;
			h2 ^= length;

			h1 += h2;
			h2 += h1;

			h1 = Murmur3.MixFinal(h1);
			h2 = Murmur3.MixFinal(h2);

			h1 += h2;
			h2 += h1;

			var hash = new byte[Murmur3.READ_SIZE];

			Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
			Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);

			return hash;
		}
	}

	public ulong Hash64Bit
	{
		get
		{
			var asBytes = this.Hash;
			var a = BitConverter.ToUInt64(asBytes, 0);
			var b = BitConverter.ToUInt64(asBytes, 8);
			return a ^ b;
		}
	}

	public void AddHash(bool val)
	{
		AddBytes(BitConverter.GetBytes(val));
	}

	public void AddHash(float val, float granularity)
	{
		int val_whole = (int)val;
		float val_remainder;
		if( val < 0.0f ) {
			val_remainder = Mathf.Ceil( val ) - val;
		} else {
			val_remainder = val - Mathf.Floor( val );
		}

		// Don't lose the sign here or we'll have 0.1 and -0.1 map to the same thing
		int remainder_scaled = (int)( val_remainder * Mathf.Sign(val)/ granularity );

		AddBytes( BitConverter.GetBytes( val_whole ) );
		AddBytes( BitConverter.GetBytes( (float)remainder_scaled ) );
	}

	public void AddHash( Vector4 val, float granularity )
	{
		AddHash( val[ 0 ], granularity );
		AddHash( val[ 1 ], granularity );
		AddHash( val[ 2 ], granularity );
		AddHash( val[ 3 ], granularity );
	}

	public void AddHash( Vector3 val, float granularity )
	{
		AddHash( val[ 0 ], granularity );
		AddHash( val[ 1 ], granularity );
		AddHash( val[ 2 ], granularity );
	}

	public void AddHash( Vector2 val, float granularity )
	{
		AddHash( val[ 0 ], granularity );
		AddHash( val[ 1 ], granularity );
	}
}
