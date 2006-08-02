using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Modbus.Util
{
	public class ModbusUtil
	{
		private static ushort[] crcTable = {
			0X0000, 0XC0C1, 0XC181, 0X0140, 0XC301, 0X03C0, 0X0280, 0XC241,
			0XC601, 0X06C0, 0X0780, 0XC741, 0X0500, 0XC5C1, 0XC481, 0X0440,
			0XCC01, 0X0CC0, 0X0D80, 0XCD41, 0X0F00, 0XCFC1, 0XCE81, 0X0E40,
			0X0A00, 0XCAC1, 0XCB81, 0X0B40, 0XC901, 0X09C0, 0X0880, 0XC841,
			0XD801, 0X18C0, 0X1980, 0XD941, 0X1B00, 0XDBC1, 0XDA81, 0X1A40,
			0X1E00, 0XDEC1, 0XDF81, 0X1F40, 0XDD01, 0X1DC0, 0X1C80, 0XDC41,
			0X1400, 0XD4C1, 0XD581, 0X1540, 0XD701, 0X17C0, 0X1680, 0XD641,
			0XD201, 0X12C0, 0X1380, 0XD341, 0X1100, 0XD1C1, 0XD081, 0X1040,
			0XF001, 0X30C0, 0X3180, 0XF141, 0X3300, 0XF3C1, 0XF281, 0X3240,
			0X3600, 0XF6C1, 0XF781, 0X3740, 0XF501, 0X35C0, 0X3480, 0XF441,
			0X3C00, 0XFCC1, 0XFD81, 0X3D40, 0XFF01, 0X3FC0, 0X3E80, 0XFE41,
			0XFA01, 0X3AC0, 0X3B80, 0XFB41, 0X3900, 0XF9C1, 0XF881, 0X3840,
			0X2800, 0XE8C1, 0XE981, 0X2940, 0XEB01, 0X2BC0, 0X2A80, 0XEA41,
			0XEE01, 0X2EC0, 0X2F80, 0XEF41, 0X2D00, 0XEDC1, 0XEC81, 0X2C40,
			0XE401, 0X24C0, 0X2580, 0XE541, 0X2700, 0XE7C1, 0XE681, 0X2640,
			0X2200, 0XE2C1, 0XE381, 0X2340, 0XE101, 0X21C0, 0X2080, 0XE041,
			0XA001, 0X60C0, 0X6180, 0XA141, 0X6300, 0XA3C1, 0XA281, 0X6240,
			0X6600, 0XA6C1, 0XA781, 0X6740, 0XA501, 0X65C0, 0X6480, 0XA441,
			0X6C00, 0XACC1, 0XAD81, 0X6D40, 0XAF01, 0X6FC0, 0X6E80, 0XAE41,
			0XAA01, 0X6AC0, 0X6B80, 0XAB41, 0X6900, 0XA9C1, 0XA881, 0X6840,
			0X7800, 0XB8C1, 0XB981, 0X7940, 0XBB01, 0X7BC0, 0X7A80, 0XBA41,
			0XBE01, 0X7EC0, 0X7F80, 0XBF41, 0X7D00, 0XBDC1, 0XBC81, 0X7C40,
			0XB401, 0X74C0, 0X7580, 0XB541, 0X7700, 0XB7C1, 0XB681, 0X7640,
			0X7200, 0XB2C1, 0XB381, 0X7340, 0XB101, 0X71C0, 0X7080, 0XB041,
			0X5000, 0X90C1, 0X9181, 0X5140, 0X9301, 0X53C0, 0X5280, 0X9241,
			0X9601, 0X56C0, 0X5780, 0X9741, 0X5500, 0X95C1, 0X9481, 0X5440,
			0X9C01, 0X5CC0, 0X5D80, 0X9D41, 0X5F00, 0X9FC1, 0X9E81, 0X5E40,
			0X5A00, 0X9AC1, 0X9B81, 0X5B40, 0X9901, 0X59C0, 0X5880, 0X9841,
			0X8801, 0X48C0, 0X4980, 0X8941, 0X4B00, 0X8BC1, 0X8A81, 0X4A40,
			0X4E00, 0X8EC1, 0X8F81, 0X4F40, 0X8D01, 0X4DC0, 0X4C80, 0X8C41,
			0X4400, 0X84C1, 0X8581, 0X4540, 0X8701, 0X47C0, 0X4680, 0X8641,
			0X8201, 0X42C0, 0X4380, 0X8341, 0X4100, 0X81C1, 0X8081, 0X4040 
		};

		// converts to string of hex characters w/ appropriate padding
		// converts each char to decimal value		
		public static byte[] GetASCIIBytes(byte[] numbers)
		{			
			return Encoding.ASCII.GetBytes(String.Join("", Array.ConvertAll<byte, string>(numbers, delegate(byte n) { return n.ToString("X2"); })));
		}

		public static byte[] GetASCIIBytes(byte number)
		{
			return Encoding.ASCII.GetBytes(number.ToString("X2"));
		}

		public static byte[] GetASCIIBytes(int[] numbers)
		{
			return Encoding.ASCII.GetBytes(String.Join("", Array.ConvertAll<int, string>(numbers, delegate(int n) { return n.ToString("X4"); })));
		}

		public static byte[] GetASCIIBytes(char[] characters)
		{
			return Encoding.ASCII.GetBytes(characters);
		}

		public static byte[] GetBytes(char[] characters)
		{
			return Array.ConvertAll<char, byte>(characters, delegate(char c) { return (byte)c; });
		}

		public static ushort[] NetworkBytesToHostUInt16(byte[] networkBytes)
		{
			if (networkBytes.Length % 2 != 0)
				throw new FormatException("Array networkBytes must contain an even number of bytes.");

			ushort[] result = new ushort[networkBytes.Length / 2];

			for (int i = 0; i < result.Length; i++)
				result[i] = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(networkBytes, i * 2));

			return result;
		}

		//// TODO extract conversion to delegate
		//// TODO create unit tests
		//public static ushort[] HostBytesToNetworkUInt16(byte[] hostBytes)
		//{
		//    if (hostBytes.Length % 2 != 0)
		//        throw new FormatException("Array hostBytes must contain an even number of bytes.");

		//    ushort[] result = new ushort[hostBytes.Length / 2];

		//    for (int i = 0; i < result.Length; i++)
		//        result[i] = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(hostBytes, i * 2));

		//    return result;
		//}

		//public static byte[] GetASCIIBytes(ushort[] numbers)
		//{
		//    return Encoding.ASCII.GetBytes(String.Join("", Array.ConvertAll<ushort, string>(numbers, delegate(ushort n) { return n.ToString("X2"); })));
		//}

		public static byte[] HexToBytes(string hex)
		{
			if (hex == null)
				throw new ArgumentNullException("hex", "Argument cannot be null.");

			if (hex.Length % 2 != 0)
				throw new FormatException("Hex string must have even number of characters.");

			byte[] bytes = new byte[hex.Length / 2];

			for (int i = 0; i < bytes.Length; i++)
				bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

			return bytes;
		}

		public static byte[] CalculateLRC(byte[] data)
		{
			byte lrc = 0;
			foreach (byte b in data)
				lrc += b;

			lrc = (byte)((lrc ^ 0xFF) + 1);

			return new byte[] { lrc };
		}

		public static byte[] CalculateCRC(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException("data", "Argument data cannot be null.");

			ushort crc = ushort.MaxValue;
			byte tableIndex;

			foreach (byte b in data)
			{
				tableIndex = (byte)(crc ^ b);
				crc >>= 8;
				crc ^= crcTable[tableIndex];
			}

			return BitConverter.GetBytes(crc);
		}
	}
}
