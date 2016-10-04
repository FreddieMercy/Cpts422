using System;

namespace CS422
{
	public class Node 
	{
		public int value { get; set;}
		public Node next { get; set;}

		public Node()
		{
			value = 0;
			next = null;
		}

	}

	public class PCQueue
	{
		private Node Front;
		private Node End;

		public PCQueue ()
		{
			Node tmp = new Node ();
			End = tmp;
			Front = tmp;
		}

		public bool Dequeue(ref int out_value)
		{
			//If queue empty

			if(Front == End)
			{
				Console.Write ("* Queue Empty, Dequeue failled!!");
				return false;
			}

			out_value = Front.value;
			Front = Front.next;


			//ensure garbage collecter recycles the poped Node, but slows "Dequeue" significantly
			Node tmp = Front;
			tmp = null;

			GC.Collect ();

			return true;

		}

		public void Enqueue(int dataValue)
		{
			if(End.next == null)
			{
				End.next = new Node ();
				End.value = dataValue;
				End = End.next;

				return;
			}

			Console.WriteLine ("Error: End is not the last Node nor the First Node");

		}

		/*
		public void printOnce()
		{
			while(Front != End)
			{
				Console.WriteLine (Front.value);
				Front = Front.next;
			}
		}
		*/
	}
}

