using System.Collections.Generic;

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
}
