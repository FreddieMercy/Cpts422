using CS422;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cpts422_lab3
{
	class MainClass
	{
		
		const string formatter = "{0,25:E16}{1,23:X16}";

		// Reinterpret the double argument as a long.
		public static void DoubleToLongBits( double argument )
		{
			long longValue;
			longValue = BitConverter.DoubleToInt64Bits( argument );

			// Display the resulting long in hexadecimal.
			Console.WriteLine( formatter, argument, longValue );
			//Console.WriteLine(longValue);
		}

		public static void Main( )
		{
			/*
			Console.WriteLine( 
				"This example of the BitConverter.DoubleToInt64Bits( " +
				"double ) \nmethod generates the following output.\n" );
			Console.WriteLine( formatter, "double argument", 
				"hexadecimal value" );
			Console.WriteLine( formatter, "---------------", 
				"-----------------" );

			// Convert double values and display the results.
			DoubleToLongBits( 1.0 );
			DoubleToLongBits( 15.0 );
			DoubleToLongBits( 255.0 );
			DoubleToLongBits( 4294967295.0 );
			DoubleToLongBits( 0.00390625 );
			DoubleToLongBits( 0.00000000023283064365386962890625 );
			DoubleToLongBits( 1.234567890123E-300 );
			DoubleToLongBits( 1.23456789012345E-150 );
			DoubleToLongBits( 1.2345678901234565 );
			DoubleToLongBits( 1.2345678901234567 );
			DoubleToLongBits( 1.2345678901234569 );
			DoubleToLongBits( 1.23456789012345678E+150 );
			DoubleToLongBits( 1.234567890123456789E+300 );
			DoubleToLongBits( double.MinValue );
			DoubleToLongBits( double.MaxValue );
			DoubleToLongBits( double.Epsilon );
			DoubleToLongBits( double.NaN );
			DoubleToLongBits( double.NegativeInfinity );
			DoubleToLongBits( double.PositiveInfinity );

			
			*/

			double a = 5684.6518482622;

			Console.WriteLine (a.ToString());
			BigNum x = new BigNum ("4");
			Console.WriteLine (x.ToString ());

			BigNum y = new BigNum ("-3");
			Console.WriteLine (y.ToString ());
		
			Console.WriteLine (x / y);
		
			Console.WriteLine (double.NaN.ToString ());
			//BigNum d = new BigNum (double.PositiveInfinity, true);
			//Console.WriteLine (d.ToString());
			//BigNum z = new BigNum ("");
		}
	}
}
