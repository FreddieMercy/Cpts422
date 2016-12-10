using System;
using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace CS422
{
	public class BigNum
	{
		private int _decim;
		private BigInteger _digit;
		private bool _isNeg;
		//private BigInteger _sciNo;

		public bool IsUndefined { get; private set; }

		public BigNum(string number)
		{
			//validation
			if(validateStr(number))
			{
				throw new ArgumentException ("input number is not valid!!");
			}

			//If the string is a valid number, this constructor will initialize the BigNum to an exactly equal
			//value. There must not be any rounding or truncation.

			cleanNum(number);
			IsUndefined = false;
		}

		public BigNum(double value, bool useDoubleToString)
		{
			if(value.ToString() == double.NaN.ToString() | value == double.PositiveInfinity | value == double.NegativeInfinity)
			{
				IsUndefined = true;
			}
			else{

				IsUndefined = false;
				if(useDoubleToString)
				{
					cleanNum (value.ToString ());
				}
				else
				{
					//Temp, need to change
					doubleNum (value);
				}
			}
		}

		public override string ToString()
		{
			if (IsUndefined) {
				return "undefined";
			}
			if (_digit == 0) {
				return "0";
			} else if (_decim == 0) {

				return "."+_digit.ToString ().TrimEnd('0');

			} 
			else {
				string s = _digit.ToString ();
				string s1 = s.Substring (0, _decim);
				string s2 = s.Substring (_decim, s.Length - _decim).TrimEnd('0');

				if (s2.Length > 0) {
					return s1.TrimStart('0') + "." + s2;
				} else {
					return s1.TrimStart('0');
				}
			}

		}

		private void doubleNum(double value)
		{
			long longValue;
			longValue = BitConverter.DoubleToInt64Bits( value );
			string result = Convert.ToString(longValue, 2);

			char[] frac = new char[52];
			char[] exp = new char[11];
			char[] s = new char[1];

			s [0] = '0';

			for (int i = 0; i < 52; i++) {
				frac[i] = '0';
			}

			for (int i = 0; i < 11; i++) {
				exp[i] = '0';
			}

			for (int i = 0; i < 52; i++) {

				if(result.Length-1-i >= 0)
				{
					frac[51-i] = result[result.Length-1-i];
				}
				else
				{
					throw new ArgumentException ("input value may not be double");
				}
			}


			for (int i = 0; i < 11; i++) {

				if(result.Length-1-i-51-1 >= 0)
				{
					exp[10-i] = result[result.Length-1-i-51-1];
				}
				else
				{
					break;
				}
			}

			string fraction = new string (frac);
			string exponent = new string (exp);

			if(result.Length==64)
			{
				s[0] = result [0];
			}

			string sign = new string (s);


			if (sign == "1") {
				_isNeg = true;
			} else {
				_isNeg = false;
			}

			string n = exponent.TrimStart ('0');
			string d = fraction.TrimEnd ('0');

			string num = "1";
			BigInteger base2 = 1;

			for (int i = 0; i < d.Length; i++) {
				base2*=10;
				num += "0";
			}

			//BigInteger up = (BigInteger)Convert.ToUInt64 (n, 2);
			BigInteger up = (BigInteger)Math.Pow(2, Convert.ToUInt64(n, 2)-1023);
			BigInteger down = (BigInteger)Convert.ToUInt64 (d, 2);
			BigInteger base1 = (BigInteger)Convert.ToUInt64 (num, 2);
			//BigInteger base2 = (BigInteger)Convert.ToUInt64 (num);
			//up -= down * 1023;
			//BigInteger tmp1 = up / down;//4
			//BigInteger tmp = (BigInteger)Math.Pow (2, (double)tmp1); //16
			//tmp = up - tmp;
			//up*=base2;
			//down*=base2;
			//down /= base1;


			//BigInteger sci = 100000000000;

			/*
			if(value.ToString().Contains("E"))
			{
				string sub = value.ToString ().Substring (value.ToString ().IndexOf("E")+2, value.ToString ().Length - value.ToString ().IndexOf("E")-2);
				sci = Convert.ToUInt64 (sub) + 1;
				BigInteger len = (BigInteger)Math.Truncate (value);
				sci -= len.ToString ().Length;
			}

			else
			{
				sci = value.ToString ().Length - value.ToString ().IndexOf(".");
			}
			*/
				
			up *= base2;
			down *= up;

			down = down/base1;

			up += down;
			/*
			up /= down;//40625
			up -= tmp1;//625
			up=tmp*(BigInteger)Convert.ToUInt64 (num) + tmp*up;
*/

			//Console.WriteLine (up);
			_digit = up;
			BigInteger left = (BigInteger)Math.Truncate(value);
			_decim = left.ToString ().Length;

			if(_isNeg)
			{
				_decim--;
			}
		}


		private void cleanNum(string num)
		{
			string cleanNum = null;

			if(!num.Contains("."))
			{
				num += ".";
			}

			if (num [0] == '-') {

				_isNeg = true;
				cleanNum = num.Substring (1, num.Length - 1).Trim ('0');

			} 

			else if(num[0] == '.')
			{
				_digit = 0;
				_decim = 0;
				_isNeg = false;
				cleanNum = num;

			}else {

				_isNeg = false;

				if (num == "0") {
					cleanNum = "0";
				} else {
					cleanNum = num.Trim ('0');
				}
			}

			int index = cleanNum.IndexOf (".");

			string s1;
			string s2;

			if (index > 0) {
				s1 = cleanNum.Substring (0, index); //decimal
				s2 = cleanNum.Substring (index + 1, cleanNum.Length - index - 1); //digit

					_decim = s1.Length;

				_digit = BigInteger.Parse (s1 + s2);

				if (_isNeg) {

					_digit = 0 - _digit;
					_decim++;
				}

			} else if (index == 0) {
				
				_decim = 0;
				if (num.Length == 1) {
					_digit = 0;
				} else {
					s2 = cleanNum.Substring (index + 1, cleanNum.Length - index - 1); //digit
					//_decim = s2.Length;

					if (s2.Length == 0) {
						_digit = 0;
					} else {
						_digit = BigInteger.Parse (s2);

					}
					if (_isNeg) {

						_digit = 0 - _digit;
						_decim++;
					}
				}

			} else {
				_decim = cleanNum.Length;

				_digit = BigInteger.Parse (cleanNum);

				if (_isNeg) {

					_digit = 0 - _digit;
					_decim++;
				}
			}

			/*
			Console.WriteLine ("cleanNum = "+cleanNum);
			Console.WriteLine ("index = " + index);
			Console.WriteLine ("s1 = " + s1);
			Console.WriteLine ("s2 = " + s2);
			Console.WriteLine ("_decim = " + _decim);
			Console.WriteLine ("_digit = " + _digit);
*/
		}

		private bool validateStr(string number)
		{
			bool error = false;
			/*
			 * Validate the string the string and throw an exception if it does satisfy the criteria below
			 * */


			// Null or empty strings are not allowed
			if (string.IsNullOrEmpty (number) || string.IsNullOrWhiteSpace (number)) {

				error = true;
			} 

			// The string must start with a minus sign, numeric character, or decimal point
			else if (number [0] != '-' && number [0] != '.' && !Char.IsNumber (number [0])) 
			{
				error = true;
			} else {

				int i = 0;
				int j = 0;
				int z = 0;

				foreach (char x in number) {

					// No whitespace is allowed
					if (x == ' ') {
						error = true;
						break;
					}

					//* ‘-‘ (at most 1 and only ever valid as the very first character)
					else if (x == '-') {
						j++;
						if (j > 1 | z > 0) {
							error = true;
							break;
						}
					}
					//* ‘.’ (at most 1)
					else if (x == '.') {
						//
						i++;
						if (i > 1) {
							error = true;
							break;
						}
					}
					//* [0-9]
					else if (!char.IsNumber (x)) {
						error = true;
						break;
					}
				}

				z++;
			}


			return error;

		}

		public static BigNum operator+(BigNum lhs, BigNum rhs)
		{
			if(lhs.IsUndefined | rhs.IsUndefined )
			{
				return new BigNum (double.PositiveInfinity, true);
			}
			
			int l = lhs.ToString ().Length - lhs._decim;
			if(l == 0)
			{
				l++;
			}

			int r = rhs.ToString ().Length - rhs._decim;

			if(r == 0)
			{
				r++;
			}

			if (l > r) {
				return addHelper (lhs, rhs);

			} else {

				return addHelper (rhs, lhs);

			}
		}

		public static BigNum addHelper(BigNum lhs, BigNum rhs)
		{

			int len = 0;
			int cout = 0;

			int l = lhs.ToString ().Length - lhs._decim;
			if(l == 0)
			{
				l++;
			}

			int r = rhs.ToString ().Length - rhs._decim;

			if(r == 0)
			{
				r++;
			}

			len = l - r;
			/* sync */
			for (int i = 0; i < len; i++) {
				rhs._digit *= 10;
			}

			BigInteger sum = lhs._digit + rhs._digit;


			/* back */
			for (int i = 0; i < len; i++) {
				rhs._digit /= 10;
			}

			BigNum num = new BigNum (sum.ToString ());

			string one = lhs.ToString ().Substring (0, lhs._decim);
			if (one == "-" | string.IsNullOrEmpty(one) | string.IsNullOrWhiteSpace(one)) {
				one = "0";
			}

			string two = rhs.ToString ().Substring (0, rhs._decim);
			if (two == "-" | string.IsNullOrEmpty(two) | string.IsNullOrWhiteSpace(two)) {
				two = "0";
			}

			BigInteger a = (BigInteger)Convert.ToInt64(one);
			BigInteger b = (BigInteger)Convert.ToInt64(two);

			BigInteger c = a + b;
			int d = 0;

			if(a.ToString().Length > b.ToString().Length)
			{
				d = a.ToString ().Length;
			}
			else
			{
				d = b.ToString ().Length;
			}

			cout = c.ToString().Length - d;


			if(lhs._decim > rhs._decim)
			{
				num._decim = lhs._decim + cout;
			}

			else
			{
				num._decim = rhs._decim + cout;
			}

			return num;
		}


		public static BigNum operator-(BigNum lhs, BigNum rhs)
		{
			if(lhs.IsUndefined | rhs.IsUndefined )
			{
				return new BigNum (double.PositiveInfinity, true);
			}

			BigNum zero;

			if(rhs._isNeg)
			{
				zero = new BigNum (rhs.ToString().Substring(1, rhs.ToString().Length-1));
			}

			else
			{
				zero = new BigNum ("-"+rhs.ToString ());
			}

			return lhs + zero;
		}

		public static BigNum operator*(BigNum lhs, BigNum rhs)
		{
			if(lhs.IsUndefined | rhs.IsUndefined )
			{
				return new BigNum (double.PositiveInfinity, true);
			}

			int l = lhs.ToString ().Length - lhs._decim;
			int r = rhs.ToString ().Length - rhs._decim;

			if(lhs.ToString().Contains("."))
			{
				l--;
			}

			if(rhs.ToString().Contains("."))
			{
				r--;
			}

			BigInteger t = lhs._digit * rhs._digit;

			BigNum tmp = new BigNum (t.ToString());
			tmp._decim = tmp.ToString().Length - l - r;

			return tmp;
		}

		public static BigNum operator/(BigNum lhs, BigNum rhs)
		{
			if(lhs.IsUndefined | rhs.IsUndefined | rhs.ToString()=="0")
			{
				return new BigNum (double.PositiveInfinity, true);
			}

			BigInteger n = lhs._digit;
			BigInteger d = rhs._digit;
			int counter = 0;

			for (int i = 0; i < 20; i++)
			{
				if ((n%d).IsZero)
				{
					break;
				}

				n*= 10;
				counter++;
			}

			BigInteger result = n / d;

			BigNum tmp = new BigNum (result.ToString ());

			int l = lhs.ToString ().Length - lhs._decim;
			int r = rhs.ToString ().Length - rhs._decim;

			if(lhs.ToString().Contains("."))
			{
				l--;
			}

			if(rhs.ToString().Contains("."))
			{
				r--;
			}

			tmp._decim = tmp.ToString ().Length - counter - l + r;

			return tmp;
		}

		public static bool operator < (BigNum lhs, BigNum rhs)
		{
			if (lhs.IsUndefined || rhs.IsUndefined)
			{
				return false;
			}

			BigNum result = lhs - rhs;
			if (result.ToString()=="0")
			{
				return false;
			}

			return result._isNeg;
		}

		public static bool operator > (BigNum lhs, BigNum rhs)
		{

			if (lhs.IsUndefined || rhs.IsUndefined)
			{
				return false;
			}

			BigNum result = lhs - rhs;
			if (result.ToString()=="0")
			{
				return false;
			}

			return !result._isNeg;
		}

		public static bool operator <= (BigNum lhs, BigNum rhs)
		{
			if (lhs.IsUndefined && rhs.IsUndefined)
			{
				return true;
			}

			if ((lhs.IsUndefined && !rhs.IsUndefined) || (!lhs.IsUndefined && rhs.IsUndefined))
			{
				return false;
			}

			BigNum result = lhs - rhs;
			if (result.ToString()=="0")
			{
				return true;
			}

			return result._isNeg;
		}

		public static bool operator >= (BigNum lhs, BigNum rhs)
		{
			if (lhs.IsUndefined && rhs.IsUndefined)
			{
				return true;
			}

			if ((lhs.IsUndefined && !rhs.IsUndefined) || (!lhs.IsUndefined && rhs.IsUndefined))
			{
				return false;
			}

			BigNum result = lhs - rhs;
			if (result.ToString()=="0")
			{
				return true;
			}

			return !result._isNeg;
		}


	}
}


