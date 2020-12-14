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
using Ionic.Zip;
using UnityEngine;

public interface IUserFileSystem : IDisposable
{
	System.IO.Stream OpenFileStream(string path);
    System.IO.Stream OpenFileStreamToMemory(string path);
}

public class ZipUserFileSystem : IUserFileSystem
{
	ZipFile m_zip;
	string m_internalRootFolder;
    string m_zipPath;

	public ZipUserFileSystem(string zipPath, string internalRootFolder)
	{
        m_zipPath = zipPath;
        m_internalRootFolder = internalRootFolder;
		m_zip = ZipFile.Read(zipPath);
		if (m_zip == null) {
			// TODO: Report error
		}
	}

	public System.IO.Stream OpenFileStream(string path)
	{
		if (m_zip == null) {
			return null;
		}

		string internalPath = path;
		if (!string.IsNullOrEmpty(m_internalRootFolder)) {
			internalPath = System.IO.Path.Combine(m_internalRootFolder, path);
		}

		ZipEntry entry = m_zip[internalPath];
        return entry.OpenReader();
	}

    public System.IO.Stream OpenFileStreamToMemory(string path)
    {
        // NOTE: It looks like the Zip system gets confused if we open
        // another file while one is currently being streamed, so we
        // need to work around that by re-opening the zip and loading
        // right into memory.
        if (m_zip == null)
        {
            return null;
        }

        using (var zippy = ZipFile.Read(m_zipPath))
        {
            string internalPath = path;
            if (!string.IsNullOrEmpty(m_internalRootFolder))
            {
                internalPath = System.IO.Path.Combine(m_internalRootFolder, path);
            }

            ZipEntry entry = zippy[internalPath];

            var str = new System.IO.MemoryStream((int)entry.UncompressedSize);
            entry.Extract(str);
            str.Seek(0, System.IO.SeekOrigin.Begin);
            return str;
        }
    }

    public void Dispose()
	{
		if (m_zip == null) {
			return;
		}

		// TODO: Do Dispose correctly
		m_zip.Dispose();
		m_zip = null;
	}
}

public class RawUserFileSystem : IUserFileSystem
{
	string m_rootFolderPath;

	public RawUserFileSystem(string rootFolderPath)
	{
		m_rootFolderPath = rootFolderPath;
	}

	public System.IO.Stream OpenFileStream(string path)
	{
		string fullPath = System.IO.Path.Combine(m_rootFolderPath, path);
		if (!System.IO.File.Exists(fullPath)) {
			Debug.LogErrorFormat("RawUserFileSystem: Unable to find the file '{0}'", fullPath);
			return null;
		}

		return System.IO.File.OpenRead(fullPath);
	}

    public System.IO.Stream OpenFileStreamToMemory(string path)
    {
        string fullPath = System.IO.Path.Combine(m_rootFolderPath, path);
        if (!System.IO.File.Exists(fullPath))
        {
            Debug.LogErrorFormat("RawUserFileSystem: Unable to find the file '{0}'", fullPath);
            return null;
        }

        return System.IO.File.OpenRead(fullPath);
    }

    public void Dispose()
	{
		// TODO: Do Dispose correctly
	}
}
