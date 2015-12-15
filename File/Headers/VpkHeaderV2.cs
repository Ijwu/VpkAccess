using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using VPKAccess.Exceptions;

namespace VPKAccess
{
    public struct VpkHeaderV2
    {
        public const uint Signature = 0x55aa1234;
        public const uint Version = 2;

        /// <summary>
        /// Size, in bytes, of the directory tree.
        /// </summary>
        public uint TreeSize { get; set; }

        /// <summary>
        /// How many bytes of file content are stored by this file.
        /// </summary>
        public uint FileDataSectionSize { get; set; }

        /// <summary>
        /// The size, in bytes, of the section containing MD5 checksums for external archive content.
        /// </summary>
        public uint ArchiveMD5SectionSize { get; set; }

        /// <summary>
        /// The size, in bytes, of th section containing MD5 checksums for content in this file. (Should be 48 always)
        /// </summary>
        public uint OtherMD5SectionSize { get; set; }

        /// <summary>
        /// The size, in bytes, of the section containing the public key and signature.
        /// This is either 0 (CSGO & The Ship) or 296 (HL2, HL2:DM, HL2:EP1, HL2:EP2, HL2:LC, TF2, DOD:S & CS:S)
        /// </summary>
        public uint SignatureSectionSize { get; set; }

        public static VpkHeaderV2 ReadVpkHeaderV2(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
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

                var output = new VpkHeaderV2();

                output.TreeSize = reader.ReadUInt32();
                output.FileDataSectionSize = reader.ReadUInt32();
                output.ArchiveMD5SectionSize = reader.ReadUInt32();
                output.OtherMD5SectionSize = reader.ReadUInt32();
                output.SignatureSectionSize = reader.ReadUInt32();

                return output;
            }
        }
    }
}