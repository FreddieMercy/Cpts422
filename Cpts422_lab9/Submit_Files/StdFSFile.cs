using System;
using System.IO;

namespace CS422
{
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
}
