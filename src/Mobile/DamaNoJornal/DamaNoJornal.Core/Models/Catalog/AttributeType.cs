namespace DamaNoJornal.Core.Models.Catalog
{
    public enum AttributeType
    {        
        SIZE,        
        BOOK_FORMAT,        
        Color,
    }

    public static class AttributeTypeHelper
    {
        public static string GetTypeDescription(AttributeType type)
        {
            switch (type)
            {
                case AttributeType.SIZE:
                    return "Tamanho";
                case AttributeType.BOOK_FORMAT:
                    return "Formato";
                case AttributeType.Color:
                    return "Cor";
                default:
                    return "";
            }
        }
    }
}