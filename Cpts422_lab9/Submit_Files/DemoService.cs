using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 7
 */

namespace CS422
{
	public class DemoService : WebService
	{
		private const string c_template =
			"<html>This is the response to the request:<br>" +
			"Method: {0}<br>Request-Target/URI: {1}<br>" +
			"Request body size, in bytes: {2}<br><br>" +
			"Student ID: {3}</html>";
		
		public DemoService ()
		{
		}

		public override void Handler (WebRequest req)
		{
			req.WriteHTMLResponse(c_template);
		}
		public override string ServiceURI 
		{ 
			get { 
				return "/"; 
			} 
		}

	}
}

