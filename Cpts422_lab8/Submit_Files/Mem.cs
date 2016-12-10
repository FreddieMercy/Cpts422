using System.Threading;
using CS422;
using NUnit.Framework;

namespace Mem
{
	[TestFixture]
	public class MemFsTest
	{
		private static MemoryFileSystem _fs;
		private static string RootDirName = "/";

		[TestFixtureSetUp]
		public void Init()
		{
			_fs = new MemoryFileSystem();
		}

		[Test]
		public void Test_CreateMemFileSystem()
		{
			Assert.IsNotNull(_fs);
			Assert.IsNotNull(_fs.GetRoot());
			Assert.AreEqual(_fs.GetRoot().Name, RootDirName);
			Assert.IsNull(_fs.GetRoot().Parent);
		}

		[Test]
		public void Test_CreateFile()
		{
			var root = _fs.GetRoot();
			Assert.IsNotNull(root);

			var newFile1 = root.CreateFile("file1.txt");
			var newFile2 = root.CreateFile("file2.txt");
			var newFile3 = root.CreateFile("dir1/file3.txt");
			var newFile4 = root.CreateFile("");
			var newFile5 = root.CreateFile(null);

			Assert.IsNotNull(newFile1);
			Assert.IsNotNull(newFile2);
			Assert.IsNull(newFile3);
			Assert.IsNull(newFile4);
			Assert.IsNull(newFile5);
		}

		[Test]
		public void Test_CreateDirectory()
		{
			var root = _fs.GetRoot();
			Assert.IsNotNull(root);

			var newDir1 = root.CreateDir("outer1");
			var newDir2 = root.CreateDir("outer2");
			var newDir3 = root.CreateDir("outer3");
			var newDir4 = root.CreateDir("outer3/inner1");
			var newDir5 = root.CreateDir("");
			var newDir6 = root.CreateDir(null);

			Assert.IsNotNull(newDir1);
			Assert.IsNotNull(newDir2);
			Assert.IsNotNull(newDir3);
			Assert.IsNull(newDir4);
			Assert.IsNull(newDir5);
			Assert.IsNull(newDir6);
		}

		[Test]
		public void Test_GetSingleFile()
		{
			var root = _fs.GetRoot();
			Assert.IsNotNull(root);

			root.CreateFile("file1.txt");
			root.CreateFile("file2.txt");


			var file1 = root.GetFile("file1.txt");
			var file2 = root.GetFile("file2.txt");
			var file3 = root.GetFile("not_a_file");
			var file4 = root.GetFile("outer2/file1.txt");
			var file5 = root.GetFile("");
			var file6 = root.GetFile(null);

			Assert.IsNotNull(file1);
			Assert.IsNotNull(file2);
			Assert.IsNull(file3);
			Assert.IsNull(file4);
			Assert.IsNull(file5);
			Assert.IsNull(file6);
		}

		[Test]
		public void Test_GetSingleDirectory()
		{
			var root = _fs.GetRoot();
			Assert.IsNotNull(root);

			root.CreateDir("outer1");
			root.CreateDir("outer2");

			var dir1 = root.GetDir("outer1");
			var dir2 = root.GetDir("outer2");
			var dir3 = root.GetDir("not_a_dir");
			var dir4 = root.GetDir("outer2/inner1");
			var dir5 = root.GetDir("");
			var dir6 = root.GetDir(null);

			Assert.IsNotNull(dir1);
			Assert.IsNotNull(dir2);
			Assert.IsNull(dir3);
			Assert.IsNull(dir4);
			Assert.IsNull(dir5);
			Assert.IsNull(dir6);
		}

		[Test]
		public void Test_GetFiles()
		{
			var root = _fs.GetRoot();
			Assert.IsNotNull(root);

			var testDir = root.CreateDir("testFiles");

			testDir.CreateFile("file1.txt");
			testDir.CreateFile("file2.txt");

			var files = testDir.GetFiles();
			Assert.AreEqual(files.Count, 2);
			testDir.CreateFile("newFileTest.pdf");
			files = testDir.GetFiles();
			Assert.AreEqual(files.Count, 3);
		}

