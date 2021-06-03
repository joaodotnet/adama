using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities
{
    public class IllustrationType : BaseEntity, IAggregateRoot
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public IllustrationType(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}