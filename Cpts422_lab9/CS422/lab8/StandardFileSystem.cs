using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CS422
{
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
}
