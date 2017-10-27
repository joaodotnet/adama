namespace ApplicationCore.Entities
{
    public class IllustrationViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string PictureUri { get; set; }
        public IllustrationType Type { get; set; }
    }
}