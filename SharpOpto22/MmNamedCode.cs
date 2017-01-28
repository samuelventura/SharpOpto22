using System;

namespace SharpOpto22
{
	public class MmNamedCode
	{
		private readonly int code;
		private readonly string name;
		
		public MmNamedCode(int code, string name)
		{
			this.code = code;
			this.name = name;
		}
		
		public int Code {
			get { return code; }
		}
	
		public string Name {
			get { return name; }
		}
		
		override public string ToString()
		{
			return string.Format("{0:X}h - {1}", code, name);
		}
	}
}

