using System;

namespace SharpOpto22
{
	public interface IMmProtocol: IDisposable
	{
		void WriteQuadlet(long destinationOffset, byte[] data);
	
		byte[] ReadQuadlet(long destinationOffset);
	
		byte[] ReadBlock(long destinationOffset, int length);
	
		void WriteBlock(long destinationOffset, byte[] data);
	}
}
