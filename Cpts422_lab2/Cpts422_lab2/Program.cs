using System;
using CS422;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;

namespace Cpts422_lab2
{
	class MainClass
	{
		static PCQueue queue = new PCQueue ();
		static Random rnd = new Random();

		public static void Main (string[] args)
		{
			/*
			var c = new Thread (Consumer);
			//var t = new Thread (Consumer);
			var p = new Thread (Producer);

			Thread.Sleep (5000);
			//t.Start ();
			c.Start ();


			Thread.Sleep(2000);
			p.Start ();

			Thread.Sleep(500);

			p.Abort ();
			c.Abort ();
			*/

			byte[] num= new byte[]{7, 1, 4, 2, 7, 3, 9};
			byte[] nums= new byte[]{1,1,1,1,1,1,1,1};
			StringWriter str = new StringWriter ();

			ThreadPoolSleepSorter x = new ThreadPoolSleepSorter (str, 8);

			x.Sort (num);
			x.Sort (nums);

			Console.WriteLine ("********************************************");
			Console.Write (str.ToString ());

			x.Dispose ();
		}

		static void Producer()
		{
			while (true) {

				int i = rnd.Next(0, 1000);

				queue.Enqueue (i);

				Console.WriteLine ("Enqueue : "+i);

			}
		}

		static void Consumer()
		{
			while (true) {

				int i = 0;
				//Console.Write ("* i = " + i);

				if (queue.Dequeue (ref i)) {
					Console.Write ("* Dequeue : " + i + "\n");
				}
			}
		}
	}
}
