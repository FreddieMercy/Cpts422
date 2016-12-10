using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 7
 */

namespace CS422
{
	public class validationMethodsandFields
	{
		public string method { get; private set;}
		public ConcurrentDictionary<string,string> headers { get; private set;}
		public string version { get; private set;}
		public string uri{ get; private set;}
		public string request{ get; private set;}
		public bool end { get; private set;}
		public long length { get; private set;}
		public bool success { get; private set; }
		private string req;
		private const int sizePerLine = 2048;
		private const int sizeTotal = 100 * 2048;

		public validationMethodsandFields ()
		{
			req = "";
			method = "";
			version = "";
			uri = null;
			request = "";
			end = false;
			length = -1;
			headers = new ConcurrentDictionary <string, string> ();
			success = false;
		}

		public void Clear()
		{
			req = "";
			method = "";
			version = "";
			uri = null;
			request = "";
			end = false;
			length = -1;
			headers = new ConcurrentDictionary <string, string> ();
			success = false;
		}

		public void parseReqFromString(TcpClient client)
		{
			NetworkStream net = client.GetStream ();
			net.ReadTimeout = 1580;//	between	1	and	2	seconds.
			StreamReader sr = new StreamReader (net);
			//The	first	is	a	timeout	for	an	individual	read	on	the	network	stream.	Set	this	to	some	value	between	1	and	2	seconds.
			requestTimer timer = new requestTimer (10000, client); //by default the time limite is 10 sec
			string s = "";
			char c;

			timer.Start ();

			try
			{
				while (sr.Peek () >= 0) {
					c = (char)sr.Read ();

					if(s == "PUT /files/123/BigNum.cs HTTP/1.1\r\nHost: localhost:5050\r\nConnection: keep-alive\r\nContent-Length: 11720\r\nOrigin: http://localhost:5050\r\nUser-Agent: Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.100 Safari/537.36\r\nContent-Type: text/x-csharp\r\nAccept: */*\r\nReferer: http://localhost:5050/files/123\r\nAccept-Encoding: gzip, deflate, sdch, br\r\nAccept-Language: en-US,en;q=0.8,zh-CN;q=0.6,zh;q=0.4,zh-TW;q=0.2,ja;q=0.2\r\nCookie: csrftoken=lIqpZViTmmo521Miw2lzHrtDXTlnulSM")
					{
						Console.WriteLine("Fuck");
					}

					//requestValidation (c)==true means is valid
					//The	second	threshold	is	for	the	content	up	to	the	body.	If	you	have	read	(100	*	1024)	bytes	or	more	and	have	not	received	
					//the	double	line	break,	then	terminate	the	request.	Put	both	of	these	thresholds	in	const	member	variables	in	the	class	
					//so	that	they	could	be	easily	changed	to	new	values	in	a	future	version.

					if (requestValidation (c) && s.Length < sizeTotal) {
						s += c;

						if (end) {
							break;
						}

						if(s.Length >= 4)
						{
							if(s.Substring(s.Length-4, 4) == "\r\n\r\n")
							{
								break;
							}
						}
					} 
					//requestValidation (c) == false means not valid, then terminate
					else {
						s = null;
						break;
					}

				}
			}
			catch(IOException) {
				parseReqFromString(client);
				return;
			}


			timer.Stop ();

			/*If it’s NOT a valid HTTP request, you will sever the socket connection and return false from the function.*/
			//if (request_was_valid) 

			if (!String.IsNullOrEmpty (s)) {
				Console.WriteLine (s);
				success = true;
				request = s;
			} 
		}

		public bool requestValidation(char c)
		{
			if (c == '\0') {

				Console.WriteLine ("c = '\0'");

				return false;

			} else if (c != '\n') {

				req += c;

			} else {

				req += c;

				if (req.Length <= 1) {
					return false;

				} else if (req [req.Length - 2] != '\r') {
					return false;
				}

				string[] cmd = req.Substring (0, req.Length - 2).Split (':');


				if (cmd.Length > 3) { 

					if(cmd[0] == "Origin")
					{
						return true;
					}
					else
					{
						return false;
					}
				} else if (cmd.Length == 3) { //Host: url: port#
					int n;

					if (cmd [0] == "Host" && Int32.TryParse (cmd [2], out n) && !String.IsNullOrEmpty (cmd [1]) && !String.IsNullOrWhiteSpace (cmd [1])) {
						req = "";
						string host = cmd [1] + ":" + cmd [2];
						headers.TryAdd ("Host", host);

						//The	first	is	a	threshold	just	to	reach	the	first	line	break.	If	you	have	read	2048	or	more	bytes	from	the	socket	and	not	
						//received	the	first	CRLF	line	break,	then	terminate	the	request.
						if(req.Length < 2048)
						{
							return true;
						}

						return false;
					}

					if(cmd[0] == "Origin")
					{
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

						if (cmd [0] [i] == ' ') {

							//	cmd [0][i] = '\0';
							break;

						}

					}
					/*
					switch (cmd [0].Substring (0, i)) {
					case "Cache-Control":

						break;
					case "Connection":

						break;

					case "Origin":

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
					case "Content-Length":
						break;
					case "Expires":

						break;
					case "Last-Modified":

						break;


					default:
						return false;
					}
*/
					string sLength = null;

					headers.TryAdd (cmd [0].Substring (0, i), cmd [1]);

					if (headers.ContainsKey ("Content-Length")) {
						headers.TryRemove ("Content-Length", out sLength);
					}

					if (sLength != null) {
						length = (long)Convert.ToUInt64 (sLength.ToString());
					}
				} 

				//crsf
				else if (req == "\r\n") {
					//req = "";
					end = true;
				} else if (cmd.Length == 1) { //length == 1, GET PATH HTTP
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
					uri = tmp [1];
					version = tmp [2];

				} 

				req = "";
			}

			//The	first	is	a	threshold	just	to	reach	the	first	line	break.	If	you	have	read	2048	or	more	bytes	from	the	socket	and	not	
			//received	the	first	CRLF	line	break,	then	terminate	the	request.
			if (req.Length < 2048) {
				return true;
			}

			return false;
		}

	}
}

