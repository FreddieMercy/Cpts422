using System;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace CS422
{
	public class ThreadPoolSleepSorter : IDisposable
	{
		private TextWriter _writer;
		private volatile ushort _count;
		private BlockingCollection<ThreadItems> core = new BlockingCollection<ThreadItems> ();
		private bool _isDead;
		private Object thisLock = new Object();

		public void Dispose() {

			Console.WriteLine ("Disposed");

			if (_isDead) 
			{
				throw new ObjectDisposedException ("The ThreadPoolSleepSorter reference had been disposed");  
			}

			_isDead = true;

			core.Dispose ();

			return;
		}

		public ThreadPoolSleepSorter (TextWriter output, ushort threadCount = 0)
		{
			_isDead = false;

			_count = threadCount;

			if (_count == 0) {

				_count = 64;

			}
			

			_writer = output;

			for (int i = 0; i < _count; i++) {

				var t = new Thread (ThreadWorkFunc);
				ThreadItems tmp = new ThreadItems ();

				t.Start (tmp);

				core.Add (tmp);
			}
			
		}

		private void ThreadWorkFunc(object x) 
		{
			ThreadItems X = (ThreadItems)x;

			while (true) {

				if(_isDead)
				{
					return;
				}

				if (X.Execute ()) {

					if(_isDead)
					{
						throw new ObjectDisposedException ("the ThreadPoolSleepSorter object had been disposed");
					}
					else{
						try
						{
							lock(thisLock)
							{
								_writer.WriteLine(X.getValue);
							}

						}
						catch(ObjectDisposedException){

							throw new ObjectDisposedException ("The wrapped TextWriter reference had been disposed");

						}
					}

					_count++;

				}

			}

		}


		public void Sort(byte[] values)
		{
			foreach(byte X in values)
			{
				ThreadItems th = core.Take ();
				th.getValue = X;
				core.Add (th);
				_count--;
			}

			while(_count!=core.Count);
				
		}

	}

	internal class ThreadItems
	{
		private byte Byte;
		private bool doit = false;
		private bool hasOne = false;

		public byte getValue
		{
			get{
				if(!hasOne)
				{
					throw new NullReferenceException ("Byte undefined!!");
				}

				return Byte;
			}

			set{
				hasOne = true;
				doit = true;
				Byte = value;
			}

		}

		public bool Execute()
		{
	
			if (doit) {

				doit = false;
				Thread.Sleep (Byte * 1000);
				return true;
			}

			return false;
		}

	}
}

