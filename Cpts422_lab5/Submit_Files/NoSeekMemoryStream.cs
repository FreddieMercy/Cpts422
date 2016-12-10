using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace CS422
{
	/// <summary>
	/// Represents a memory stream that does not support seeking, but otherwise has
	/// functionality identical to the MemoryStream class.
	/// </summary>
	public class NoSeekMemoryStream : MemoryStream
	{
		private MemoryStream _self;
		public NoSeekMemoryStream(byte[] buffer)
		{
			_self = new MemoryStream (buffer);
		}

		public NoSeekMemoryStream(byte[] buffer, int offset, int count)
		{
			_self = new MemoryStream (buffer, offset, count);
		}
			
		public override bool CanSeek {
			get {
				throw new NotSupportedException ();
			}
		}

		public override long Seek (long offset, SeekOrigin loc)
		{
			throw new NotSupportedException ();
		}

		public override long Position {
			get {
				return _self.Position;
			}
			set {
				throw new NotSupportedException ();
			}
		}

		public override long Length {
			get {
				throw new NotSupportedException ();
			}
		}

		//#################################################################################

		public override bool CanRead {
			get {
				return _self.CanRead;
			}
		}

		public override bool CanWrite {
			get {
				return _self.CanWrite;
			}
		}

		public override int Capacity {
			get {
				return _self.Capacity;
			}
			set {
				_self.Capacity = value;
			}
		}

		public override System.Threading.Tasks.Task CopyToAsync (Stream destination, int bufferSize, System.Threading.CancellationToken cancellationToken)
		{
			return _self.CopyToAsync (destination, bufferSize, cancellationToken);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}

		public override void Flush ()
		{
			_self.Flush ();
		}

		public override System.Threading.Tasks.Task FlushAsync (System.Threading.CancellationToken cancellationToken)
		{
			return _self.FlushAsync (cancellationToken);
		}


		public override byte[] GetBuffer ()
		{
			return _self.GetBuffer ();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			return _self.Read (buffer, offset, count);
		}

		public override System.Threading.Tasks.Task<int> ReadAsync (byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			return _self.ReadAsync (buffer, offset, count, cancellationToken);
		}

		public override int ReadByte ()
		{
			return _self.ReadByte ();
		}

		public override void SetLength (long value)
		{
			_self.SetLength (value);
		}

		public override byte[] ToArray ()
		{
			return _self.ToArray ();
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			_self.Write (buffer, offset, count);
		}

		public override System.Threading.Tasks.Task WriteAsync (byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			return _self.WriteAsync (buffer, offset, count, cancellationToken);
		}

		public override void WriteByte (byte value)
		{
			_self.WriteByte (value);
		}

		public override void WriteTo (Stream stream)
		{
			_self.WriteTo (stream);
		}

		public override IAsyncResult BeginRead (byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return _self.BeginRead (buffer, offset, count, callback, state);
		}

		public override IAsyncResult BeginWrite (byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return _self.BeginWrite (buffer, offset, count, callback, state);
		}

		public override bool CanTimeout {
			get {
				return _self.CanTimeout;
			}
		}

		public override void Close ()
		{
			_self.Close ();
		}


		[Obsolete ("CreateWaitHandle will be removed eventually.  Please use 'new ManualResetEvent(false)' instead.")]
		protected override System.Threading.WaitHandle CreateWaitHandle ()
		{
			return base.CreateWaitHandle ();
		}

		public override int EndRead (IAsyncResult asyncResult)
		{
			return _self.EndRead (asyncResult);
		}

		public override void EndWrite (IAsyncResult asyncResult)
		{
			_self.EndWrite (asyncResult);
		}


		[Obsolete ("Do not call or override this method.")]
		protected override void ObjectInvariant ()
		{
			base.ObjectInvariant ();
		}

		public override int ReadTimeout {
			get {
				return _self.ReadTimeout;
			}
			set {
				_self.ReadTimeout = value;
			}
		}


		public override int WriteTimeout {
			get {
				return _self.WriteTimeout;
			}
			set {
				_self.WriteTimeout = value;
			}
		}

		public override System.Runtime.Remoting.ObjRef CreateObjRef (Type requestedType)
		{
			return _self.CreateObjRef (requestedType);
		}


		public override object InitializeLifetimeService ()
		{
			return _self.InitializeLifetimeService ();
		}

		public override bool Equals (object obj)
		{
			return _self.Equals (obj);
		}

		public override int GetHashCode ()
		{
			return _self.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("[NoSeekMemoryStream: Length={0}, CanRead={1}, CanSeek={2}, CanWrite={3}, Capacity={4}, Position={5}, CanTimeout={6}, ReadTimeout={7}, WriteTimeout={8}]", Length, CanRead, CanSeek, CanWrite, Capacity, Position, CanTimeout, ReadTimeout, WriteTimeout);
		}






	}
}

