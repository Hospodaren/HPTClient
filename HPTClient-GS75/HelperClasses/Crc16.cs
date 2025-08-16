using System.IO;
using System.Text;

namespace HPTClient
{
    public class Crc16
    {
        //Name   : "CRC-16"
        //Poly   : 8005
        //Init   : 0000
        //RefIn  : True
        //RefOut : True
        //XorOut : 0000
        //Check  : BB3D
        const ushort polynomial = 0xA001;
        ushort[] table = new ushort[256];

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
        }

        public string GetCheckSumAsHexString(string fileName)
        {
            StreamReader sr = new StreamReader(fileName, new UTF8Encoding(false));
            return CreateChecksumString(sr);
        }

        public string GetCheckSumAsHexString(Stream stream)
        {
            StreamReader sr = new StreamReader(stream, new UTF8Encoding(false));
            return CreateChecksumString(sr);
        }

        private string CreateChecksumString(StreamReader sr)
        {
            string s = sr.ReadToEnd();
            UTF8Encoding enc = new UTF8Encoding(false);
            byte[] ba = enc.GetBytes(s);
            byte[] checksum = ComputeChecksumBytes(ba);
            string strChecksum = checksum[0].ToString("X2") + checksum[1].ToString("X2");
            return strChecksum;
        }

        public Crc16()
        {
            // TEMP
            var sb = new StringBuilder("CCITT_16 = [");

            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; i++)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; j++)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
                sb.Append("0x");
                sb.Append(value.ToString("X"));
                sb.Append(", ");
            }
            sb.Append("]");
            string result = sb.ToString();
        }
    }
}
