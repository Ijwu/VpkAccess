using System.IO;
using System.Text;
using VPKAccess.Exceptions;

namespace VPKAccess
{
    public struct VpkHeaderV1
    {
        public const uint Signature = 0x55aa1234;
        public const uint Version = 1;

        public uint TreeSize { get; set; }

        public static VpkHeaderV1 ReadVpkHeaderV1(FileStream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream,Encoding.UTF8, true))
            {
                var sig = reader.ReadUInt32();
                if (sig != Signature)
                {
                    throw new FileFormatException("File header is not recognizable.");
                }

                var ver = reader.ReadUInt32();
                if (ver != Version)
                {
                    throw new FileFormatException("File header is not the correct version.");
                }

                var output = new VpkHeaderV1();

                output.TreeSize = reader.ReadUInt32();

                return output;
            }
        }
    }
}