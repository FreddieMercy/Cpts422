using System;
using System.IO;

namespace CS422
{
	public class NumberedTextWriter : TextWriter
	{
		private int _index;
		public TextWriter _writer = null;
		private bool _isDead;

		protected override void Dispose(bool disposing) {

			if (_isDead) 
			{
				throw new ObjectDisposedException ("The wrapped TextWriter reference had been disposed");  
			}

			_isDead = true;
			base.Dispose(disposing);
		}

		public NumberedTextWriter(TextWriter wrapThis)
		{
			if(wrapThis == null)
			{
				throw new ArgumentNullException("wrapThis == null");
			}

			_index = 1;
			_writer = wrapThis;
			_isDead = false;

		}

		public NumberedTextWriter(TextWriter wrapThis, int startingLineNumber)
		{

			if (wrapThis == null)
			{
				throw new ArgumentNullException("wrapThis == null");
			}

			_index = startingLineNumber;
			_writer = wrapThis;
			_isDead = false;
		}

		public override System.Text.Encoding Encoding {
			get {
				try
				{
					return _writer.Encoding;
				}
				catch(ObjectDisposedException){

					throw new ObjectDisposedException ("The wrapped TextWriter reference had been disposed");

				}
			}
		}

		public override void WriteLine(string value)
		{
			
			if(_isDead)
			{
				throw new ObjectDisposedException ("the NumberedTextWriter object had been disposed");
			}
			else{
				try
				{
					_writer.WriteLine(_index.ToString() + ": " + value);
					_index++;
				}
				catch(ObjectDisposedException){

					throw new ObjectDisposedException ("The wrapped TextWriter reference had been disposed");

				}
			}
		}

	}

	public class IndexedNumsStream : MemoryStream
	{

		private long _position;
		private long _length;

		private long clamp(long index)
		{

			if (index < 0)
			{

				return 0;
			}
			else
			{

				return index;
			}
		}

		public IndexedNumsStream(long position, long length)
		{
			_position = clamp(position);
			_length = clamp(length);
		

			if (_position > _length)
			{
				_position = _length;
			}

		}

		public override void SetLength(long value)
		{
			_length = clamp(value);
		}

		public override long Position
		{
			get
			{
				return _position;
			}

			set
			{
				
				_position = clamp(value);
				if (_position > _length)
				{
					_position = _length;
				}
			
			}
		}

		public override long Length {
			get {
				return _length;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (CanWrite)
			{
				throw new NotSupportedException("IndexedNumsStream is read only.");
			}
			else
			{
				throw new NotSupportedException("The IndexedNumsStream object cannot be written!");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{

			// error handling

			if (buffer == null)
			{
				throw new ArgumentNullException("buffer == null");
			}

			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset < 0");
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count < 0");
			}

			if (offset + count > buffer.Length)
			{
				throw new ArgumentException("offset + count > buffer.Length");
			}

			int read = 0;

			Position += offset;

			if (Position >= Length)
			{
				return 0;
			}

			while (read < count)
			{
				if (Position >= Length)
				{
					return read;
				}

				buffer[read] = Convert.ToByte(Position % 256);
				Position++;
				read++;
			}

			return read;
		}

		public override bool CanWrite {
			get {
				return false;
			}
		}

		public override long Seek (long offset, SeekOrigin loc)
		{
			switch (loc) {

			case SeekOrigin.Begin:

				Position = offset;

				break;

			case SeekOrigin.Current:

				Position += offset;

				break;

			case SeekOrigin.End:

				if(offset < 0){

					Position = Length + offset;

				}else{
					Position = Length;
				}

				break;				

			}

			return Position;
		}

	}

}