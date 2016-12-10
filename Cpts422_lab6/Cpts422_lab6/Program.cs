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
			WebServer.Start(5050, 10);
		}
	}
}
