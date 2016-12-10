using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 7
 */

namespace CS422
{
	public class WebRequest
	{
		private const string DefaultTemplate =
			"HTTP/1.1 {0}\r\n" +
			"Content-Type: text/html\r\n" +
			"{1}" +
			"\r\n\r\n" +
			"<html>URI:{2}<br>DateNow:{3}<br>{4}</html>";
		
		public Stream Body { get; private set;}
		public ConcurrentDictionary<string,string> Headers { get; private set;}
		public string Method { get; private set;}
		public string RequestTarget_URI { get; private set;}
		public string HTTPVersion { get; private set;}
		public bool Success { get; private set;}
		private validationMethodsandFields validate;

		private NetworkStream _net = null;

		public TcpClient Client{
			set{
				_net = (value as TcpClient).GetStream();
				validate.parseReqFromString (value);
				Method = validate.method;
				HTTPVersion = validate.version;
				RequestTarget_URI = validate.uri;
				Headers = validate.headers;
				Success = validate.success;

				if (validate.length >= 0) {
					Body = new ConcatStream (new MemoryStream(Encoding.ASCII.GetBytes(validate.request)), _net, validate.length);
				} else {
					Body = new ConcatStream (new MemoryStream(Encoding.ASCII.GetBytes(validate.request)), _net);
				}

			}

		}

		public NetworkStream ClientStream{

			get{
				return _net;
			}
		}

		public WebRequest ()
		{
			Method = "";
			RequestTarget_URI = "";
			HTTPVersion = "";
			Headers  = new ConcurrentDictionary <string, string>();
			validate = new validationMethodsandFields ();
		}

		//This allows the handlers/services, or the web server core in the case of no 
		//appropriate service for the URI, to easily write responses to the clients.

		public void WriteNotFoundResponse(string pageHTML)
		{
			//The first writes a response with a 404 status code and the specified HTML string as the body of
			//the response. 
			if (pageHTML == null) {
				throw new ArgumentNullException ("pageHTML == null");
			}

			string bodyText = "Failed";
			//Body.Write (Encoding.ASCII.GetBytes(bodyText), 0, bodyText.Length-1);
			byte[] response;
			bool len = true;


			try {
				long en = Body.Length;
			} catch (Exception) {
				len = false;
			}

			try {
				if (len) {
					response = Encoding.ASCII.GetBytes (string.Format (pageHTML, "404 not OK", "Content-Length: "+bodyText.Length+"\r\n", RequestTarget_URI, DateTime.Now, bodyText));
				} else {
					response = Encoding.ASCII.GetBytes (string.Format (pageHTML, "404 not OK", "", RequestTarget_URI, DateTime.Now, bodyText));
				}

				_net.Write (response, 0, response.Length-1);
			} catch (Exception) {
				if (len) {
					response = Encoding.ASCII.GetBytes (string.Format (DefaultTemplate, "404 not OK", "Content-Length: "+bodyText.Length+"\r\n", RequestTarget_URI, DateTime.Now, bodyText));
				} else {
					response = Encoding.ASCII.GetBytes (string.Format (DefaultTemplate, "404 not OK", "", RequestTarget_URI, DateTime.Now, bodyText));
				}
				_net.Write (response, 0, response.Length-1);
			}

		}

		public bool WriteHTMLResponse(string htmlString)
		{
			//The second writes a response with a 200 status code and the specified HTML string as the
			//body of the response. 
			string id = "11356533";
			byte[] response;
			try {
				response= Encoding.ASCII.GetBytes(string.Format (htmlString, Method, RequestTarget_URI,"", id));
			}
			catch(Exception) {
				this.WriteNotFoundResponse ("");
				return false;
			}

			try
			{
				_net.Write(response, 0, response.Length-1);
			}
			catch(ObjectDisposedException) {

				Console.WriteLine("timeout, client had been disposed.");

			}
			return true;
		}

		/*
		Both functions create and write the response line, response headers, and double break before
		writing the HTML string to the network stream. In other words, the strings for HTML content are to be
		the bodies of the responses. For the response headers, include at a minimum:
		Content-Type: text/html
		and
		Content-Length: ___
		where “___” is replaced with the actual content (body) length, in bytes.
		*/

		public Tuple<long,long> GetRangeHeader()
		{
			//??

			return null;
		}

		public static long GetContentLengthOrDefault(ConcurrentDictionary<string,string>headers,long defaultValue)
		{
			if(headers.ContainsKey("Content-Length"))
			{
				string val=headers["Content-Length"];
				long len;
				if(long.TryParse(val,out len))
				{
					return len;
				}

				return defaultValue;
			}

			return defaultValue;
		}

		public bool WriteGenericFileResponse(Stream fileData)
		{

			return false;
		}


		/*

		string GetHTMLListing(Dir422 dir)
		{
		StringBuilder sb = new StringBuilder(“<html>”);
		foreach (File422 file in dir.GetFiles()) {
		sb.AppendFormat(
		“<a href=‘{0}’>{1}</a><br>”,
		GetHREFFromFile422(file),
		file.Name);
		}
		// For you TODO: produce another listing of directories
		sb.Append(“</html>”);
		return sb.ToString();
		}
		*/

		/*

		void SomeFuncInOurService(string URI) {
			if URI maps to directory -> send HTML file listing
			else if URI maps to file -> send file contents
			else -> send 404
			}

		*/
	}
}
