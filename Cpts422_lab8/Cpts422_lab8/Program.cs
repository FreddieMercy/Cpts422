using System;
using CS422;

namespace Cpts422_lab3
{
	class MainClass
	{
		private const string DefaultTemplate =
			"HTTP/1.1 200 OK\r\n" +
			"Content-Type: text/html\r\n" +
			"\r\n\r\n" +
			"<html>ID Number: {0}<br>" +
			"DateTime.Now: {1}<br>" +
			"Requested URL: {2}</html>";
		
		public static void Main (string[] args)
		{
			string s = "/a/b/c/";

			string[] x = s.Split ('/');
			Console.WriteLine (x.Length);
			foreach (string y in x) {
				Console.WriteLine (y);
			}

			Console.WriteLine ("path = " + s);

			MemoryFileSystem _fs = new MemoryFileSystem();

			var root = _fs.GetRoot();

			root.CreateFile("file1.txt");

			var openedFile1 = root.GetFile("file1.txt").OpenReadOnly();

			var openedFile2 = root.GetFile("file1.txt").OpenReadOnly();

			var openedFile3 = root.GetFile("file1.txt").OpenReadOnly();

			/* Attempt to open an instance of file1.txt for READ/WRITE -- should fail */
			var nullStream = root.GetFile("file1.txt").OpenReadWrite();

			/* Dispose of the READ-ONLY file streams */
			openedFile1.Dispose();
			openedFile2.Dispose();
			openedFile3.Dispose();

			var nonNullStream = root.GetFile("file1.txt").OpenReadWrite();

		}
	}
}
