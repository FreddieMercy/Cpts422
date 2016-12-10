using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CS422
{
	public class FilesWebService : WebService
	{
		private FileSys422 _fs;
		private const int FileBufSize = 4096;
		private const string uri = "/files";
		private bool m_allowUploads = true;
		
		private readonly string _listFormateString = "<html>" +
			"{0}"+
			"<h1>Folders</h1>" +
			"{1}"+
			"<h1>Files</h1>" +
			"{2}" +
			"<br><br>"+
			"{3}"+
			"</html>";

		private readonly string _format = "<a href='{0}'>{1}</a><br>";
		private string _path;

		/*
		For the ServiceURI property, 
		return the hard-coded string “/files”. This means you will need 
		you navigate to localhost:4220/files/something in order to download 
		files from this service in the browser
		*/

		public override string ServiceURI { get{ return uri; } }

		public FilesWebService(FileSys422 fs)
		{
			/*
			Constructor for the service that takes a reference to the file system to share. 
			All content from this file system will be shared by your service. 
			Recall from the previous homework that you can construct a StandardFileSystem (which inherits from FileSys422) 
			from any folder on your computer and it will not let content above that directory be shared
			*/

			if(fs == null)
			{

				throw new NullReferenceException ("fs == null");
			}

			_fs = fs;
			_path = null;
		}

		public override void Handler (WebRequest req)
		{
			/*
			1. First note that the web server core will not send any requests to this function that 
			   don’t start with the string “/files” (provided you implemented the server core correctly).
			2. Parse the URI and determine which of the 3 is the case:
				1) The URI maps to an existing directoryin the file system
				2) The URI maps to an existing file in the file system
				3) The URI maps to something that doesn’t exist in the file systemo
			3. Handle the request appropriately based on the case
				1) File listing in HTML (further described below) for a directory URI
				2) File contents (remember: as the response body!) for a file URI'
				3) 404  for any other UR
			4. Provide support for partial content responses (i.e. support the Range header)
				1) https://tools.ietf.org/html/rfc7233
			*/

			if(req == null)
			{
				throw new NullReferenceException ("req == null");

			}

			string[] path = req.RequestTarget_URI.Split ('/');
			string tmp = req.RequestTarget_URI.Substring (ServiceURI.Length, req.RequestTarget_URI.Length - ServiceURI.Length);
			int i = 0;
			Dir422 current = _fs.GetRoot ();
			_path = req.RequestTarget_URI;

			if (path [1] != uri.Substring(1, uri.Length-1)) {
				req.WriteNotFoundResponse ("");
				return;
			}

			if (req.RequestTarget_URI == null | string.IsNullOrEmpty (req.RequestTarget_URI) | string.IsNullOrWhiteSpace (req.RequestTarget_URI)) {
				req.WriteNotFoundResponse ("");
				return;
			}

			if (tmp.Length == 0) {

				req.WriteHTMLResponse(BuildDirHTML (current));
				return;

			}

			if (tmp [tmp.Length - 1] == '/') {
				tmp = tmp.Substring (0, tmp.Length - 1);
			}

			if (tmp [0] == '/') {
				tmp = tmp.Substring (1, tmp.Length - 1);
			}

			path = tmp.Split ('/');

			//ABOVE: make /a/b/c/d/ or /a/b/c/d -> a/b/c/d and then split

			//BELOW: cd to dir if exists, otherwise return 404

			while (path [i] != path [path.Length - 1] && current.ContainsDir (path [i], false)) {

				current = current.GetDir (path [i]);
				i++;

			}

			if (path [i] == path [path.Length - 1]) {
				if (current.ContainsDir (path [i], false)) {

					if (req.Method == "GET") {
						req.WriteHTMLResponse (BuildDirHTML (current.GetDir (path [i])));
					//} else if (req.Method == "PUT") { //since we want to upload file, folder with the same name does not matter
						return;
					}


				} else if (current.ContainsFile (path [i], false)) {

					if (req.Method == "GET") {
						//download file
						string s = Path.GetExtension (path [i]);
						if (String.Equals (s.Substring (1, s.Length - 1), "JPEG", StringComparison.OrdinalIgnoreCase) ||
						    String.Equals (s.Substring (1, s.Length - 1), "PNG", StringComparison.OrdinalIgnoreCase) ||
						    String.Equals (s.Substring (1, s.Length - 1), "PDF", StringComparison.OrdinalIgnoreCase) ||
						    String.Equals (s.Substring (1, s.Length - 1), "MP4", StringComparison.OrdinalIgnoreCase) ||
						    String.Equals (s.Substring (1, s.Length - 1), "TXT", StringComparison.OrdinalIgnoreCase) ||
						    String.Equals (s.Substring (1, s.Length - 1), "HTML", StringComparison.OrdinalIgnoreCase) ||
						    String.Equals (s.Substring (1, s.Length - 1), "XML", StringComparison.OrdinalIgnoreCase)) {// JPEG, PNG, PDF, MP4, TXT, HTML and XML")
	
							Stream file = (current.GetFile (path [i]) as StdFSFile).OpenReadOnly ();

							if (file == null) {
								req.WriteNotFoundResponse ("");
								return;
							}						

							req.WriteHTMLResponse ("");

							file.Seek (0, SeekOrigin.Begin);

							while (true) {
								byte[] buf = new byte[FileBufSize];
								int read = file.Read (buf, 0, FileBufSize);

								if (read == 0) {
									break;
								}

								req.ClientStream.Write (buf, 0, read);

							}

							return;
						}

						req.WriteNotFoundResponse ("");

					} else if (req.Method == "PUT") {

						//cannot upload file since there is one with the same name already

						req.WriteNotFoundResponse ("<html>There is file with the same name</html>");
					} 

					//leave "return" here just in case
					return;
				}

				else {
					if (req.Method == "PUT") {

						//upload file

						/*

						1. 	So, once you’ve got the code in place to create an open the proper file for a PUT request,you will then 
							just read all the data from the body of the request and write it to the file. 

						2.	Write a regular “200 OK” response if the upload completes successfully. 
							This result goes to the JavaScript and you will not see it in the browser (although the simple JavaScript you’ve been given does display the status code
							from the upload). Make sure the additional requirements are also met:

								* Don’t allow overwriting existing files (X)
								* Allow all (non-existing-file-overwriting) uploads provided the file name doesn’t contain reserved characters (/ and \) (X)
								* Files can be uploaded to any directory that the service shares (X)
								* Files must be uploaded into the current directory that the user is seeing the listing for within the browser (X)
								  (in other words you can’t just have every upload just always put the file in the root directory)
								* There is no limit on file sizes or content (X)

						*/

						//fetch out the extension and length(??), and add in "file" declartion!!!


						if (path [i].Contains ("/") | path [i].Contains ("\\")) {
							req.WriteNotFoundResponse ("illegal filename! (path[i].Contains('/')|path[i].Contains('\\'))");
							return;
						}

						string extension = "";
						//long fileLength = 0;

						if (req.Headers.ContainsKey ("Content-Type")) {
							extension = req.Headers ["Content-Type"].Split ('/') [1];
							if (extension == "plain") {
								extension = "txt";
							}
						}

						/*
						if(req.Headers.ContainsKey("Content-Length"))
						{
							extension = Convert.ToUInt64(req.Headers["Content-Length"]);
						}
*/

						Stream file;

						if (path [i].Contains ("." + extension)) {
							file = (current.CreateFile (path [i]) as StdFSFile).OpenReadWrite ();

						} else if (!string.IsNullOrEmpty (extension) && !string.IsNullOrWhiteSpace (extension)) {
							file = (current.CreateFile (path [i] + "." + extension) as StdFSFile).OpenReadWrite ();
						} else {
							file = (current.CreateFile (path [i]) as StdFSFile).OpenReadWrite ();
						}

						if (file == null) {
							req.WriteNotFoundResponse ("");
							return;
						}						

						//req.WriteHTMLResponse (string.Format (DefaultTemplate, Path.GetExtension (path [i]), file.Length));

						file.Seek (0, SeekOrigin.Begin);

						//Console.Write ("body: ");
								
						StreamReader sr = new StreamReader (req.ClientStream);
						//string s = "";
						char c;
						while (sr.Peek () >= 0) {
							c = (char)sr.Read ();
							//s += c;
							file.Write (BitConverter.GetBytes(c), 0, 1);

						}

						//Console.Write (s);

						req.WriteHTMLResponse ("");

						file.Close ();

						return;
					}
				}
			}

			req.WriteNotFoundResponse ("");

			return;

		}

		public string BuildDirHTML(Dir422 directory)
		{
			/*
			1. This utility function builds the HTML string for a web page that provides a list of directory contents.
			2. Since learning HTML is not really a major goal of the class, you are given the generic HTML format below. 
			   You can add additional formatting if you are familiar with HTML (and perhaps CSS). The only major requirements are:
			   		1) Have valid, working links for all files and folders in the directory, in list (one link per “line”) 
			   		   on the page. It should be easy for anyone to navigate the page.
			   		2) The list of folder andfile links are separated on the page such that it’s easy to distinguish them visually. 
			   		   Since there is no requirement for icons by the links or any special formatting on the links, it might be difficult for 
			   		   the user to distinguish a link as going to another folder vs downloading a file if all the file and folder links were 
			   		   in one list and not separated sections.
			   		3) The text of the link is the file or folder name only, not the full URI
			*/

			//string template = "";
			string file = "";
			string dir = "";
			string script = "";
			string uploader = "";

			foreach (Dir422 x in directory.GetDirs()) {
				if (x is Dir422) {

					dir += string.Format (_format, _path + "/" + (x as Dir422).Name, (x as Dir422).Name);

				} else {
					throw new Exception ("some items in the directory has unknown type");

				}
			}

			foreach (File422 x in directory.GetFiles()) {
				if (x is File422) {

					file += string.Format (_format, _path + "/" + (x as File422).Name, (x as File422).Name);

				} else {
					throw new Exception ("some items in the directory has unknown type");

				}
			}

			if (m_allowUploads) {
				script = @"<script>

								function selectedFileChanged(fileInput, urlPrefix)
								{
									document.getElementById('uploadHdr').innerText = 'Uploading ' + fileInput.files[0].name + '...';

									// Need XMLHttpRequest to do the upload
									if (!window.XMLHttpRequest)
									{
										alert('Your browser does not support XMLHttpRequest. Please update your browser.');
										return;
									}

									// Hide the file selection controls while we upload
									var uploadControl = document.getElementById('uploader');
									if (uploadControl)
									{
										uploadControl.style.visibility = 'hidden';
									}

									// Build a URL for the request
									if (urlPrefix.lastIndexOf('/') != urlPrefix.length - 1)
									{
										urlPrefix += '/';
									}

									var uploadURL = urlPrefix + fileInput.files[0].name;
									// Create the service request object
									var req = new XMLHttpRequest();
									req.open('PUT', uploadURL);
									req.onreadystatechange = function()
									{
										document.getElementById('uploadHdr').innerText = 'Upload (request status == ' + req.status + ')';
										// Un-comment the line below and comment-out the line above if you want the page to
										// refresh after the upload
										//location.reload();
									};

									req.send(fileInput.files[0]);
								}
								</script>
								
				";
			}

			if (m_allowUploads)
			{
				uploader = string.Format("<hr><h3 id='uploadHdr'>Upload</h3><br>" +
					"<input id=\"uploader\" type='file' " +
					"onchange='selectedFileChanged(this,\"{0}\")' /><hr>",
					GetHREF(directory, true));
			};

			return string.Format (_listFormateString, script, dir, file, uploader);
		}

		private string GetHREF(Dir422 dir, bool abs)
		{
			string path = "";

			if (dir.Parent != null) {

				path = GetHREF (dir.Parent, false) +  dir.Name+"/";

			} else {
				path = uri+"/" + path;
			}

			if(abs)
			{
				path = path.Substring (0, path.Length - 1);
				//Console.WriteLine (path);
			}

			return path;
		}

	}
}

