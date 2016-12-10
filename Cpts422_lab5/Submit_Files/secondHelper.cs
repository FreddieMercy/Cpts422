using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace CS422
{
	public class secondHelper : Stream
	{
		private Stream _second;
		private long position;
		private int typeLen;
		private long length;
		private long origins;

		public secondHelper (Stream second, long firstLen, long fixLen)
		{
			if (second == null) {
				throw new NullReferenceException ("second == null");
			}

			_second = second;
			position = 0;

			try
			{
				origins = _second.Position;
			}
			catch(Exception) {
				origins = 0;
			}

			try
			{
				long len = _second.Length;
				typeLen = 1;

				if(fixLen > 0)
				{
					if (firstLen > fixLen) {
						typeLen = 0;
						length = 0;
					} else {
						length = fixLen - firstLen;
						typeLen = 0;
					}
				}
			}
			catch(Exception) {
				if(fixLen < 0)
				{
					typeLen = -1;
				}
				else
				{
					if (firstLen > fixLen) {
						typeLen = 0;
						length = 0;
					} else {
						length = fixLen - firstLen;
						typeLen = 0;
					}
				}
			}
		}

		public override long Position {
			get {
				try
				{
					return _second.Position;
				}
				catch(Exception) {
					return position;
				}
			}
			set {
				if(value < 0)
				{
					throw new ArgumentOutOfRangeException ("value < 0");
				}

				if(typeLen >= 0 && value > Length)
				{

					throw new ArgumentOutOfRangeException ("Position exceed the Length");
				}

				position = value;
				if (this.CanSeek) {
					_second.Position = position;
				} else {
					throw new NotImplementedException ("_second does not support Position, because it cannot seek");
				}
			}
		}

		public override long Length {
			get {

				switch(typeLen)
				{
				case 1:
					return _second.Length;
				
				case 0:
					return length;
				
				default:
					throw new NotImplementedException ("_second does not support Length");
		
				}
			}
		}

		public override void Flush ()
		{
			_second.Flush ();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			int pos = 0;
			for (int i = 0; i < count; i++) {
				try
				{
					int tmp = _second.Read (buffer, offset+i, 1);
					pos += tmp;
					position += tmp;
				}
				catch(Exception) {

					throw new OutOfMemoryException("Position out of Range");
				}
			}

			try
			{
				if (origins + pos > Length -1 || position > Length - 1) {
					throw new OutOfMemoryException("Position out of Range");
				}
			}
			catch(Exception) {


			}

			return pos;
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			for (int i = 0; i < count; i++) {
				try
				{
					_second.Write (buffer, offset+i, 1);
					position++;
				}
				catch(Exception) {

					throw new OutOfMemoryException("Position out of Range");
				}
			}

			try
			{
				if (position > Length - 1) {
					throw new OutOfMemoryException("Position out of Range");
				}
			}
			catch(Exception) {


			}
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			if(this.CanSeek)
			{
				position = _second.Seek(offset, origin);
				return position;
			}
			throw new NotImplementedException("_second cannot seek");
		}

		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}

		public override bool CanRead {
			get {
				return _second.CanRead;
			}
		}

		public override bool CanWrite {
			get {
				return _second.CanWrite;
			}
		}

		public override bool CanSeek {
			get {
				return _second.CanSeek;
			}
		}


	}
}

