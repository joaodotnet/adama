using Newtonsoft.Json;

namespace DamaNoJornal.Core.Models.User
{
    public class UserInfo
    {
        public string UserId { get; set; }

        public string PreferredUsername { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string CardNumber { get; set; }

        public string CardHolder { get; set; }

        public string CardSecurityNumber { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string Email { get; set; }

        public bool EmailVerified { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberVerified { get; set; }
    }
}
