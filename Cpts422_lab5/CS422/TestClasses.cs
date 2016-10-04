using System;
using System.IO;

namespace CS422
{
	public class TestStream : MemoryStream
	{
		public TestStream ()
		{
			
		}

		public override bool CanRead {
			get {
				return false;
			}
		}

		public override bool CanWrite {
			get {
				return false;
			}
		}

		public override bool CanSeek {
			get {
				return false;
			}
		}

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}

		public override long Length {
			get {
				throw new NotImplementedException ();
			}
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}
						
	}

	public class TestStream2 : MemoryStream
	{

		public TestStream2 ()
		{

		}

		public override bool CanRead {
			get {
				return false;
			}
		}

		public override bool CanWrite {
			get {
				return false;
			}
		}

		public override bool CanSeek {
			get {
				return false;
			}
		}

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}

		public override long Length {
			get {
				throw new NotImplementedException ();
			}
		}

		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}

		public override long Position {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
	}
}

