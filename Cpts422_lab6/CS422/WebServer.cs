
/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 6
 */

using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace CS422
{

	public class WebServer
	{
		private static readonly BlockingCollection<TcpClient> _collection = new BlockingCollection<TcpClient>();
		private static readonly List<Thread> _threadPool = new List<Thread>();
		private static readonly HashSet<WebService> _webServices = new HashSet<WebService> {new DemoService()};
		private static TcpListener _tcpListener;
		private static Thread _listenerThread = null;
		private static int _numOfThread;
		/*
If the “Content-Length” header is present in the request, then the body stream will support
querying the length. Otherwise querying the length will throw an exception. You will have implemented
the ConcatStream class such that you can pass the parsed content length value to its constructor in
instances when the Content-Length header is present. The ConcatStream will also support construction
from 2 streams with no further information, so you will use that constructor if the content length header
is not present.
		*/
		public static void Start(int port, int numOfThread)
		{
			if (port < 0) {

				throw new ArgumentException("port < 0");
			}

			if (_listenerThread != null) {
				throw new Exception ("Server is running already, listening failed!");
			}

			_numOfThread = numOfThread < 0 ? 64 : numOfThread;

			_listenerThread = new Thread (() => {
				bool active = true;

				_tcpListener = new TcpListener (System.Net.IPAddress.Any, port);

				try {
					_tcpListener.Start ();
				} catch (Exception) {
					Console.WriteLine ("Access Denied, has no permission");

					return;
				}

				_tcpListener.Start ();

				while (active) {

					try {
						_collection.Add (_tcpListener.AcceptTcpClient ());
					} catch (SocketException) {
						break;
					}
				}
			});

			_listenerThread.Start ();

			for (int i = 0; i < _numOfThread; i++) {
				Thread t = new Thread (ThreadWork);
				t.Start ();
				_threadPool.Add (t);
			}
		}

		private static WebRequest BuildRequest(TcpClient client)
		{
			WebRequest request = new WebRequest ();

			request.clientNetworkStream = client.GetStream ();
			if (request.Success) {
				return request;
			}
			client.Close ();
			return null;
		}

		private static void ThreadWork()
		{
			bool active = true;
			while (active) {
				TcpClient client = _collection.Take ();
				if (client == null) {
					break;
				}

				WebRequest request = BuildRequest (client);

				if (request == null) {
					client.Close ();
				} else {
					WebService handler = FindHandlerFor (request);
					if (handler != null) {
						handler.Handler (request);
						//return;
					} else {
						request.WriteNotFoundResponse ("");
					}

				}
			}

		}

		public static void AddService(WebService service)
		{
			//if the request-target/URI starts with the string specified by the WebService object’s
			//ServiceURI parameter, then it can process that request.

			_webServices.Add (service);
		}


		public static void Stop()
		{
			int count = _collection.Count;

			_tcpListener.Stop ();
			_listenerThread.Join ();

			for (int i = 0; i < count; i++) {
				TcpClient tmp = _collection.Take ();
				tmp.Close ();
				tmp = null;
				_collection.Add (tmp);
			}

			foreach (Thread th in _threadPool) {
				th.Join ();
			}
		}

		private static WebService FindHandlerFor(WebRequest req)
		{
			foreach (WebService ser in _webServices) {
				if (req != null) {
					if (req.RequestTarget_URI.Length >= ser.ServiceURI.Length) {
						if (string.Compare (req.RequestTarget_URI.Substring (0, ser.ServiceURI.Length), ser.ServiceURI, StringComparison.OrdinalIgnoreCase) == 0) {
							return ser;
						}
					} 
				}

			}

			return null;
		}

	}
}