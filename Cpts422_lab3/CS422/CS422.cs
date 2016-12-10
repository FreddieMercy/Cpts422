
/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 3
 */

using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace CS422
{

	public class WebServer
	{
		private static string req = "";
		private static string method = "";
		private static bool end = false;
		private static string host;

		public static bool Start(int port, string responseTemplate)
		{
			bool active = true;
			StreamWriter sw;
			StreamReader sr;

			if (port < 0)
			{
				Console.WriteLine ("port < 0");
				return false;
			}

			if (responseTemplate == null)
			{
				Console.WriteLine ("responseTemplate == null");
				return false;
			}

			TcpListener listener=new TcpListener(IPAddress.Any,port);

			try
			{
				
				listener.Start();
				/*End of try*/}
			catch(Exception)
			{
				Console.WriteLine ("Access Denied, has no permission");

				return false;
			}

			while (active) {
				// blocking call to accept client    

				using (TcpClient client = listener.AcceptTcpClient ()) {

					//ReadRequestFromClient ();

					sw = new StreamWriter(client.GetStream());
					sr = new StreamReader(client.GetStream());
					//NetworkStream t = client.GetStream ();
					string s = "";
					char c;

					while (sr.Peek () >= 0) {
						c = (char)sr.Read ();
						if (requestValidation (c)) {
							s += c;
							if(end)
							{
								break;
							}
						} else {
							s = null;
							break;
						}
					}

					/*If it’s NOT a valid HTTP request, you will sever the socket connection and return false from the function.*/
					//if (request_was_valid) 

					if (!String.IsNullOrEmpty (s)) {
						
						Console.WriteLine (s);
						//WriteResponseToClient ();
						//Where to input the info??

						sw.Write (string.Format (responseTemplate, 11356533, DateTime.Now, host));

						sw.Dispose ();
						sr.Dispose ();
						client.Close ();

						return true;
					} 
						
					sw.Dispose ();
					sr.Dispose ();
					client.Close ();

					/*end of client*/}

			/*end of while*/}
			
			return false;
		}

		private static bool requestValidation(char c)
		{
			if (c == '\0') {

				Console.WriteLine ("c = '\0'");

				return false;

			} else if (c != '\n') {
				
				req += c;

			} else {

				req += c;

				if(req.Length <= 1)
				{
					return false;

				}
				else if(req[req.Length-2]!='\r')
				{
					return false;
				}

				string[] cmd = req.Substring(0, req.Length-2).Split (':');


				if (cmd.Length > 3) { //not possible for single line
					return false;
				} else if (cmd.Length == 3) { //Host: url: port#
					int n;

					if (cmd [0] == "Host" && Int32.TryParse (cmd [2], out n) && !String.IsNullOrEmpty (cmd [1]) && !String.IsNullOrWhiteSpace (cmd [1])) {
						req = "";
						host = cmd [1] +":"+ cmd [2];
						return true;
					}

					return false;
				} else if (cmd.Length == 2) { //other headers
					//check
					foreach (string x in cmd) {
						if (String.IsNullOrEmpty (x) | String.IsNullOrWhiteSpace (x)) {
							return false;
						}
					}

					//validate

					int i = 0;
					for (i = 0; i < cmd [0].Length; i++) {

						if (cmd [0][i] == ' ') {

						//	cmd [0][i] = '\0';
							break;

						}

					}

					switch (cmd [0].Substring(0,i)) {
					case "Cache-Control":

						break;
					case "Connection":

						break;
					case "Upgrade-Insecure-Requests":

						break;
					case "Pragma":

						break;
					case "Trailer":

						break;
					case "Transfer-Encoding":

						break;
					case "Upgrade":

						break;
					case "Via":

						break;
					case "Warning":

						break;
					case "Accept":

						break;
					case "Accept-Charset":

						break;
					case "Accept-Encoding":

						break;
					case "Accept-Language":

						break;
					case "Authorization":

						break;
					case "Cookie":

						break;
					case "Expect":

						break;
					case "From":

						break;
					case "Host":
						host = cmd [1];
						break;
					case "If-Match":

						break;
					case "If-Modified-Since":

						break;
					case "If-None-Match":

						break;
					case "If-Range":

						break;
					case "If-Unmodified-Since":

						break;
					case "Max-Forwards":

						break;
					case "Proxy-Authorization":

						break;
					case "Range":

						break;
					case "Referer":

						break;
					case "TE":

						break;
					case "User-Agent":

						break;
					case "Allow":

						break;
					case "Content-Encoding":

						break;
					case "Content-Language":

						break;
					case "Content-Location":

						break;
					case "Content-MD5":

						break;
					case "Content-Range":

						break;
					case "Content-Type":

						break;
					case "Expires":

						break;
					case "Last-Modified":

						break;


					default:
						return false;
					}
				} 

				//crsf
				else if(req == "\r\n") {
					end = true;
				}
				else if (cmd.Length == 1) { //length == 1, GET PATH HTTP
					string[] tmp = cmd [0].Split (' ');

					List <string> http = new List<string> ();
					foreach (string x in tmp) {

						if (!String.IsNullOrEmpty (x) && !String.IsNullOrWhiteSpace (x)) {
							http.Add (x);
						}
							
					}

					tmp = http.ToArray ();

					if (tmp [2] != "HTTP/1.1") {

						return false;
					}

					method = tmp [0];
				} 

				req = "";
			}

			return true;

		}

	}
}

