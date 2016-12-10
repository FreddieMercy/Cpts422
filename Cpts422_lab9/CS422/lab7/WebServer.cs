
/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 7
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
		private static readonly HashSet<WebService> _webServices = new HashSet<WebService> {};
		private static TcpListener _tcpListener;
		private static Thread _listenerThread = null;
		private static int _numOfThread;
		private static object _lockis = new object();

		public static void Start(int port, int numOfThread)
		{
			if (port < 0) {

				throw new ArgumentException ("port < 0");
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

			request.Client = client;
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

				if (request != null) {

					WebService handler = FindHandlerFor (request);
					if (handler != null) {
						handler.Handler (request);
						//return;
					} else {
						request.WriteNotFoundResponse ("");
						//client.Close ();
					}

				}

				client.Close ();
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
					if(req.RequestTarget_URI=="/")
					{
						return new DemoService ();
					}

					else
					{

						if (req.RequestTarget_URI.Length >= ser.ServiceURI.Length) {
							if (string.Compare (req.RequestTarget_URI.Substring (0, ser.ServiceURI.Length), ser.ServiceURI, StringComparison.OrdinalIgnoreCase) == 0) {
								return ser;
							}
						} else {
							if (string.Compare (ser.ServiceURI.Substring (0, req.RequestTarget_URI.Length), req.RequestTarget_URI, StringComparison.OrdinalIgnoreCase) == 0) {
								return ser;
							}
						}
					}
				}

			}

			return null;
		}

	}
}