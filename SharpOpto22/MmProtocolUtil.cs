using System;
using System.Collections.Generic;

namespace SharpOpto22
{
	/**
	 * FloatUtil provides serialize methods for float type.
	 * 
	 * Page 51 IEEE 754 float
	 * 
	 * @author Samuel Ventura
	 * 
	 */	
	public class MmProtocolUtil
	{
		/**
		 * UnitType
		 * 
		 * <p>
		 * <b>Unit type:</b> Page 113 Status Area. Read from address FFFF F030 0020
		 * 
		 * <pre>
		 * 0x00000062 SNAP-PAC-SB2
		 * 0x00000064 SNAP-PAC-SB1
		 * 0x0000006E SNAP-PAC-S2
		 * 0x00000074 SNAP-PAC-EB2
		 * 0x00000076 SNAP-PAC-EB1
		 * 0x00000078 SNAP-PAC-R2
		 * 0x0000007A SNAP-PAC-R1
		 * 0x0000007C SNAP-PAC-S1
		 * 0x00000083 SNAP-ENET-S64
		 * 0x0000008A SNAP-UPN-ADS
		 * 0x0000008C SNAP-UP1-M64
		 * 0x00000092 SNAP-UP1-D64
		 * 0x00000093 SNAP-UP1-ADS
		 * 0x00000094 SNAP-WLAN-FH-ADS
		 * 0x00000097 SNAP-ENET-D64
		 * 0x00000098 SNAP-B3000-ENET or SNAP-ENET-RTC
		 * 0x000000E1 E1
		 * 0x000000E2 E2
		 * 0x00000193 SNAP-LCE
		 * </pre>
		 * 
		 * <ul>
		 * <li>
		 * </ul>
		 * */
		private static Dictionary<int, string> unitTypes;

		public static string GetUnitType(int deviceType)
		{
			if (unitTypes == null) {
				unitTypes = new Dictionary<int, string>();
				
				unitTypes.Add(0x00000062, "SNAP-PAC-SB2");
				unitTypes.Add(0x00000064, "SNAP-PAC-SB1");
				unitTypes.Add(0x0000006E, "SNAP-PAC-S2");
				unitTypes.Add(0x00000074, "SNAP-PAC-EB2");
				unitTypes.Add(0x00000076, "SNAP-PAC-EB1");
				unitTypes.Add(0x00000078, "SNAP-PAC-R2");
				unitTypes.Add(0x0000007A, "SNAP-PAC-R1");
				unitTypes.Add(0x0000007C, "SNAP-PAC-S1");
				unitTypes.Add(0x00000083, "SNAP-ENET-S64");
				unitTypes.Add(0x0000008A, "SNAP-UPN-ADS");
				unitTypes.Add(0x0000008C, "SNAP-UP1-M64");
				unitTypes.Add(0x00000092, "SNAP-UP1-D64");
				unitTypes.Add(0x00000093, "SNAP-UP1-ADS");
				unitTypes.Add(0x00000094, "SNAP-WLAN-FH-ADS");
				unitTypes.Add(0x00000097, "SNAP-ENET-D64");
				unitTypes.Add(0x00000098, "SNAP-B3000-ENET or SNAP-ENET-RTC");
				unitTypes.Add(0x000000E1, "E1");
				unitTypes.Add(0x000000E2, "E2");
				unitTypes.Add(0x00000193, "SNAP-LCE");				
			}
			
			if (unitTypes.ContainsKey(deviceType)) { 
				return unitTypes[deviceType]; 
			}
			
			return "Unknown";
		}
	
		/**
		 * NakCode
		 * 
		 * <p>
		 * Response packets contain an rcode parameter, which shows whether the response is an ACK or a NAK. An rcode of 0
		 * is an ACK; anything else is a NAK. If a NAK appears in the rcode parameter, check the Status area of the memory
		 * map to find out the reason for the NAK.
		 * <p>
		 * <b>CAUTION:</b> If more than one client is communicating with the memory map (for example, your program and PAC
		 * Manager), you may read an overridden NAK code.
		 * <ul>
		 * <li>Page 103 Error Codes
		 * <li>Page 113 Status Area
		 * </ul>
		 * */
		private static Dictionary<int, string> nakCodes;
	
