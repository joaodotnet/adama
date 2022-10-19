namespace ApplicationCore.DTOs
{
    public class FileData
    {
        public byte[] Data { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public string FileName { get; set; }
        public bool Uploaded { get; set; }
        public string? StoredFileName { get; set; }
        public int ErrorCode { get; set; }
        public bool IsPrincipal { get; set; }
    }
}
