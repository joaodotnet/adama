namespace DamaAdmin.Shared.Models
{
    public class FileDetailViewModel
    {
        public string PictureUri { get; set; }
        public string Location { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public bool? IsActive { get; set; }
        public int? Order { get; set; }
    }
}
