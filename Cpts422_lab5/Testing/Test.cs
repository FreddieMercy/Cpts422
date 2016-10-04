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
		public void All_General_Test ()
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

						Assert.AreEqual (0, z.Position);

						Assert.AreEqual (0, j.Position);

						Assert.AreEqual (true, s.CanRead);
						Assert.AreEqual (true, s.CanWrite);
						Assert.AreEqual (true, s.CanSeek);

						Assert.AreEqual (false, i.CanRead);
						Assert.AreEqual (false, i.CanWrite);
						Assert.AreEqual (false, i.CanSeek);

						i.Position = 1;
						Assert.AreEqual (1, i.Position);

						i.Position = s.Length - 2;
						Assert.AreEqual (s.Length - 2, i.Position);
						Assert.AreEqual (s.Length - 2, s.Position);

						i.Position = s.Length - 1;
						Assert.AreEqual (s.Length - 1, i.Position);
						Assert.AreEqual (s.Length - 1, s.Position);

						i.Position = s.Length+100;
						Assert.AreEqual (s.Length+100, i.Position);
						Assert.AreEqual (s.Length - 1, s.Position);
						Assert.AreEqual (101, z.Position);

						//Assert.Throws<IndexOutOfRangeException>(()=>{i.Position = 10000000;});

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

						j.Position = s.Length + m.Length-1;
						Assert.AreEqual (s.Length + m.Length-1, j.Position);
						Assert.AreEqual (s.Length - 1, s.Position);
						Assert.AreEqual (s.Length + m.Length-1 - s.Position, m.Position);

						j.Position = s.Length;
						Assert.AreEqual (s.Length, j.Position);
						Assert.AreEqual (s.Length - 1, s.Position);
						Assert.AreEqual (1, m.Position);
					
						Assert.Throws<IndexOutOfRangeException>(()=>{j.Position = 10000000;});


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
						Assert.Throws<ArgumentException> (() => {

							using (ConcatStream c = new ConcatStream (a, b)) {

							}
						});

						Assert.Throws<ArgumentOutOfRangeException> (() => {

							using (ConcatStream c = new ConcatStream (d, b, -1)) {

							}
						});

						Assert.Throws<ArgumentOutOfRangeException> (() => {

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
		}

		[Test ()]
		public void Write_Test ()
		{

		}
	}
}

