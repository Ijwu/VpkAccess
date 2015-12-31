using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPKAccess.Exceptions;

namespace VPKAccess
{
    public abstract class VpkFileBase
    {
        /// <summary>
        /// The path of the set of Vpk archives and index file represented by this object.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The directory tree being represented by this vpk file.
        /// </summary>
        public List<VpkDirectoryEntry> Files { get; } = new List<VpkDirectoryEntry>();

        protected VpkFileBase(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Reads a VPK file for a header and attempts to determine its file version.
        /// </summary>
        /// <param name="path">Path to the file to be scanned.</param>
        /// <returns>Integer version of the file read.</returns>
        public static int DetermineVpkFileVersion(string path)
        {
            var stream = File.OpenRead(path);
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var sig = reader.ReadUInt32();
                if (sig == 0x55aa1234)
                {
                    var version = reader.ReadUInt32();
                    return (int)version;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the file data for the provided <see cref="VpkDirectoryEntry"/>
        /// </summary>
        /// <param name="entry">The entry for which the data should be read.</param>
        /// <returns>Byte array of file data.</returns>
        public byte[] GetDataForDirectoryEntry(VpkDirectoryEntry entry)
        {
            if (entry.EntryLength > 0)
            {
                var filePath = Path.Combine(Path.GetDirectoryName(FilePath), $"pak01_{entry.ArchiveIndex:D3}.vpk");
                using (var archiveFile = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[entry.EntryLength];

                    archiveFile.Seek(entry.EntryOffset, SeekOrigin.Begin);
                    archiveFile.Read(buffer, 0, entry.EntryLength);

                    if (entry.PreloadBytes > 0)
                    {
                        buffer = buffer.Concat(entry.PreloadData).ToArray();
                    }

                    return buffer;
                }
            }
            else
            {
                if (entry.PreloadBytes > 0)  
                    return entry.PreloadData;
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Takes a stream with the position set after the VpkHeader.
        /// </summary>
        /// <param name="stream">Stream from which to read in the file tree.</param>
        protected virtual void ReadFileTree(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                while (true)
                {
                    var extension = reader.ReadNullTerminatedString();
                    if (string.IsNullOrEmpty(extension))
                    {
                        break;
                    }

                    while (true)
                    {
                        var folder = reader.ReadNullTerminatedString();
                        if (string.IsNullOrEmpty(folder))
                        {
                            break;
                        }

                        while (true)
                        {
                            var file = reader.ReadNullTerminatedString();
                            if (string.IsNullOrEmpty(file))
                            {
                                break;
                            }

                            VpkDirectoryEntry entry = VpkDirectoryEntry.ReadVpkDirectoryEntry(stream);
                            entry.FilePath = $"{folder}/{file}.{extension}";
                            Files.Add(entry);
                        }
                    }
                }
            }
        }
    }
}