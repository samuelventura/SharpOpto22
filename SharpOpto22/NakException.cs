using System;

namespace SharpOpto22
{
	public class NakException : Exception
	{
		private readonly byte resultCode;
		
		public NakException(byte resultCode)
			: base(string.Format("Result code {0:X}h -> NAK", resultCode))
		{
			this.resultCode = resultCode;
		}
		
		public byte ResultCode {
			get { return resultCode; }
		}
	}
}

