using System;

namespace SharpOpto22
{
	public interface IMmStream: IDisposable
	{
		void Write(byte[] data);
		void Read(byte[] data);
	}
}

