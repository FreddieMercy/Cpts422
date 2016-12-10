
/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 12
 */

using System;
using System.Collections.Generic;
using System.IO;

//If	the	second	stream	doesn’t	support	seeking,	have	the	
//ConcatStream	provide	forward-only	reading	functionality	with	no	seeking.


/*

		if	the	second	stream	doesn’t	support	
		seeking,	then	only	complete	the	write	if	you	are	at	the	exact	correct	position.	Otherwise	throw	an	
		exception.	
		*/


namespace CS422
{
	public class ConcatStream : Stream
	{
		private readonly Stream _first, _second;
		private readonly bool _fixedLength;
		private long _length, _position;

		public ConcatStream(Stream first, Stream second)
		{
			if (null == first) 
			{
				throw new ArgumentNullException ("null == first");
			}

			if (null == second) 
			{
				throw new ArgumentNullException ("null == second");
			}

			if (!first.CanSeek)
			{
				throw new ArgumentException("!first.CanSeek");
			}

			_first = first;
			_second = second;
			_position = _first.Position = 0;

			if (second.CanSeek)
			{
				_length = _first.Length + _second.Length;
			}
		}

		public ConcatStream(Stream first, Stream second, long fixedLength) : this(first, second)
		{
			if (fixedLength <= 0)
			{
				throw new ArgumentOutOfRangeException ("fixedLength <= 0");
			}

			_length = fixedLength;
			_fixedLength = true;
		}

		//*****************************************************************************************

		//General functions
		public override bool CanRead { 
			get { 
				return _first.CanRead && _second.CanRead; 
			} 
		}

		public override bool CanSeek {
			get { 
				return _first.CanSeek && _second.CanSeek; 
			} 
		}

		public override bool CanWrite { 
			get { 
				return _first.CanWrite && _second.CanWrite; 
			} 
		}

		//**********************************************************************************************

		public override long Length
		{
			get
			{
				if (!CanSeek && !_fixedLength) 
				{
					//No Seeking == No Length & Position
					throw new NotSupportedException("!CanSeek && !_fixedLength Don't Even TRY!!!");
				}

				return _length;
			}
		}

		public override long Position
		{
			get
			{
				return _position;
			}
			set
			{
				if (0 > value)
				{
					_position = 0;

					if (CanSeek)
					{
						_first.Position = 0;
						_second.Position = 0;
					}

					return;
				}

				if (CanSeek)
				{

					if (Length < value)
					{
						_position = Length;
						_first.Position = _first.Length;
						_second.Position = _second.Length;

						return;
					}
				}

				_position = value;

				if (CanSeek)
				{
					if (_position <= _first.Length)
					{
						_first.Position = _position;
						_second.Position = 0;
					}
					else
					{
						_first.Position = _first.Length;
						_second.Position = _position - _first.Length;
					}
				}
			}
		}

		public override void Flush()
		{
			_first.Flush();
			_second.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (!CanSeek) 
			{
				throw new NotSupportedException("!CanSeek");
			}

			switch (origin)
			{
			case SeekOrigin.Begin:
				Position = offset;
				break;
			case SeekOrigin.Current:
				Position += offset;
				break;
			case SeekOrigin.End:
				Position = Length + offset;
				break;
			}

			return _position;
		}

		public override void SetLength(long value)
		{
			if (!CanSeek) 
			{
				throw new NotSupportedException("!CanSeek");
			}

			_length = 0 > value ? 0 : value;

			if (_position > _length)
			{
				Position = _length;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!CanRead)
			{
				throw new NotSupportedException("!CanRead");
			}

			if (null == buffer) 
			{
				throw new ArgumentNullException ("null == buffer");
			}

			if (offset < 0) 
			{
				throw new ArgumentOutOfRangeException ("offset < 0");
			}

			if (count < 0) 
			{
				throw new ArgumentOutOfRangeException ("count < 0");
			}

			if (count + offset > buffer.Length) 
			{
				throw new ArgumentException ("count + offset > buffer.Length");
			}

			int bytesRead = 0;

			// Read the first stream if it is enought, otherwise read the rest bytes from second
			int readFromFirst = _position + count < _first.Length ? count : (int) (_first.Length - _position);

			// Read the second stream 
			int readFromSecond = readFromFirst > 0 ? count - readFromFirst : count;

			if (_fixedLength && _position + count > _length)
			{
				throw new ArgumentException(string.Format("_fixedLength && _position + count > _length (@ length {0})", _length));
			}

			if (readFromFirst > 0)
			{
				bytesRead = _first.Read(buffer, offset, readFromFirst);
				_position += bytesRead;
			}

			if (bytesRead >= 0 && CanSeek)
			{
				if (_second.Position != _position - _first.Length)
				{
					_second.Seek(0, SeekOrigin.Begin);
				}
			}

			if (readFromSecond > 0)
			{
				int temp = _second.Read(buffer, offset + bytesRead, readFromSecond);
				bytesRead += temp;
				_position += temp;
			}

			return bytesRead;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("!CanWrite");
			}

			if (null == buffer) 
			{
				throw new ArgumentNullException ("null == buffer");
			}

			if (offset < 0) 
			{
				throw new ArgumentOutOfRangeException ("offset < 0");
			}

			if (count < 0) 
			{
				throw new ArgumentOutOfRangeException ("count < 0");
			}

			if (count + offset > buffer.Length) 
			{
				throw new ArgumentException ("count + offset > buffer.Length");
			}

			// write to first stream if first is enough, otherwise write the rest to the second
			int writeToFirst = _position + count < _first.Length ? count : (int) _first.Length - offset;

			// Write to second stream if first is not enough
			int writeToSecond = writeToFirst > 0 ? count - writeToFirst : count;

			if (_fixedLength && _position + count > _length)
			{
				throw new ArgumentException(string.Format("_fixedLength && _position + count > _length (@ length {0})", _length));
			}

			if (writeToFirst > 0)
			{
				_first.Write(buffer, offset, writeToFirst);
				_position += writeToFirst;
			}
			else
			{
				writeToFirst = 0;
			}

			if (_position >= _first.Length && CanSeek)
			{
				// Check the second position
				if (_second.Position != _position - _first.Length)
				{
					_second.Seek(_position - _first.Length, SeekOrigin.Begin);
				}
			}

			if (writeToSecond > 0)
			{
				_second.Write(buffer, offset + writeToFirst, writeToSecond);
				_position += writeToSecond;
			}

			if (_position > _length)
			{
				SetLength(_position);
			}
		}
	}
}
