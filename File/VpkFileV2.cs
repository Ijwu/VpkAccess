using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VPKAccess
{
    public class VpkFileV2 : VpkFileBase
    {
        /// <summary>
        /// VPK file header.
        /// </summary>
        public VpkHeaderV2 Header { get; private set; }

        private VpkFileV2(string path) : base(path)
        {
        }

        /// <summary>
        /// Creates a new <see cref="VpkFileV2"/> from the input path to the VPK package index file.
        /// </summary>
        /// <param name="path">Path to package index file.</param>
        /// <returns><see cref="VpkFileV2"/> representing the VPK package.</returns>
        public static VpkFileV2 ReadVpkV2File(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                var output = new VpkFileV2(path);
                output.Header = VpkHeaderV2.ReadVpkHeaderV2(stream);
                output.ReadFileTree(stream);
                return output;
            }
        }

        /// <summary>
        /// Takes a stream with the position set after the VpkHeader.
        /// Accommodates for archive indices of 0x7fff, only present in v2 files.
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
                            if (entry.ArchiveIndex == 0x7fff)
                                entry.EntryOffset += 28 + (int) Header.TreeSize;
                            entry.FilePath = $"{folder}/{file}.{extension}";
                            Files.Add(entry);
                        }
                    }
                }
            }
        }
    }
}
