using System;
using System.IO;
using NUnit.Framework;
using CS422;
using System.Net.Sockets;

namespace Testing
{
	//Test Cases:

	//	* Seek
	//	* Read
	//	* Write

	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void General_Test_for_fun ()
		{
			using (Stream s = new MemoryStream ()) {
				for (int i = 0; i < 122; i++) {
					s.WriteByte ((byte)i);
				}
				s.Position = 10;
				using (Stream m = new MemoryStream()) {
					for (int i = 0; i < 122; i++) {
						m.WriteByte ((byte)i);
					}
					m.Position = 20;

					using(Stream z = new TestStream())
					{
						z.Position = 10;

						ConcatStream j = new ConcatStream (s,m);
						ConcatStream i = new ConcatStream (s, z);
						ConcatStream k = new ConcatStream (s, z, 10000);


						Assert.AreEqual (s.Position+m.Position, j.Position);

						Assert.AreEqual (true, s.CanRead);
						Assert.AreEqual (true, s.CanWrite);
						Assert.AreEqual (true, s.CanSeek);

						Assert.AreEqual (false, i.CanRead);
						Assert.AreEqual (false, i.CanWrite);
						Assert.AreEqual (false, i.CanSeek);

						Assert.Throws<NotSupportedException>(()=>{i.Position = 1;});

						Assert.AreEqual (10000, k.Length);

						Assert.AreEqual (true, j.CanRead);
						Assert.AreEqual (true, j.CanWrite);
						Assert.AreEqual (true, j.CanSeek);
						Assert.AreEqual (s.Length+m.Length, j.Length);

						j.Position = 1;
						Assert.AreEqual (1, j.Position);

						j.Position = s.Length - 2;
						Assert.AreEqual (s.Length - 2, j.Position);
						Assert.AreEqual (s.Length - 2, s.Position);

						j.Position = s.Length - 1;
						Assert.AreEqual (s.Length - 1, j.Position);
						Assert.AreEqual (s.Length - 1, s.Position);

						j.Position = s.Length;
						Assert.AreEqual (s.Length, j.Position);
						Assert.AreEqual (s.Length - 1, s.Position);

						Assert.AreEqual (1, m.Position);
					
						Assert.Throws<ArgumentOutOfRangeException>(()=>{j.Position = 10000000;});


						i.Dispose ();
						j.Dispose ();
						k.Dispose ();
					}
				}
			}

		}

		[Test ()]
		public void Second_Constructor_Test ()
		{
			using(Stream a = new TestStream())
			{
				using (Stream b = new MemoryStream ())
				{
					using (Stream d = new MemoryStream ()) {

						Assert.AreEqual (0, b.Length);
						/*
						Assert.Throws<ArgumentException> (() => {

							using (ConcatStream c = new ConcatStream (a, b)) {

							}
						});
						*/

						Assert.Throws<ArgumentException> (() => {

							using (ConcatStream c = new ConcatStream (d, b, -1)) {

							}
						});

						Assert.Throws<ArgumentException> (() => {

							using (ConcatStream c = new ConcatStream (d, b, 0)) {

							}
						});
					}
				}
			}
		}

		[Test ()]
		public void Seek_Test ()
		{
			
		}

