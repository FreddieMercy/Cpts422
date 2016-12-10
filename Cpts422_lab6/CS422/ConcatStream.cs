
/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 5
 */

using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Generic;

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
		private Stream _first;
		private Stream _second;
		private long _length;
		private bool _secCons;
		private long _position;

		public ConcatStream(Stream first, Stream second)
		{
			if (first == null) {
				throw new NullReferenceException ("_first == null");
			}

			if (second == null) {
				throw new NullReferenceException ("_second == null");
			}

			_first = first;
			_second = new secondHelper (second, _first.Length, -1);

			try
			{
				long tmp = _first.Length;
			}
			catch (Exception)
			{
				throw new ArgumentException ("_first does not have Length, cannot compile!!!");
			}

			_first.Position = 0;
			_secCons = false;
			_position = 0;
		}

		public ConcatStream(Stream first, Stream second, long fixedLength)
		{

			if (first == null) {
				throw new NullReferenceException ("_first == null");
			}

			if (second == null) {
				throw new NullReferenceException ("_second == null");
			}

			if(fixedLength <= 0)
			{
				throw new ArgumentException ("fixedLength <= 0");
			}

			_first = first;
			_second = new secondHelper (second, _first.Length, fixedLength);

			try
			{
				long tmp = _first.Length;
			}
			catch (Exception)
			{
				throw new ArgumentException ("_first does not have Length, cannot compile!!!");
			}

			_first.Position = 0;
			_position = 0;
			_length = fixedLength;
			_secCons = true;
		}

		public override long Seek (long offset, SeekOrigin origin)
		{

			if (this.CanSeek) {

				switch (origin) {

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

			else
			{
				throw new NotImplementedException ("The Concat Stream cannot seek");
			}

		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			if(this.CanRead)
			{

				int returnValue = 0;
				int tmp = 0;

				if(buffer == null)
				{
					throw new ArgumentNullException ("buffer == null");
				}
				if(offset < 0 | count < 0)
				{
					throw new ArgumentOutOfRangeException ("offset < 0 | count < 0");
				}

				if(offset+count > buffer.Length)
				{
					throw new ArgumentException ("offset+count > buffer.Length");
				}

				returnValue = _first.Read (buffer, offset, count);

				Console.WriteLine ("return " + returnValue);
				tmp = _second.Read (buffer, returnValue + offset, count - returnValue);

				returnValue += tmp;

				return returnValue;
			}
			else
			{
				throw new NotImplementedException ("Not allowed READ and WRITE, don't try it");
			}

		}
		public override void Write (byte[] buffer, int offset, int count)
		{
			if(this.CanWrite)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException ("buffer == null");
				}
				if(offset < 0 | count < 0)
				{
					throw new ArgumentOutOfRangeException ("offset < 0 | count < 0");
				}

				if(offset+count > buffer.Length)
				{
					throw new ArgumentException ("offset+count > buffer.Length");
				}

				for (int i = 0; i < count; i++) {
					if (_first.Position > _first.Length - 1) {
						try
						{
							_second.Write (buffer, offset+i, 1);
						}
						catch(Exception) {

							throw new OutOfMemoryException("Position out of Range");
						}
					} else {
						_first.Write (buffer, offset + i, 1);
					}
				}

				try
				{
					if (Position > Length - 1) {
						throw new OutOfMemoryException("Position out of Range");
					}
				}
				catch(Exception) {


				}
			}
			else
			{
				throw new NotImplementedException ("Not allowed READ and WRITE, don't try it");
			}
		}

		public override long Position {
			get {
				return _first.Position + _second.Position;
			}
			set {
				if(this.CanSeek)
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException ("value < 0");
					}
					_position = value;
					if (_position > _first.Length - 1) {
						_first.Position = _first.Length - 1;
						try
						{
							_second.Position = _position - _first.Position;
						}
						catch(Exception) {
							throw new ArgumentOutOfRangeException ("Position too big");
						}
					} else {
						_first.Position = _position;
						_second.Position = 0;
					}
				}
				else
				{
					throw new NotSupportedException ("this Can not Seek hence does not support Position");
				}
			}
		}

		public override long Length {
			get {
				long f = 0;
				long s = 0;

				if (_secCons) {
					if (_length > 0) {
						return _length;
					}

					throw new ArgumentException ("_length <= 0 but constructed by ConcatStream(Stream first, Stream second, long fixedLength)");
				}

				try {
					f = _first.Length;
				} catch (Exception) {

					throw new NotImplementedException ("first does not have Length");
				}

				try {
					s = _second.Length;
				} catch (Exception) {
					throw new NotImplementedException ("second does not have Length");
				}

				return f + s;
			}
		}

		public override bool CanRead {
			get {
				try
				{
					return _first.CanRead && _second.CanRead;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public override bool CanWrite {
			get {
				try
				{
					return _first.CanWrite && _second.CanWrite;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public override bool CanSeek {
			get {

				try
				{
					return _first.CanSeek && _second.CanSeek;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public override void Flush ()
		{
			_first.Flush ();
			_second.Flush ();
		}

		public override void SetLength (long value)
		{
			throw new NotImplementedException ("I don't think this class can set length");
		}

	}
}

