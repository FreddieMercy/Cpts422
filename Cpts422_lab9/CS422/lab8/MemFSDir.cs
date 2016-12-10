using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace CS422
{
	
	public class MemFSDir : Dir422
	{
		public override string Name { get; }
		public override Dir422 Parent { get; }

		//private readonly ConcurrentDictionary<File422, int> _opens;
		private readonly List<Dir422> _dirs;
		private readonly List<File422> _files;

		private readonly object _lock = new object();

		public MemFSDir(string name, Dir422 parent)
		{
			if (string.IsNullOrEmpty(name) | string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("string.IsNullOrEmpty(name) | string.IsNullOrWhiteSpace(name)");
			}

			Name = name;
			Parent = parent;

			_files = new List<File422>();
			_dirs = new List<Dir422>();
			//_opens = new ConcurrentDictionary<File422, int>();
		}

		public override IList<Dir422> GetDirs()
		{
			return _dirs;
		}

		public override IList<File422> GetFiles()
		{
			return _files;
		}
		//1. Searches for a file with the specified name within this directory, optionally recursively
		//	 searching subdirectories if requested. Returns true if the file is found within the scope of
		//	 the search, false if it is not found.

		//2. Must reject file names with path characters. So if the file name string contains the / or \
		//	 path characters, then return false immediately.

		public override bool ContainsFile(string fileName, bool recursive)
		{
			//Validate:

			if(string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName) || recursive == null)
			{
				return false;
			}


			//GetFile(fileName):
			//1. This is a non-recursive search, so it only looks for files immediately within this one.
			//2. Return null immediately if the directory name string contains path characters.
			//3. Returns null if there is no file with the specified name within this one.
			//4. Returns a non-null File422 object if the file with the specified name exists within this one.

			if (this.GetFile (fileName) != null) {
				return true;
			} 

			else if (!recursive) {
				return false;
			}

			foreach (Dir422 x in this.GetDirs()) {
				if (x.ContainsFile (fileName, recursive)) {
					return true;
				}
			}

			return false;
		}

		public override bool ContainsDir(string dirName, bool recursive)
		{
			//Validate:

			if(string.IsNullOrEmpty(dirName) || string.IsNullOrWhiteSpace(dirName) || recursive == null)
			{
				return false;
			}

			//GetFile(fileName):
			//1. This is a non-recursive search, so it only looks for directories immediately within this one.
			//2. Return null immediately if the directory name string contains path characters.
			//3. Returns null if there is no directory with the specified name within this one.
			//4. Returns a non-null File422 object if the directory with the specified name exists within this one.

			if (this.GetDir (dirName) != null) {
				return true;
			} 

			else if (!recursive) {
				return false;
			}

			//GetDirs():
			//Gets a list of all the directories contained within this one. This is NOT a recursive search.
			//It only returns the directories that are directly inside this one.

			foreach (Dir422 x in this.GetDirs()) {
				if (x.ContainsDir (dirName, recursive)) {
					return true;
				}
			}

			return false;
		}

		//Analogous to GetDir, only for a file. Must return null if the file name string contains path
		//characters (/ or \).

		public override File422 GetFile(string fileName)
		{
			if (String.IsNullOrEmpty(fileName) | String.IsNullOrWhiteSpace(fileName)) {
				return null;
			}

			if(fileName.Contains("/") | 
				fileName.Contains("\\"))
			{
				return null;
			}

			foreach(File422 x in this.GetFiles())
			{
				if(x.Name == fileName)
				{
					return x;
				}
			}

			return null;
		}

		//1. This is a non-recursive search, so it only looks for directories immediately within this one.
		//2. Return null immediately if the directory name string contains path characters.
		//3. Returns null if there is no directory with the specified name within this one.
		//4. Returns a non-null Dir422 object if the directory with the specified name exists within this one.

		public override Dir422 GetDir(string dirName)
		{
			if (String.IsNullOrEmpty(dirName) | String.IsNullOrWhiteSpace(dirName)) {
				return null;
			}

			if(dirName.Contains("/") | 
				dirName.Contains("\\"))
			{
				return null;
			}

			foreach(Dir422 x in this.GetDirs())
			{
				if(x.Name == dirName)
				{
					return x;
				}
			}

			return null;
		}
			
		public override File422 CreateFile(string fileName)
		{
			if (String.IsNullOrEmpty(fileName) | String.IsNullOrWhiteSpace(fileName)) {
				return null;
			}

			if(fileName.Contains("/") | 
				fileName.Contains("\\"))
			{
				return null;
			}

			MemFSFile tmp = new MemFSFile (fileName, this);
			_files.Add (tmp);

			return tmp;
		}

		//1. First validates the file name to ensure that it does not have any invalid characters. For
		//	 our file systems we consider the only invalid characters to be the path characters (/ and
		//	 \). If the file name has these characters, or is null or empty, then null is returned.
		//2. Unlike CreateFile, this will NOT delete contents if the directory already exists. Instead it
		//	 will just return the Dir422 object for the existing directory.
		//3. If the directory does not exist, it is created and returned.
		//4. Null is returned on failure.

		public override Dir422 CreateDir(string dirName)
		{
			if (String.IsNullOrEmpty(dirName) | String.IsNullOrWhiteSpace(dirName)) {
				return null;
			}

			if(dirName.Contains("/") | 
				dirName.Contains("\\"))
			{
				return null;
			}

			MemFSDir tmp = new MemFSDir (dirName, this);
			_dirs.Add (tmp);

			return tmp;
		}

	}

}