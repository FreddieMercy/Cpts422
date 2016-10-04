
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


// TODO: Position, when second stream does not support length
namespace CS422
{
	public class ConcatStream : Stream
	{
		private Stream _first;
		private Stream _second;
		private long _length;
		private bool _secCons;
		private long _position;

		//If	the	second	stream	
		//does	not	support	seeking,	assume	it’s	at	position	0	when	passed	into	the	constructor.

		// *Then, if the second stream supports seeking, I set the _Sposition to the second stream's position.

		/*

		if	the	second	stream	doesn’t	support	
		seeking,	then	only	complete	the	write	if	you	are	at	the	exact	correct	position.	Otherwise	throw	an	
		exception.	


			*Read & Write still have problemsss!!!!!!!!!!
		*/

		public ConcatStream(Stream first, Stream second)
		{
			_first = first;
			_second = second;
			try
			{
				_length = _first.Length;
			}
			catch (Exception)
			{
				throw new ArgumentException ("_first does not have Length, cannot compile!!!");
			}

			_first.Position = 0;

			try
			{
				if(!_second.CanSeek)
				{
					_second.Position = 0;
				}
			}
			catch(Exception) {

				Console.WriteLine ("It should never happen because I don't think _second.CanSeek is unreadable");
				_second.Position = 0;
			}
			_length = -1;
			_secCons = false;
			_position = 1;
		}

		public ConcatStream(Stream first, Stream second, long fixedLength)
		{
			_first = first;
			_second = second;
			try
			{
				_length = _first.Length;
			}
			catch (Exception)
			{
				throw new ArgumentException ("_first does not have Length, cannot compile!!!");
			}

			if (fixedLength > 0) {
				_length = fixedLength;
				_secCons = true;
			} else {
				throw new ArgumentOutOfRangeException ("fixedLength <= 0!!! ");

			}
			_first.Position = 0;

			try
			{
				if(!_second.CanSeek)
				{
					_second.Position = 0;
				}
			}
			catch(Exception) {

				Console.WriteLine ("It should never happen because I don't think _second.CanSeek is unreadable");
				_second.Position = 0;
			}
			_position = 1;
		}
		//########################################################################################

		public override long Seek (long offset, SeekOrigin origin)
		{
			//If	the	second	stream	doesn’t	support	seeking,	have	the	
			//ConcatStream	provide	forward-only	reading	functionality	with	no	seeking.

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
				byte[] bytes = new byte[1];

				/*
						ArgumentNullException	
							buffer is null.
						ArgumentOutOfRangeException	
							offset or count is negative.
						NotSupportedException	
							The stream does not support reading.
						ObjectDisposedException	
							Methods were called after the stream was closed.
						ArgumentException	
							The sum of offset and count is larger than the buffer length.
						IOException	
							An I/O error occurs.

					*/

				if(buffer == null)
				{
					throw new ArgumentNullException ("buffer == null");
				}
				if(offset < 0 | count < 0)
				{
					throw new ArgumentOutOfRangeException ("offset < 0 | count < 0");
				}

				for (int i = 0; i < count; i++) {
					
					if (Position > _first.Length - 1) {

						try
						{
							returnValue += _second.Read (bytes, (int)Position + offset + i, 1);
							buffer[i]=bytes[0];
						}
						catch(Exception e)
						{
							//throw e;

							if (this.hadLength) {
								throw new Exception ("cannot READ at Position = " + Position + " and the Length = " + this.Length);
							} else {
								throw new Exception ("cannot READ at Position = " + Position + " and the Length is unavailable");
							}
						}

					} else {

						try
						{
							returnValue += _first.Read (bytes, (int)Position + offset + i, 1);
							buffer[i]=bytes[0];
						}
						catch(Exception e)
						{
							//throw e;

							if (this.hadLength) {
								throw new Exception ("cannot READ at Position = " + Position + " and the Length = " + this.Length);
							} else {
								throw new Exception ("cannot READ at Position = " + Position + " and the Length is unavailable");
							}
						}
					}
				}

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
				byte[] bytes = new byte[1];
				if(buffer == null)
				{
					throw new ArgumentNullException ("buffer == null");
				}
				if(offset < 0 | count < 0)
				{
					throw new ArgumentOutOfRangeException ("offset < 0 | count < 0");
				}

				Position += offset;

				for (int i = 0; i < count; i++) {

					if (Position > _first.Length - 1) {

						try
						{
							_second.Write (bytes, (int)Position + offset + i, 1);
							buffer[i]=bytes[0];
						}
						catch(Exception e)
						{
							//throw e;

							if (this.hadLength) {
								throw new Exception ("cannot WRITE at Position = " + Position + " and the Length = " + this.Length);
							} else {
								throw new Exception ("cannot WRITE at Position = " + Position + " and the Length is unavailable");
							}

						}

					} else {

						try
						{
							_first.Write (bytes, (int)Position + offset + i, 1);
							buffer[i]=bytes[0];
						}
						catch(Exception e)
						{
							//throw e;

							if (this.hadLength) {
								throw new Exception ("cannot WRITE at Position = " + Position + " and the Length = " + this.Length);
							} else {
								throw new Exception ("cannot WRITE at Position = " + Position + " and the Length is unavailable");
							}
						}
					}
				}
			}
			else
			{
				throw new NotImplementedException ("Not allowed READ and WRITE, don't try it");
			}
		}
		//########################################################################################

		public override long Position {
			get {
				if (_position > _first.Length - 1) {

					_position = _first.Position + 1 + _second.Position;
					return _first.Position+_second.Position;

				}
				else
				{
					_position = _first.Position + 1;
					return _first.Position;
				}
			}
			set {
				//Fisrt?Second?
				_position = value;

				if (_position > _first.Length - 1) {
					_first.Position = _first.Length - 1;
					if(this.hadLength && this.Length >= _position + 1)
					{
						_second.Position = _position - _first.Position;
					}
					else if(!this.hadLength)
					{
						int i = 0;
						try
						{
							for(i = 0; i < _position - _first.Position + 1; i++)
							{
								_second.Position = i;
							}

						}
						catch(Exception) {

							throw new IndexOutOfRangeException ("Failed to set _second.Position at i =" + i + ", _second.Position = " + _second.Position + ", total Position = "+Position);
						}
					}
					else
					{
						throw new IndexOutOfRangeException ("The Length is not enought for the Position");
					}

				} else {

					if(_position < 0)
					{
						throw new IndexOutOfRangeException ("The Position cannot be negative");
					}

					_first.Position = _position;
					_second.Position = 0;
				}
		
				_position++;
			}
		}

		private bool hadLength
		{
			get{
				try
				{
					long tmp = this.Length;
					return true;
				}
				catch(Exception) {

					return false;

				}
			}
		}

		public override long Length {
			get {
				long f = 0;
				long s = 0;

				if(_secCons)
				{
					if (_length > 0) {
						return _length;
					}

					throw new ArgumentException ("_length <= 0 but constructed by ConcatStream(Stream first, Stream second, long fixedLength)");
				}

				try 
				{
					f = _first.Length;
				}
				catch(Exception) {

					throw new NotImplementedException ("first does not have Length");
				}

				try 
				{
					s = _second.Length;
				}
				catch(Exception) {
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

