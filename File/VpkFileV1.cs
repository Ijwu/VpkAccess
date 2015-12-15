using System.IO;

namespace VPKAccess
{
    public class VpkFileV1 : VpkFileBase
    {
        public VpkHeaderV1 Header { get; private set; }

        public VpkFileV1(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// Creates a new <see cref="VpkFileV1"/> from the input path to the VPK package index file.
        /// </summary>
        /// <param name="path">Path to package index file.</param>
        /// <returns><see cref="VpkFileV1"/> representing the VPK package.</returns>
        public static VpkFileV1 ReadVpkV1File(string path)
        {
            var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            var output = new VpkFileV1(path);
            output.Header = VpkHeaderV1.ReadVpkHeaderV1(stream);
            output.ReadFileTree(stream);
            return output;
        }
    }
}