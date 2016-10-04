using System;
using System.IO;

namespace CS422
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			StringWriter s = new StringWriter ();

			NumberedTextWriter t = new NumberedTextWriter (s);

			t.WriteLine ("Hello");

			IndexedNumsStream i = new IndexedNumsStream (0, 100);

			Console.WriteLine (s.ToString ());
		}
	}
}
