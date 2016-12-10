using NUnit.Framework;
using System;
using CS422;
using System.IO;

namespace STD
{
	[TestFixture]
	public class StandardFsTest
	{
		private static string _currentDirectory;
		private static string _testDirectory;
		private static StandardFileSystem _fs;

		private const string RootDirName = "StandardTestDir";

		[TestFixtureSetUp]
		public void Initialize()
		{
			/* Create the root directory */
			_currentDirectory = Directory.GetCurrentDirectory();
			_testDirectory = Path.Combine(_currentDirectory, RootDirName);
			Directory.CreateDirectory(_testDirectory);

			/* Create test directories */
			Directory.CreateDirectory(Path.Combine(_testDirectory, "outer1"));
			Directory.CreateDirectory(Path.Combine(_testDirectory, "outer2"));
			Directory.CreateDirectory(Path.Combine(_testDirectory, "outer1", "inner1"));
			Directory.CreateDirectory(Path.Combine(_testDirectory, "outer1", "inner2"));
			Directory.CreateDirectory(Path.Combine(_testDirectory, "outer2", "inner1"));
			Directory.CreateDirectory(Path.Combine(_testDirectory, "outer2", "inner2"));
			Directory.CreateDirectory(Path.Combine(_testDirectory, "outer2", "inner3"));

			/* Create test files */
			using (File.Create(Path.Combine(_testDirectory, "file1.txt"))) 
			using (File.Create(Path.Combine(_testDirectory, "file2.txt"))) 
			using (File.Create(Path.Combine(_testDirectory, "expenses.xlsx"))) 
			using (File.Create(Path.Combine(_testDirectory, "outer1", "submissions.txt"))) 
			using (File.Create(Path.Combine(_testDirectory, "outer1", "syllabus.pdf"))) 
			using (File.Create(Path.Combine(_testDirectory, "outer2", "mapping.obj"))) 
			using (File.Create(Path.Combine(_testDirectory, "outer1", "inner2", "resume.pdf"))) 
			using (File.Create(Path.Combine(_testDirectory, "outer2", "inner3", "final.docx"))) { /* Empty */ }

			_fs = StandardFileSystem.Create(_testDirectory);
		}

		[Test]
		public void Test_CreateFileSystem()
		{
			Assert.IsNull(StandardFileSystem.Create(Path.Combine(_testDirectory, "does_not_exist")));
			Assert.IsNotNull(_fs);
		}

		[Test]
		public void Test_FileSystemContains()
		{
			var root = _fs.GetRoot();
			var expensesFile = root.GetFile("expenses.xlsx");
			var outer2Dir = root.GetDir("outer2");
			var inner3Dir = outer2Dir.GetDir("inner3");
			var finalFile = inner3Dir.GetFile("final.docx");


			Assert.IsTrue(_fs.Contains(root));
			Assert.IsTrue(_fs.Contains(expensesFile));
			Assert.IsTrue(_fs.Contains(outer2Dir));
			Assert.IsTrue(_fs.Contains(inner3Dir));
			Assert.IsTrue(_fs.Contains(finalFile));
		}

		[Test]
		public void Test_GetRoot()
		{
			var root = _fs.GetRoot();

			Assert.IsNotNull(root);
			Assert.IsNull(root.Parent);
			Assert.AreEqual(root.Name, RootDirName);
		}

		[Test]
		public void Test_ContainsFile()
		{
			var root = _fs.GetRoot();

			/* Non-Recursive */
			Assert.IsTrue(root.ContainsFile("file1.txt", false));
			Assert.IsTrue(root.ContainsFile("file2.txt", false));
			Assert.IsFalse(root.ContainsFile("not_a_file.txt", false));

			Assert.IsTrue(root.ContainsFile("expenses.xlsx", false));
			Assert.IsFalse(root.ContainsFile("expenses.xlsx" + Path.DirectorySeparatorChar, false));
			Assert.IsFalse(root.ContainsFile("submissions.txt", false));
			Assert.IsFalse(root.ContainsFile("final.docx", false));

			/* Recursive */
			Assert.IsFalse(root.ContainsFile("not_a_file", true));
			Assert.IsTrue(root.ContainsFile("file1.txt", true));
			Assert.IsTrue(root.ContainsFile("submissions.txt", true));
			Assert.IsTrue(root.ContainsFile("final.docx", true));
			Assert.IsFalse(root.ContainsFile("resume.pdf" + Path.DirectorySeparatorChar, true));
		}

		[Test]
		public void Test_ContainsDirectory()
		{
			var root = _fs.GetRoot();

			/* Non-recursive */
			Assert.IsTrue(root.ContainsDir("outer1", false));
			Assert.IsTrue(root.ContainsDir("outer2", false));
			Assert.IsFalse(root.ContainsDir("not_a_dir", false));

			Assert.IsFalse(root.ContainsDir("outer2" + Path.DirectorySeparatorChar, false));
			Assert.IsFalse(root.ContainsDir("inner3", false));

			/* Recursive */
			Assert.IsTrue(root.ContainsDir("inner1", true));
			Assert.IsTrue(root.ContainsDir("inner3", true));
			Assert.IsTrue(root.ContainsDir("outer1", true));
			Assert.IsFalse(root.ContainsDir("not_a_dir", true));
			Assert.IsFalse(root.ContainsDir("inner2" + Path.DirectorySeparatorChar, true));
		}

		[Test]
		public void Test_GetSingleFile()
		{
			var root = _fs.GetRoot();

			Assert.IsNotNull(root.GetFile("file1.txt"));
			Assert.IsNotNull(root.GetFile("expenses.xlsx"));

			Assert.IsNull(root.GetFile("not_a_file"));
			Assert.IsNull(root.GetFile("submissions.txt"));
		}

		[Test]
		public void Test_GetSingleDir()
		{
			var root = _fs.GetRoot();

			Assert.IsNotNull(root.GetDir("outer1"));
			Assert.IsNotNull(root.GetDir("outer2"));

			Assert.IsNull(root.GetDir("not_a_dir"));
			Assert.IsNull(root.GetDir("expenses.xlsx"));
		}

		[Test]
		public void Test_OpenFile()
		{
			var root = _fs.GetRoot();

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
		public void Test_CreateFile()
		{
			var root = _fs.GetRoot();

			var newFile = root.CreateFile("newFile.txt");

			Assert.IsNotNull(newFile);
			Assert.AreEqual(newFile.Name, "newFile.txt");
			Assert.AreEqual(newFile.Parent, root);
		}

		[Test]
		public void Test_CreateDirectory()
		{
			var root = _fs.GetRoot();

			var newDir = root.CreateDir("newDir");

			Assert.IsNotNull(newDir);
			Assert.AreEqual(newDir.Name, "newDir");
			Assert.AreEqual(newDir.Parent, root);
		}

		[Test]
		public void Test_AttemptToCreateDuplicate()
		{
			var root = _fs.GetRoot();

			Assert.IsNotNull(root.CreateFile("file1.txt"));
			Assert.IsNotNull(root.CreateFile("file2.txt"));

			Assert.IsNotNull(root.CreateDir("outer1"));
			Assert.IsNotNull(root.CreateDir("outer2"));
		}

		[TestFixtureTearDown]
		public static void Cleanup()
		{
			Directory.Delete(_testDirectory, true);
		}
	}
}
