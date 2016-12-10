using System;

namespace CS422
{
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

