using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace CS422
{
	public  abstract class WebService
	{
		/*
		Implement an abstract base class for a web “service”. This is essentially a web app that runs on
		top of the server core. When the server gets a new connection, it processes it on a thread pool thread,
		builds a request object, finds a service/handler for this request, and then sends the request to the
		handler to process.
		*/

		public WebService ()
		{
		}

		public abstract void Handler (WebRequest req);
			 
		public abstract string ServiceURI
		{
			get;
		}
	
	}
}

