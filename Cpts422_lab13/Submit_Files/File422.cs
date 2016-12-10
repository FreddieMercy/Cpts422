using System.IO;

namespace CS422
{
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
}
