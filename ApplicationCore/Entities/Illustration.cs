namespace ApplicationCore.Entities
{
    public class Illustration : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PictureUri { get; set; }
        public int IllustrationTypeId { get; set; }
        public IllustrationType IllustrationType { get; set; }
        public byte[] Image { get; set; }
    }
}