		public static string GetNakCode(int nakCode)
		{
			if (nakCodes == null) {
				nakCodes = new Dictionary<int, string>();
				
				nakCodes.Add(0x0000, "No error");
				nakCodes.Add(0xE001, "Undefined command");
				nakCodes.Add(0xE002, "Invalid point type");
				nakCodes.Add(0xE003, "Invalid float");
				nakCodes.Add(0xE004, "Powerup Clear expected");
				nakCodes.Add(0xE005, "Invalid memory address or invalid data for the memory address");
				nakCodes.Add(0xE006, "Invalid command length");
				nakCodes.Add(0xE007, "Reserved");
				nakCodes.Add(0xE008, "Busy");
				nakCodes.Add(0xE009, "Cannot erase flash");
				nakCodes.Add(0xE00A, "Cannot program flash");
				nakCodes.Add(0xE00B, "Downloaded image too small");
				nakCodes.Add(0xE00C, "Image CRC mismatch");
				nakCodes.Add(0xE00D, "Image length mismatch");
				nakCodes.Add(0xE00E, "Feature is not yet implemented");
				nakCodes.Add(0xE00F, "Communications watchdog timeout");				
			}
			
			if (nakCodes.ContainsKey(nakCode)) { 
				return nakCodes[nakCode]; 
			}
			
			return "Unknown";
		}
	
		/**
		 * ModuleType
		 * 
		 * <pre>
		 *      This hex value   Indicates this module type        This hex value       Indicates this module type
		 *      00               4-channel digital or empty*       4D                   SNAP-AIMA-32
		 *      04               SNAP-AICTD                        4E                   SNAP-AIV-32
		 *      09               SNAP-AITM-2                       4F                   SNAP-AITM-8
		 *      0A               SNAP-AIPM                         64                   SNAP-AIMA
		 *      0B               SNAP-AILC                         66                   SNAP-AITM
		 *      0C               SNAP-AILC-2                       69                   SNAP-AIRATE
		 *      10               SNAP-AIRTD                        70                   SNAP-AIVRMS
		 *      12               SNAP-AIV                          71                   SNAP-AIARMS
		 *      20               SNAP-AITM-i                       A3                   SNAP-AOA23
		 *      21               SNAP-AITM2-i                      A5                   SNAP-AOV25
		 *      22               SNAP-AIMA-i                       A7                   SNAP-AOV27
		 *      23               SNAP-AIV-i                        A8                   SNAP-AOA28
		 *      24               SNAP-AIV2-i                       A9                   SNAP-AOD29
		 *      25               SNAP-pH/ORP                       B3                   SNAP-AOA23-iSRC
		 *      26               SNAP-AIMA-iSRC                    D0                   SNAP-PID-V
		 *      27               SNAP-AIMA2-i                      E0                   SNAP-IDC-32
		 *      28               SNAP-AIARMS-i                     E1                   SNAP-ODC-32-SRC
		 *      29               SNAP-AIVRMS-i                     E2                   SNAP-ODC-32-SNK
		 *      40               SNAP-AIMA-4                       E3                   SNAP-IAC-A-16
		 *      41               SNAP-AIV-4                        E4                   SNAP-IAC-16
		 *      42               SNAP-AICTD-4                      E5                   SNAP-IDC-16
		 *      43               SNAP-AIR40K-4                     F0                   SNAP-SCM-232
		 *      44               SNAP-AIMV-4                       F1                   SNAP-SCM-485 or SNAP-SCM-485-422
		 *      45               SNAP-AIMV2-4                      F6                   SNAP-SCM-PROFI
		 *      4A               SNAP-AIMA-8                       F8                   SNAP-SCM-MCH16
		 *      4B               SNAP-AIV-8                        F9                   SNAP-SCM-W2
		 *      4C               SNAP-AICTD-8
		 * Digital modules with more than four points are individually listed in this table.
		 * </pre>
		 * 
		 * <p>
		 * Reference
		 * <ul>
		 * <li>Page 106 Module Type Code
		 * <li>Page 143
		 * </ul>
		 * */
		private static Dictionary<int, string> moduleTypes;
	
