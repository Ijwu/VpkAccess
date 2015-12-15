using System;
using System.IO;
using System.Linq;
using System.Text;

namespace VPKAccess
{
    public struct VpkDirectoryEntry
    {
        /// <summary>
        /// The 32bit CRC of the file's data.
        /// </summary>
        public int CRC { get; set; }

        /// <summary>
        /// The number of bytes contained in the index file.
        /// </summary>
        public short PreloadBytes { get; set; }

        /// <summary>
        /// Zero based index of the archive this file's data is in.
        /// If 0x7fff, the data follows the directory.
        /// </summary>
        public short ArchiveIndex { get; set; }

        /// <summary>
        /// If ArchiveIndex is 0x7fff, the offset of the file data relative to the end of the directory.
        /// Otherwise, the offset of the data from the start of the specified archive.
        /// </summary>
        public int EntryOffset { get; set; }

        /// <summary>
        /// If zero, the entire file is stored in the preload data.
        /// Otherwise, the number of bytes stored starting at EntryOffset.
        /// </summary>
        public int EntryLength { get; set; }

        public const ushort Terminator = 0xffff;

        /// <summary>
        /// The content of the preload data of this entry.
        /// </summary>
        public byte[] PreloadData { get; set; }

        /// <summary>
        /// The path of this file in the package index.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Read a <see cref="VpkDirectoryEntry"/> from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>A <see cref="VpkDirectoryEntry"/> representing the data from the stream.</returns>
        public static VpkDirectoryEntry ReadVpkDirectoryEntry(Stream stream)
        {
            var output = new VpkDirectoryEntry();
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                output.CRC = reader.ReadInt32();
                output.PreloadBytes = reader.ReadInt16();
                output.ArchiveIndex = reader.ReadInt16();
                output.EntryOffset = reader.ReadInt32();
                output.EntryLength = reader.ReadInt32();
                reader.ReadUInt16();

                if (output.PreloadBytes > 0)
                {
                    output.PreloadData = reader.ReadBytes(output.PreloadBytes);
                }

                return output;
            }
        }
    }
}