using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AuthConfig
    {      
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public SageApplicationType ApplicationId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SigningSecret { get; set; }
        public string CallbackURL { get; set; }
    }
}