		[Test]
		public void Test_GetDirectories()
		{
			var root = _fs.GetRoot();
			Assert.IsNotNull(root);

			var testDir = root.CreateDir("testDirs");

			testDir.CreateDir("outer1");
			testDir.CreateDir("outer2");
			testDir.CreateDir("outer3");

			var dirs = testDir.GetDirs();
			Assert.AreEqual(dirs.Count, 3);
			testDir.CreateDir("outer4");
			dirs = testDir.GetDirs();
			Assert.AreEqual(dirs.Count, 4);
		}

		[Test]
		public void Test_FileSystemContains()
		{
			var root = _fs.GetRoot();
			Assert.IsNotNull(root);

			var outer1 = root.CreateDir("outer1");
			var outer2 = root.CreateDir("outer2");
			var outer3 = root.CreateDir("outer3");

			var file1 = root.GetDir("outer1").CreateFile("file1.pdf");
			var file2 = root.GetDir("outer2").CreateFile("file2.pfd");
			var file3 = root.GetDir("outer3").CreateDir("inner1").CreateFile("colin.xlsx");

			Assert.IsTrue(_fs.Contains(outer1));
			Assert.IsTrue(_fs.Contains(outer2));
			Assert.IsTrue(_fs.Contains(outer3));
			Assert.IsTrue(_fs.Contains(file1));
			Assert.IsTrue(_fs.Contains(file2));
			Assert.IsTrue(_fs.Contains(file3));
		}

		[Test]
		public void Test_OpenFilesNonThreaded()
		{
			var root = _fs.GetRoot();

			root.CreateFile("file1.txt");

			/* Open three instances of file1.txt for READ-ONLY */
			var openedFile1 = root.GetFile("file1.txt").OpenReadOnly();
			Assert.IsNotNull(openedFile1);
			var openedFile2 = root.GetFile("file1.txt").OpenReadOnly();
			Assert.IsNotNull(openedFile2);
			var openedFile3 = root.GetFile("file1.txt").OpenReadOnly();
			Assert.IsNotNull(openedFile3);

			/* Attempt to open an instance of file1.txt for READ/WRITE -- should fail */
			var nullStream = root.GetFile("file1.txt").OpenReadWrite();
			Assert.IsNull(nullStream);

			/* Dispose of the READ-ONLY file streams */
			openedFile1.Dispose();
			openedFile2.Dispose();
			openedFile3.Dispose();

			/* Now that there are no instances of the file opened, attempt
             * to open for READ/WRITE */
			var nonNullStream = root.GetFile("file1.txt").OpenReadWrite();
			Assert.IsNotNull(nonNullStream);

			/* Ensure that if a file is opened for write that it cannot then be opened for
             * read OR write. */
			openedFile1 = root.GetFile("file1.txt").OpenReadOnly();
			Assert.IsNull(openedFile1);
			nonNullStream.Dispose();

			openedFile1 = root.GetFile("file1.txt").OpenReadOnly();
			Assert.IsNotNull(openedFile1);
			openedFile1.Dispose();
		}

		[Test]
		public void Test_OpenFilesThreaded()
		{
			_fs.GetRoot().CreateFile("file1.txt");
			_fs.GetRoot().CreateFile("file2.txt");
			_fs.GetRoot().CreateFile("file3.txt");

			System.Threading.Thread work1 = new Thread(OpenFilesWork);
			System.Threading.Thread work2 = new Thread(OpenFilesWork);
			System.Threading.Thread work3 = new Thread(OpenFilesWork);

			work1.Start();
			work2.Start();
			work3.Start();
		}

		private void OpenFilesWork()
		{
			var root = _fs.GetRoot();

			var openFile1 = root.GetFile("file1.txt").OpenReadOnly();
			var openFile2 = root.GetFile("file1.txt").OpenReadOnly();
			var openFile3 = root.GetFile("file1.txt").OpenReadOnly();

			Assert.IsNotNull(openFile1);
			Assert.IsNotNull(openFile2);
			Assert.IsNotNull(openFile3);
		}
	}
}