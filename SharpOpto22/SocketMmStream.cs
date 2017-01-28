using System;
using System.Net.Sockets;

namespace SharpOpto22
{
	public class SocketMmStream : IMmStream
	{
		private readonly TcpClient socket;
	
		public SocketMmStream(string host, int port, int timeout)
		{
			this.socket = new TcpClient();
			var result = socket.BeginConnect(host, port, null, null);
			if (!result.AsyncWaitHandle.WaitOne(timeout, true))
				Thrower.Throw("Timeout connecting to {0}:{1}", host, port);
			socket.EndConnect(result);
			socket.ReceiveTimeout = timeout;
			socket.SendTimeout = timeout;
		}
		
		public void Write(byte[] data)
		{
			socket.GetStream().Write(data, 0, data.Length);
		}
	
		public void Read(byte[] data)
		{
			var count = socket.GetStream().Read(data, 0, data.Length);
			if (count != data.Length)
				Thrower.Throw("Read count mismatch requested:{0} available:{1}", data.Length, count);
		}

		public void Dispose()
		{
			Disposer.Dispose(socket);
		}

	}
}

