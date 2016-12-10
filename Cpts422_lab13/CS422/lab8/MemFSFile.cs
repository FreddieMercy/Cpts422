using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace CS422
{
	public class MemFSFile : File422
	{
		public override string Name { get; }
		public override Dir422 Parent { get; }

		private object _lock;
		private string _status;
		private int _readOnlyTimes;

		public MemFSFile(string name, Dir422 parent)
		{
			if (string.IsNullOrEmpty(name) | string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("string.IsNullOrEmpty(name) | string.IsNullOrWhiteSpace(name)");
			}

			Name = name;
			Parent = parent;
			_lock = new object ();
			_status = null;
			_readOnlyTimes = 0;
		}

		public override Stream OpenReadOnly ()
		{
			lock (_lock) {
				if (_status == null | _status == "R") {
					_status = "R";
					ThisStream s = new ThisStream ();
					s.Disposed += waitForRWtoDispose;
					_readOnlyTimes++;
					return s;
				}

				return null;
			}
		}

		public override Stream OpenReadWrite ()
		{
			lock (_lock) {
				if (_status == null) {
					_status = "RW";
					ThisStream s = new ThisStream ();
					s.Disposed += waitForRWtoDispose;
					return s;
				}
				return null;
			}
		}

		private void waitForRWtoDispose(object sender, EventArgs e)
		{
			lock (_lock) {
				if (_readOnlyTimes == 0) {
					_status = null;
				} else if (_status == "R") {
					_readOnlyTimes--;
					if (_readOnlyTimes == 0) {

						_status = null;
					}
				}
			}
		}
	}

}
	


internal class ThisStream : MemoryStream
{
	public delegate void DisposedEventHandler(object sender, EventArgs e);
	public event DisposedEventHandler Disposed;

	public ThisStream()
	{

	}

	~ThisStream()
	{
		Dispose(false);
	}

	protected override void Dispose(bool disposing)
	{
		if (Disposed != null)
		{
			Disposed(new object(), new EventArgs());
		}

		base.Dispose(disposing);

	}

}
	