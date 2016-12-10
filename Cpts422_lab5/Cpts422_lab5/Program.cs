using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Generic;
using CS422;

namespace Cpts422_lab5
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			byte[] buf1 = { 1, 2, 3, 4, 5, 6, 7, 8 };
			byte[] buf2 = { 10, 20, 30, 40, 50, 60, 70, 80 };

			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf = new byte[20];

						stream.Read (buf, 0, 20);

						//stream.Read(buf, 0, 4);
						//mem1.Position = 10000000000;

						foreach (var value in buf) {
							Console.WriteLine (value);
						}


					}
				}
			}

			MemoryStream s3 = new MemoryStream ();

			Console.WriteLine ("s3.position = " + s3.Position);
			s3.Write (buf1, 0, 8);
			Console.WriteLine ("s3.position = " + s3.Position);


			byte[] buf4 = new byte[10];
			byte[] buf5 = new byte[10];
			using (MemoryStream s1 = new MemoryStream (buf1)) {
				using (MemoryStream s2 = new MemoryStream (buf2)) {
					using (ConcatStream stream = new ConcatStream (s1, s2)) {
						byte[] buf3 = {150,151,152,153,154,155,156,157,158,159,160};
						byte[] buf = new byte[200];
						stream.Write (buf3, 0, 5);
						stream.Position = 0;
						stream.Read (buf, 0, 200-1);
						Console.WriteLine ("write buf: ");
						foreach (var value in buf) {
							Console.WriteLine (value);
						}
					}
				}
			}

		}
	}
}
