using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CS422
{
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
}
