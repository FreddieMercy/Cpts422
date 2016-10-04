using System;
using System.IO;

namespace CS422
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			IndexedNumsStream t = new IndexedNumsStream (0, 100);
			byte[] buffer = new byte[512];
			int read = t.Read(buffer, 80, 220);
			Console.WriteLine ("read = " + read.ToString ());


			var s = new IndexedNumsStream(100,100);

			s.Dispose ();

			try
			{
				Console.WriteLine(s.Length);
			}
			catch(ObjectDisposedException) {
			
			}

			/*
			using (var stringWriter = new StringWriter ()) 
			{
				stringWriter.Dispose ();

				try
				{
					stringWriter.Write("a");
				}
				catch(ObjectDisposedException) {

					throw new ObjectDisposedException ("String Error");

				}
			}

			using (var stringWriter = new StringWriter ()) 
			{
				var writer = new NumberedTextWriter (stringWriter);

				writer.Dispose ();

				try
				{
					writer.WriteLine ("nihaoma");
				}
				catch(ObjectDisposedException) {

					throw new ObjectDisposedException ("Error");
					
				}
			}

			*/
		}
	}
}
