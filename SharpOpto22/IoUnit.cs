using System;
using System.Text;

namespace SharpOpto22
{
	//http://www.opto22.com/documents/1465_OptoMMP_Protocol_Guide.pdf
	public class IoUnit : IIoUnit
	{
		private readonly ASCIIEncoding ascii = new ASCIIEncoding();
		private readonly IMmProtocol protocol;
		
		public IoUnit(string ip, int port = 2001, int timeout = 400)
		{
			this.protocol = new MmProtocol(new SocketMmStream(ip, port, timeout));
		}
		
		public void Dispose()
		{
			protocol.Dispose();
		}

		// Get Device Type (Page 113)
		public MmNamedCode GetDeviceType()
		{
			var dof = 0xFFFFF0300020L;
			var d = protocol.ReadQuadlet(dof);
			var code = ByteArrayToInt(d);
			return new MmNamedCode(code, MmProtocolUtil.GetUnitType(code));
		}
	
		// Get Device Part Number (Page 115)
		public string GetDevicePartNumber()
		{
			var dof = 0xFFFFF0300080L;
			var d = protocol.ReadBlock(dof, 20);
			return ToNulTerminatedString(d);
		}
	
		// Get Firmware Date (Page 115)
		public string GetFirmwareDate()
		{
			var dof = 0xFFFFF03000A0L;
			var d = protocol.ReadBlock(dof, 10);
			return ToNulTerminatedString(d);
		}
	
		// Get Firmware Time (Page 115)
		public string GetFirmwareTime()
		{
			var dof = 0xFFFFF03000B0L;
			var d = protocol.ReadBlock(dof, 10);
			return ToNulTerminatedString(d);
		}
	
		// Get Firmware Revision (Page 113)
		public string GetFirmwareRevision()
		{
			var dof = 0xFFFFF030001CL;
			var d = protocol.ReadQuadlet(dof);
			var revision = '?';
			switch (d[2]) {
				case 0:
					revision = 'A';
					break;
				case 1:
					revision = 'B';
					break;
				case 2:
					revision = 'R';
					break;
			}
			return string.Format("{0}{1}.{2}{3}", revision, d[0], d[1], (char)('a' + d[3]));
		}
	
		// Send Powerup Clear (Page 120)
		public void SendPowerupClear()
		{
			var dof = 0xFFFFF0380000L;
			var d = new byte[] { 0x00, 0x00, 0x00, 0x01 };
			protocol.WriteQuadlet(dof, d);
		}
	
		// Requires Powerup Clear (Page 113)
		public bool RequiresPowerupClear()
		{
			var dof = 0xFFFFF0300004L;
			var d = protocol.ReadQuadlet(dof);
			return (d[3] != 0 || d[2] != 0 || d[1] != 0 || d[0] != 0); //d[3]=1
		}
	
		// Get Module Type (Page 108)
		public MmNamedCode GetModuleType(int module)
		{
			var dof = 0xFFFFF0100000L + 0x3000 * module;
			var d = protocol.ReadQuadlet(dof);
			var code = ByteArrayToInt(d);
			return new MmNamedCode(code, MmProtocolUtil.GetModuleType(code));
		}
		
		public string GetPointName(int point)
		{
			return GetPointName(point / 4, point % 4);
		}
	
		public void SetDigitalPointState(int point, bool state)
		{
			SetDigitalPointState(point / 4, point % 4, state);
		}
	
		public bool GetDigitalPointState(int point)
		{
			return GetDigitalPointState(point / 4, point % 4);
		}
	
		public bool GetAndClearDigitalPointOnLatch(int point)
		{
			return GetAndClearDigitalPointOnLatch(point / 4, point % 4);
		}
		
		public bool GetAndClearDigitalPointOffLatch(int point)
		{
			return GetAndClearDigitalPointOffLatch(point / 4, point % 4);
		}
		
		public void SetAnalogPointValue(int point, float value)
		{
			SetAnalogPointValue(point / 4, point % 4, value);
		}
	
		public float GetAnalogPointValue(int point)
		{
			return GetAnalogPointValue(point / 4, point % 4);
		}
		
		// Get Point Name (Page 108)
		public string GetPointName(int module, int point)
		{
			var dof = 0xFFFFF0100030L + 0x3000 * module + 0xC0 * point;
			var d = protocol.ReadBlock(dof, 0x33);
			return ToNulTerminatedString(d);
		}
	
		// Get Digital Point State (Page 137) 4-POINT MODULES ONLY
		public bool GetDigitalPointState(int module, int point)
		{
			var dof = 0xFFFFF0800000L + 0x40L * (4 * module + point);
			var d = protocol.ReadQuadlet(dof);
			return (d[3] != 0 || d[2] != 0 || d[1] != 0 || d[0] != 0); //d[3]=1
		}
	
		public bool GetAndClearDigitalPointOnLatch(int module, int point)
		{
			var dof = 0xFFFFF02E0004L + 0x18L * (4 * module + point);
			var d = protocol.ReadQuadlet(dof);
			return (d[3] != 0 || d[2] != 0 || d[1] != 0 || d[0] != 0); //d[3]=1		
		}
		
		public bool GetAndClearDigitalPointOffLatch(int module, int point)
		{
			var dof = 0xFFFFF02E0008L + 0x18L * (4 * module + point);
			var d = protocol.ReadQuadlet(dof);
			return (d[3] != 0 || d[2] != 0 || d[1] != 0 || d[0] != 0); //d[3]=1	
		}
	
		// Set Digital Point State (Page 138) 4-POINT MODULES ONLY
		public void SetDigitalPointState(int module, int point, bool state)
		{
			var dof = 0xFFFFF0900000L + 0x40L * (4 * module + point);
			var d = new byte[] { 0x00, 0x00, 0x00, 0x01 };
			if (!state) {
				dof += 0x04L;
			}
			protocol.WriteQuadlet(dof, d);
		}
		
		// Get Analog Point Value (Page 84 Extended)
		public float GetAnalogPointValue(int module, int point)
		{
			var dof = 0xFFFFF0260000L + 0x1000L * module + 0x40L * point;
			var d = protocol.ReadQuadlet(dof);
			return ByteArrayToFloat(d);
		}
	
		// Set Analog Point Value (Page 85 Extended)
		public void SetAnalogPointValue(int module, int point, float value)
		{
			var dof = 0xFFFFF02A0000L + 0x1000L * module + 0x40L * point;
			var d = FloatToByteArray(value);
			protocol.WriteQuadlet(dof, d);
		}
		
		private string ToNulTerminatedString(byte[] d)
		{
			var text = ascii.GetString(d);
			//remove from first \0
			//old trailing data maybe present
			var first = text.IndexOf('\0');
			if (first < 0)
				return text;
			return text.Substring(0, first);			
		}
		
		private int ByteArrayToInt(byte[] d)
		{
			if (BitConverter.IsLittleEndian)
				Array.Reverse(d);
			return BitConverter.ToInt32(d, 0);
		}
	
		private float ByteArrayToFloat(byte[] d)
		{
			if (BitConverter.IsLittleEndian)
				Array.Reverse(d);
			return BitConverter.ToSingle(d, 0);
		}
	
		private byte[] FloatToByteArray(float value)
		{
			var ar = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(ar);
			return ar;
		}
	}
}

