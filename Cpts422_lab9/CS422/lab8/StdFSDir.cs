using System;
using System.Collections.Generic;
using System.IO;

namespace CS422
{
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
}
