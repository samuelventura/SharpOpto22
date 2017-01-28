using System;

namespace SharpOpto22
{
	/// <summary>
	/// MmProtocol implements the Memory Mapped Protocol. 
	/// Reference
	/// <list type="bullet">
	/// <item><term>Page 88 - Communication Packets</term></item>
	/// <item><term>Page 99 - Read and Write Packet Structure</term></item>
	/// </list>
	/// </summary>
	public class MmProtocol : IMmProtocol
	{
		/// <summary>
		/// Transaction codes (Page 99) define the type of packet. 
		/// This 4 bits code is send on the higher 4 bits of the 4th byte on every packet. 
		/// Code is initialized ready to be copied to 4th byte in the right position without 
		/// requiring shift operation. 
		/// </summary>
		public enum TransactionCode
		{
			WriteQuadletRequest = 0x00,
			WriteBlockRequest = 0x10,
			WriteQuadletOrBlockResponse = 0x20,
			ReadQuadletRequest = 0x40,
			ReadBlockRequest = 0x50,
			ReadQuadletResponse = 0x60,
			ReadBlockResponse = 0x70,
		}
		
		private readonly int sourceId;
		private readonly int transactionLabel;
		private readonly IMmStream mmStream;
		
		public MmProtocol(IMmStream mmStream, int sourceId = 0, int transactionLabel = 0)
		{
			this.mmStream = mmStream;
			
			// Optional parameter. If you are running two or more applications simultaneously, you can give 
			// each application a different ID in this parameter. If an error occurs, you can read the
			// Source address in the memory map Status area to find out which application caused the error.
			// Range 0x0000-0xffff (16bit)
			var si = sourceId & 0xffff;
			if (si != sourceId)
				Thrower.Throw("Source id {0:X} out of range (0x0000 to 0xFFFF)", sourceId);
			this.sourceId = sourceId; 
			
			// A label specified by the requester and identifying this transaction.
			// Range 0x00-0x3F (8bit). This value is returned in the response packet.
			var tl = (transactionLabel << 2) & 0xff;
			if (tl >> 2 != transactionLabel)
				Thrower.Throw(
					"Transaction label {0:X} out of range (0x00 to 0x3F)", transactionLabel);
			this.transactionLabel = tl;			
		}
		
		public void Dispose()
		{
			mmStream.Dispose();
		}

		public void WriteQuadlet(long destinationOffset, byte[] data)
		{
			checkDestinationOffset(destinationOffset);
			checkData(data, 4);
			var request = new byte[16];
			configRequest(request, TransactionCode.WriteQuadletRequest, destinationOffset);
			// copy data
			for (int i = 0; i < data.Length; i++) {
				request[12 + i] = data[i];
			}
			var response = new byte[12];
			mmStream.Write(request);
			checkRequest(request);
			mmStream.Read(response);
			checkResponse(response, TransactionCode.WriteQuadletOrBlockResponse);
		}
	
		public byte[] ReadQuadlet(long destinationOffset)
		{
			checkDestinationOffset(destinationOffset);
			var request = new byte[12];
			configRequest(request, TransactionCode.ReadQuadletRequest, destinationOffset);
			var response = new byte[16];
			var data = new byte[4];
			mmStream.Write(request);
			checkRequest(request);
			mmStream.Read(response);
			checkResponse(response, TransactionCode.ReadQuadletResponse);
			// copy data
			for (var i = 0; i < data.Length; i++) {
				data[i] = response[12 + i];
			}
			return data;
		}
	
		public byte[] ReadBlock(long destinationOffset, int length)
		{
			checkDestinationOffset(destinationOffset);
			checkBlockLength(length);
			var request = new byte[16];
			configRequest(request, TransactionCode.ReadBlockRequest, destinationOffset);
			// calculate quadlets
			var quadlets = (length - 1) / 4 + 1; // -1/4->0
			var paddedLength = 4 * quadlets;
			// set block length
			request[12] = (byte)((paddedLength >> 8) & 0xff);
			request[13] = (byte)(paddedLength & 0xff);
			mmStream.Write(request);
			checkRequest(request);
			var response = new byte[16 + paddedLength];
			mmStream.Read(response);
			checkResponse(response, TransactionCode.ReadBlockResponse);
			checkBlockLength(response, paddedLength);
			// copy data
			var data = new byte[length];
			for (var i = 0; i < data.Length; i++) {
				data[i] = response[16 + i];
			}
			return data;
		}