		[Test ()]
		public void Read_Test ()
		{
			
			byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8 };
			byte[] buf2 = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 101, 102, 103, 104 };

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[20];
						byte[] test = buf;
						s1.Position = 0;
						long pos1 = s1.Position;
						long pos2 = s2.Position;
						int beg = 0;
						int end = buf.Length;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[2];
						byte[] test = buf;
						s1.Position = 0;
						long pos1 = s1.Position;
						long pos2 = s2.Position;
						int beg = 0;
						int end = buf.Length;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[12];
						byte[] test = buf;
						s1.Position = 0;
						long pos1 = s1.Position;
						long pos2 = s2.Position;
						int beg = 0;
						int end = buf.Length;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[2];
						byte[] test = buf;
						s1.Position = 2;
						s2.Position = 3;
						long pos1 = s1.Position;
						long pos2 = s2.Position;
						int beg = 0;
						int end = buf.Length;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[12];
						byte[] test = buf;
						s1.Position = 2;
						s2.Position = 3;
						long pos1 = s1.Position;
						long pos2 = s2.Position;
						int beg = 0;
						int end = buf.Length;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[12];
						byte[] test = buf;
						s1.Position = s1.Length-1;
						s2.Position = 3;
						long pos1 = s1.Position;
						long pos2 = s2.Position;
						int beg = 0;
						int end = buf.Length;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}
		}

		[Test ()]
		public void Write_Test ()
		{

			byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8 };
			byte[] buf2 = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 101, 102, 103, 104 };

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {

						byte[] buf = new byte[20];
						byte[] test = buf;

						long pos1 = s1.Position;
						long pos2 = s2.Position;

						byte[] tar = {150,151,152,153,154,155,156,157,158,159,160};
						int beg = 0;
						int end = tar.Length - 1;

						stream.Write (tar, beg, end);
						s1.Position = pos1;
						s2.Position = pos2;
						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[200];
						byte[] test = buf;
						s1.Position = 0;
						long pos1 = s1.Position;
						long pos2 = s2.Position;

						byte[] tar = {150,151,152,153,154,155,156,157,158,159,160};
						int beg = 0;
						int end = tar.Length - 1;

						stream.Write (tar, beg, end);
						s1.Position = pos1;
						s2.Position = pos2;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[12];
						byte[] test = buf;
						s1.Position = 0;
						long pos1 = s1.Position;
						long pos2 = s2.Position;

						byte[] tar = {150,151,152,153,154,155,156,157,158,159,160};
						int beg = 0;
						int end = tar.Length - 1;

						stream.Write (tar, beg, end);
						s1.Position = pos1;
						s2.Position = pos2;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[200];
						byte[] test = buf;
						s1.Position = 2;
						s2.Position = 3;
						long pos1 = s1.Position;
						long pos2 = s2.Position;

						byte[] tar = {150,151,152,153,154,155,156,157,158,159,160};
						int beg = 0;
						int end = tar.Length - 1;

						stream.Write (tar, beg, end);
						s1.Position = pos1;
						s2.Position = pos2;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[200];
						byte[] test = buf;
						s1.Position = 2;
						s2.Position = 3;
						long pos1 = s1.Position;
						long pos2 = s2.Position;

						byte[] tar = {150,151,152,153,154,155,156,157,158,159,160};
						int beg = 0;
						int end = tar.Length - 1;

						stream.Write (tar, beg, end);
						s1.Position = pos1;
						s2.Position = pos2;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[200];
						byte[] test = buf;
						s1.Position = s1.Length-1;
						s2.Position = 3;
						long pos1 = s1.Position;
						long pos2 = s2.Position;

						byte[] tar = {150,151,152,153,154,155,156,157,158,159,160};
						int beg = 0;
						int end = tar.Length - 1;

						stream.Write (tar, beg, end);
						s1.Position = pos1;
						s2.Position = pos2;

						stream.Read (buf, beg, end);

						for (int i = (int)pos1; i < buf1.Length; i++) {
							if(i-pos1>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf1[i], buf[i - pos1]);
						}

						for (int i = (int)pos2; i < buf2.Length; i++) {
							if(i+buf1.Length-pos1-pos2>=end-beg)
							{
								break;
							}
							Assert.AreEqual (buf2[i], buf[i+buf1.Length-pos1-pos2]);
						}

						for (int i = buf1.Length + buf2.Length-(int)pos1-(int)pos2; i < end-beg; i++) {

							Assert.AreEqual (test [i], buf [i]); 
						}

					}
				}
			}

		}

	}
		
}

