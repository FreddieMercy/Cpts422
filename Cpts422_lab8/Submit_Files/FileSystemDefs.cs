using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace CS422
{

	public abstract class Dir422
	{
		//A get-only property for the name of the directory. This is NOT the full path, it is just the
		//directory name.
		public abstract string Name { get; }

		//A get-only property that returns the parent directory of this one. For all directories
		//except the root of the file system, this must be non-null. It must be null only for the root
		//of the file system.
		public abstract Dir422 Parent { get; }

		//Gets a list of all the directories contained within this one. This is NOT a recursive search.
		//It only returns the directories that are directly inside this one.
		public abstract IList<Dir422> GetDirs();


		//Analogous to GetDirs, but for files instead. Like GetDirs, it is not recursive
		public abstract IList<File422> GetFiles();

		//1. Searches for a file with the specified name within this directory, optionally recursively
		//	 searching subdirectories if requested. Returns true if the file is found within the scope of
		//	 the search, false if it is not found.

		//2. Must reject file names with path characters. So if the file name string contains the / or \
		//	 path characters, then return false immediately.

		public abstract bool ContainsFile(string fileName, bool recursive);


		//Analogous to ContainsFile, only for directories. Like ContainsFile, this must return false if
		//path characters are present in the name.
		public abstract bool ContainsDir(string dirName, bool recursive);

		//Analogous to GetDir, only for a file. Must return null if the file name string contains path
		//characters (/ or \).
		public abstract File422 GetFile(string fileName);


		//1. This is a non-recursive search, so it only looks for directories immediately within this one.
		//2. Return null immediately if the directory name string contains path characters.
		//3. Returns null if there is no directory with the specified name within this one.
		//4. Returns a non-null Dir422 object if the directory with the specified name exists within this one.
		public abstract Dir422 GetDir(string dirName);


		//1.First validates the file name to ensure that it does not have any invalid characters. For
		//	our file systems we consider the only invalid characters to be the path characters (/ and
		//	\). If the file name has these characters, or is null or empty, then null is returned.
		//2.Otherwise, if the name is ok, the file is created with a size of 0 in this directory. Should
		//	the file already exist, it is truncated back to a length of 0, erasing all prior content.
		//3.A File422 object is returned on success, null on failure.
		public abstract File422 CreateFile(string fileName);



		//1. First validates the file name to ensure that it does not have any invalid characters. For
		//	 our file systems we consider the only invalid characters to be the path characters (/ and
		//	 \). If the file name has these characters, or is null or empty, then null is returned.
		//2. Unlike CreateFile, this will NOT delete contents if the directory already exists. Instead it
		//	 will just return the Dir422 object for the existing directory.
		//3. If the directory does not exist, it is created and returned.
		//4. Null is returned on failure.
		public abstract Dir422 CreateDir(string dirName);
	}


	public abstract class File422
	{
		//A get-only property that gets the name of the file. This is NOT the full path, it is just the
		//file name.
		public abstract string Name { get; }

		//Returns the directory that this file resides in. Must always be non-null
		public abstract Dir422 Parent { get; }

		//Opens the file for reading and returns the read-only stream. Returns null on failure.
		public abstract Stream OpenReadOnly();

		//Opens the file for reading and writing and returns the read/write stream. Returns null
		//on failure.
		public abstract Stream OpenReadWrite();
	}

	public abstract class FileSys422
	{
		//Gets the root directory for this file system. The parent of the root of any file system must be null.
		//This property is abstract in this class.
		public abstract Dir422 GetRoot();

		//1. Returns true if the file is contained anywhere inside this file system, false otherwise.
		//2. Must NOT search through every single directory in the entire file system to determine
		//	 this. There is a much better way that you should understand after reading the Dir422
		//	 and File422 class specifications.
		//3. Implement this function within this class so that inheriting classes will not have to
		//	 provide their own implementation. Mark it as virtual so that they can provide an
		//	 override, but just make sure that you implement it here so that they don’t have to
		public virtual bool Contains(File422 file)
		{

			return Contains(file.Parent);
		}


		//1. returns true if the directory is contained anywhere inside this file system, false
		//	 otherwise.
		//2. Must NOT search through every single directory in the entire file system to determine
		//	 this. There is a much better way that you should understand after reading the Dir422
		//	 class specification.
		//3. Implement this function within this class so that inheriting classes will not have to
		//	 provide their own implementation. Mark it as virtual so that they can provide an
		//	 override, but just make sure that you implement it here so that they don’t have to.
		public virtual bool Contains(Dir422 dir)
		{
			if (dir == null) return false;

			while (dir.Parent != null)
			{
				dir = dir.Parent;
			}

			return ReferenceEquals(dir, GetRoot());
		}
	}


	//path of customed class: absolute path
	//path of the module: absolute path
	//path (file/dir name) of the customized class: relative, only the name 
	public class StdFSDir : Dir422
	{
		private readonly string _path;
		public override string Name { get; }
		public override Dir422 Parent { get; }

		public StdFSDir(string path, Dir422 parent)
		{
			string[] decom = path.Split ('/');
			if (!Directory.Exists (path)) {
				throw new ArgumentException ("!Directory.Exists (path)");
			}

			//root
			if (path == "/") {

				Parent = null;
				Name = "/";
				_path = path;

			} else {
				//path = /a
				if(!string.IsNullOrEmpty(decom[decom.Length-1]) && !string.IsNullOrWhiteSpace(decom[decom.Length-1]) )
				{
					Parent = parent;
					Name = decom [decom.Length - 1];
					_path = path;
				}
				//path = /a/
				else
				{
					Parent = parent;
					Name = decom [decom.Length - 2];
					_path = path.Substring (0, path.Length - 1);
				}
			}


		}

		public override IList<Dir422> GetDirs()
		{
			List<Dir422> dirs = new List<Dir422>();

			foreach (string file in Directory.GetDirectories(_path))
			{
				dirs.Add(new StdFSDir(file, this));
			}

			return dirs;
		}

		public override IList<File422> GetFiles()
		{
			List<File422> files = new List<File422>();

			foreach (string file in Directory.GetFiles(_path))
			{
				files.Add(new StdFSFile(file, this));
			}

			return files;
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
				throw new NullReferenceException ("string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName) || recursive == null");
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
				throw new NullReferenceException ("string.IsNullOrEmpty(dirName) || string.IsNullOrWhiteSpace(dirName) || recursive == null");
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
				throw new NullReferenceException ("String.IsNullOrEmpty(fileName) | String.IsNullOrWhiteSpace(fileName)");
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
				throw new NullReferenceException ("String.IsNullOrEmpty(dirName) | String.IsNullOrWhiteSpace(dirName)");
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


		//1.First validates the file name to ensure that it does not have any invalid characters. For
		//	our file systems we consider the only invalid characters to be the path characters (/ and
		//	\). If the file name has these characters, or is null or empty, then null is returned.
		//2.Otherwise, if the name is ok, the file is created with a size of 0 in this directory. Should
		//	the file already exist, it is truncated back to a length of 0, erasing all prior content.
		//3.A File422 object is returned on success, null on failure.

		public override File422 CreateFile(string fileName)
		{
			if (String.IsNullOrEmpty(fileName) | String.IsNullOrWhiteSpace(fileName)) {
				throw new NullReferenceException ("String.IsNullOrEmpty(fileName) | String.IsNullOrWhiteSpace(fileName)");
			}

			if(fileName.Contains("/") | 
				fileName.Contains("\\"))
			{
				return null;
			}

			var newPath = Path.Combine(_path, fileName);

			if (File.Exists (newPath)) {

				File.Delete (newPath);
			}

			if(this.ContainsFile(fileName, false)){
				//this.GetFile (fileName) = null;
				Console.WriteLine("********************************************************\nThe target File exists in Memory!!\n********************************************************");
			}

			try
			{
				using (File.Create(newPath)) {

				}
			}
			catch (Exception)
			{
				return null;
			}

			return new StdFSFile(newPath, this);
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
				throw new NullReferenceException ("String.IsNullOrEmpty(fileName) | String.IsNullOrWhiteSpace(fileName)");
			}

			if(dirName.Contains("/") | 
				dirName.Contains("\\"))
			{
				return null;
			}

			var newPath = Path.Combine(_path, dirName);

			if (Directory.Exists (newPath)) {

				try
				{
					return this.GetDir(dirName);
				}
				catch(Exception) {
					throw new Exception ("Somehow the dir exists in real world but not in memory");
				}
			}

			try
			{
				Directory.CreateDirectory(newPath);
			}
			catch (Exception)
			{
				return null;
			}

			return new StdFSDir(newPath, this);
		}


	}



	public class StdFSFile : File422
	{
		private readonly string _path;
		public override string Name { get; }
		public override Dir422 Parent { get; }

		public StdFSFile(string path, Dir422 parent)
		{
			string[] decom = path.Split ('/');
			if (!File.Exists (path)) {
				throw new ArgumentException ("!File.Exists (path)");
			}

			//root
			if (path == "/") {

				throw new Exception ("cannot set root as File!! using wrong class?");

			} else {
				//path = /a
				if(!string.IsNullOrEmpty(decom[decom.Length-1]) && !string.IsNullOrWhiteSpace(decom[decom.Length-1]) )
				{
					Parent = parent;
					Name = decom [decom.Length - 1];
					_path = path;
				}
				//path = /a/
				else
				{
					Parent = parent;
					Name = decom [decom.Length - 2];
					_path = path.Substring (0, path.Length - 1);
				}
			}

		}

		public override Stream OpenReadOnly()
		{
			try {
				return new FileStream (_path, FileMode.Open, FileAccess.Read);
			} catch (Exception) {
				// The file cannot be opened
				return null;
			}
		}

		public override Stream OpenReadWrite()
		{
			try {
				return new FileStream (_path, FileMode.Open, FileAccess.ReadWrite);
			} catch (Exception) {
				return null;
			}
		}
	}



	public class StandardFileSystem : FileSys422
	{
		private readonly Dir422 _root;

		public StandardFileSystem(string path = "/")
		{
			_root = new StdFSDir(path, null);
		}

		public override Dir422 GetRoot()
		{
			return _root;
		}

		public static StandardFileSystem Create (string rootDir = "/")
		{
			if (Directory.Exists (rootDir)) {
				return new StandardFileSystem (rootDir);
			} else {
				return null;
			}
		}
	}



	public class MemFSFile : File422
	{
		public override string Name { get; }
		public override Dir422 Parent { get; }

		private object _lock;
		private string _status;
		private int _readOnlyTimes;

		public MemFSFile(string name, Dir422 parent)
		{
			if (string.IsNullOrEmpty(name) | string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("string.IsNullOrEmpty(name) | string.IsNullOrWhiteSpace(name)");
			}

			Name = name;
			Parent = parent;
			_lock = new object ();
			_status = null;
			_readOnlyTimes = 0;
		}

		public override Stream OpenReadOnly ()
		{
			lock (_lock) {
				if (_status == null | _status == "R") {
					_status = "R";
					ThisStream s = new ThisStream ();
					s.Disposed += waitForRWtoDispose;
					_readOnlyTimes++;
					return new ThisStream ();
				}

				return null;
			}
		}

		public override Stream OpenReadWrite ()
		{
			lock (_lock) {
				if (_status == null) {
					_status = "RW";
					ThisStream s = new ThisStream ();
					s.Disposed += waitForRWtoDispose;
					return s;
				}
				return null;
			}
		}

		private void waitForRWtoDispose()
		{
			lock (_lock) {
				if (_readOnlyTimes == 0) {
					_status = null;
				} else if (_status == "R") {
					_readOnlyTimes--;
				}
			}
		}
	}



public delegate void DisposedEventHandler();

internal class ThisStream : MemoryStream
{

	public event DisposedEventHandler Disposed;

	public ThisStream()
	{

	}

	~ThisStream()
	{
		Dispose(false);
	}

	protected override void Dispose(bool disposing)
	{
		if (Disposed != null)
		{
			Disposed();
		}

		base.Dispose(disposing);

	}
}
		


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

	public class MemoryFileSystem : FileSys422
	{
		private readonly Dir422 _root;

		public MemoryFileSystem()
		{
			_root = new MemFSDir("/", null);
		}

		public override Dir422 GetRoot()
		{
			return _root;
		}
	}
		
}