		public void WriteBlock(long destinationOffset, byte[] data)
		{
			checkDestinationOffset(destinationOffset);
			checkData(data, -1);
			checkBlockLength(data.Length);
			var quadlets = (data.Length - 1) / 4 + 1; // -1/4->0
			var request = new byte[16 + 4 * quadlets];
			configRequest(request, TransactionCode.WriteBlockRequest, destinationOffset);
			// copy data
			for (int i = 0; i < data.Length; i++) {
				request[16 + i] = data[i];
			}
			// set block length
			request[12] = (byte)((data.Length >> 8) & 0xff);
			request[13] = (byte)(data.Length & 0xff);
			mmStream.Write(request);
			checkRequest(request);
			var response = new byte[12];
			mmStream.Read(response);
			checkResponse(response, TransactionCode.WriteQuadletOrBlockResponse);
		}

		private void configRequest(byte[] request, TransactionCode transactionCode, long destinationOffset)
		{
			// bytes 0-1 reserved (destination id)
			// assumes lower 2 bits are zeroed
			request[2] = (byte)transactionLabel;
			// assumes lower 4 bits are zeroed
			request[3] = (byte)transactionCode;
			// shift source id lower 18 bits into bytes 4-5 (big-endian)
			request[4] = (byte)(sourceId >> 8 & 0xff);
			request[5] = (byte)(sourceId & 0xff);
			// shift destination offset into bytes 6-11 (big-endian)
			var dof = destinationOffset;
			for (var i = 0; i < 6; i++) {
				request[11 - i] = (byte)(dof & 0xff);
				dof >>= 8;
			}
		}

		private void checkRequest(byte[] request)
		{
		}
	
		private void checkResponse(byte[] response, TransactionCode transactionCode)
		{
			// 40h returned for required powerup clear
			// 50h returned for invalid address
			var _resultCode = response.Length >= 7 ? response[6] : (byte)0;
			if (_resultCode != 0) {
				throw new NakException(_resultCode);
			}
			
			// check source id matches
			var _sourceId = 0xffff & response[4] << 8 | response[5];
			if (_sourceId != sourceId) {
				Thrower.Throw(
					"Invalid source id, expected {0:X} but received {1:X}", 
					sourceId, _sourceId);
			}
	
			// check transaction label matches
			var _transactionLabel = response[2] & 0xff;
			if (_transactionLabel != transactionLabel) {
				Thrower.Throw(
					"Invalid transaction label, expected {0:X} but received {1:X}", 
					transactionLabel, _transactionLabel);
			}
	
			// check transaction code matches
			var _transactionCode = response[3] & 0xff;
			if (!Enum.IsDefined(typeof(TransactionCode), _transactionCode)
			    || (TransactionCode)_transactionCode != transactionCode) {
				Thrower.Throw(
					"Invalid transaction code, expected {0:X} but received {1:X}", 
					transactionCode, _transactionCode);
			}
		}

		private void checkDestinationOffset(long destinationOffset)
		{
			var _destinationOffset = (destinationOffset & 0xffffffffffffL);
			if (destinationOffset != _destinationOffset) {
				Thrower.Throw(
					"Invalid destinationOffset {0:X}, range is 0x000000000000 to 0xffffffffffff",
					destinationOffset);
			}
		}

		private void checkBlockLength(int length)
		{
			if (length != (length & 0xffff)) { 
				Thrower.Throw(
					"Invalid length {0:X}, range is 0x0000 to 0xffff", 
					length);
			}
		}
	
		private void checkData(byte[] data, int length)
		{
			if (data == null) {
				Thrower.Throw("data is null");
			}
			if (length >= 0 && data.Length != length) { // do not test if length<0
				Thrower.Throw(
					"Invalid data length, expected {0} but received {1}",
					length, data.Length);
			}
		}

		private void checkBlockLength(byte[] response, int length)
		{
			var _length = response[12] << 8 | response[13];
			if (_length != length) {
				Thrower.Throw(
					"Invalid block length, expected {0} but received {1}", 
					length, _length);
			}
		}
	}
}