		public static string GetModuleType(int moduleType)
		{
			if (moduleTypes == null) {
				moduleTypes = new Dictionary<int, string>();
				moduleTypes.Add(0x00, "4-channel digital or empty*");
				moduleTypes.Add(0x4D, "SNAP-AIMA-32");
				moduleTypes.Add(0x04, "SNAP-AICTD");
				moduleTypes.Add(0x4E, "SNAP-AIV-32");
				moduleTypes.Add(0x09, "SNAP-AITM-2");
				moduleTypes.Add(0x4F, "SNAP-AITM-8");
				moduleTypes.Add(0x0A, "SNAP-AIPM");
				moduleTypes.Add(0x64, "SNAP-AIMA");
				moduleTypes.Add(0x0B, "SNAP-AILC");
				moduleTypes.Add(0x66, "SNAP-AITM");
				moduleTypes.Add(0x0C, "SNAP-AILC-2");
				moduleTypes.Add(0x69, "SNAP-AIRATE");
				moduleTypes.Add(0x10, "SNAP-AIRTD");
				moduleTypes.Add(0x70, "SNAP-AIVRMS");
				moduleTypes.Add(0x12, "SNAP-AIV");
				moduleTypes.Add(0x71, "SNAP-AIARMS");
				moduleTypes.Add(0x20, "SNAP-AITM-i");
				moduleTypes.Add(0xA3, "SNAP-AOA23");
				moduleTypes.Add(0x21, "SNAP-AITM2-i");
				moduleTypes.Add(0xA5, "SNAP-AOV25");
				moduleTypes.Add(0x22, "SNAP-AIMA-i");
				moduleTypes.Add(0xA7, "SNAP-AOV27");
				moduleTypes.Add(0x23, "SNAP-AIV-i");
				moduleTypes.Add(0xA8, "SNAP-AOA28");
				moduleTypes.Add(0x24, "SNAP-AIV2-i");
				moduleTypes.Add(0xA9, "SNAP-AOD29");
				moduleTypes.Add(0x25, "SNAP-pH/ORP");
				moduleTypes.Add(0xB3, "SNAP-AOA23-iSRC");
				moduleTypes.Add(0x26, "SNAP-AIMA-iSRC");
				moduleTypes.Add(0xD0, "SNAP-PID-V");
				moduleTypes.Add(0x27, "SNAP-AIMA2-i");
				moduleTypes.Add(0xE0, "SNAP-IDC-32");
				moduleTypes.Add(0x28, "SNAP-AIARMS-i");
				moduleTypes.Add(0xE1, "SNAP-ODC-32-SRC");
				moduleTypes.Add(0x29, "SNAP-AIVRMS-i");
				moduleTypes.Add(0xE2, "SNAP-ODC-32-SNK");
				moduleTypes.Add(0x40, "SNAP-AIMA-4");
				moduleTypes.Add(0xE3, "SNAP-IAC-A-16");
				moduleTypes.Add(0x41, "SNAP-AIV-4");
				moduleTypes.Add(0xE4, "SNAP-IAC-16");
				moduleTypes.Add(0x42, "SNAP-AICTD-4");
				moduleTypes.Add(0xE5, "SNAP-IDC-16");
				moduleTypes.Add(0x43, "SNAP-AIR40K-4");
				moduleTypes.Add(0xF0, "SNAP-SCM-232");
				moduleTypes.Add(0x44, "SNAP-AIMV-4");
				moduleTypes.Add(0xF1, "SNAP-SCM-485 or SNAP-SCM-485-422");
				moduleTypes.Add(0x45, "SNAP-AIMV2-4");
				moduleTypes.Add(0xF6, "SNAP-SCM-PROFI");
				moduleTypes.Add(0x4A, "SNAP-AIMA-8");
				moduleTypes.Add(0xF8, "SNAP-SCM-MCH16");
				moduleTypes.Add(0x4B, "SNAP-AIV-8");
				moduleTypes.Add(0xF9, "SNAP-SCM-W2");
				moduleTypes.Add(0x4C, "SNAP-AICTD-8");				
			}
			
			if (moduleTypes.ContainsKey(moduleType)) { 
				return moduleTypes[moduleType]; 
			}
			
			return "Unknown";
		}
	}
}

