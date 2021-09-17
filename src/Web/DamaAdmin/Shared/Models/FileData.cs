namespace DamaAdmin.Shared.Models
{
    public class FileData
    {
        public byte[] Data { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public string FileName { get; set; }
    }
}
