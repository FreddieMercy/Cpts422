using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

/* Junhao Zhang "Freddie"
 * ID# : 11356533
 * CS 422 - Fall 2016
 * Assignment 7
 */

namespace CS422
{
	public class requestTimer
	{
		//The	second	is	a	timeout	for	the	request	to	send	up	to	the	double	line	break,	which	is	the	start	of	the	body	and	therefore	
		//the	point	at	which	the	server	finds	a	handler	for	the	request.	If	the	double	break	has	not	been	received	after	10	seconds	total,	
		//then	terminate	the	request	by	closing	the	socket	connection.	

		private Stopwatch _Stopwatch;
		public long _timeout { get; set; }
		private Thread _th;
		private TcpClient _client;

		public requestTimer (long timeout, TcpClient client = null)
		{
			_Stopwatch = new Stopwatch ();
			_timeout = timeout;
			_client = client;
			_th = new Thread (timing);
			_th.Start ();
		}

		public void Start()
		{
			_Stopwatch.Reset ();
			_Stopwatch.Start ();
		}

		private void timing()
		{
			while (true) {

				if (_Stopwatch.ElapsedMilliseconds >= _timeout) {
					if (_client == null) {
						throw new TimeoutException ("Timeout!! Duration : " + _Stopwatch.ElapsedMilliseconds);
					} else {
						_client.Close ();
						Console.WriteLine ("Timeout!! Duration : " + _Stopwatch.ElapsedMilliseconds);
					}
				}

			}
		}


		public void Stop()
		{
			_Stopwatch.Stop ();
			Console.WriteLine ("Timer stopped. Duration : " + _Stopwatch.ElapsedMilliseconds);
			_Stopwatch.Reset ();
		}
	}
}

