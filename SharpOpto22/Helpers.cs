using System;
using System.Net.Sockets;

namespace SharpOpto22
{
	static class Assert
	{
		public static void Equal(int a, int b, string format)
		{
			if (a != b)
				throw new Exception(string.Format(format, a, b));
		}
		
		public static void Equal(ushort a, ushort b, string format)
		{
			if (a != b)
				throw new Exception(string.Format(format, a, b));
		}
		
		public static void Equal(byte a, byte b, string format)
		{
			if (a != b)
				throw new Exception(string.Format(format, a, b));
		}
	}
	
	static class Disposer
	{
		//SerialPort, Socket, TcpClient, Streams, Writers, Readers, ...
		public static void Dispose(IDisposable closeable)
		{
			try {
				if (closeable != null)
					closeable.Dispose();
			} catch (Exception) {
			}
		}

		public static void Close(TcpListener closeable)
		{
			try {
				if (closeable != null)
					closeable.Stop();
			} catch (Exception) {
			}
		}

	}
	
	static class Thrower
	{
		public static void Throw(string format, params object[] args)
		{
			var message = format;
			if (args.Length > 0) {
				message = string.Format(format, args);
			}
			throw new Exception(message);
		}
		
		public static void Throw(Exception inner, string format, params object[] args)
		{
			var message = format;
			if (args.Length > 0) {
				message = string.Format(format, args);
			}
			throw new Exception(message, inner);
		}
	}
}
