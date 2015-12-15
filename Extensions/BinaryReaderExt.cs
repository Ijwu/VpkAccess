using System;
using System.IO;
using System.Text;

namespace VPKAccess
{
    public static class BinaryReaderExt
    {
        public static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            StringBuilder output = new StringBuilder();
            int buffer = -1;
            while ((buffer = reader.ReadByte()) != 0x00)
            {
                if (buffer == -1)
                    throw new EndOfStreamException("Null teriminated string could not be read past end of stream.");
                output.Append(encoding.GetString(new[] {(byte)buffer}));
            }

            return output.ToString();
        }
    }
}   