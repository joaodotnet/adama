using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dama.IdentityServer.Models.AccountViewModels
{
    public class LogoutInputModel
    {
        public string LogoutId { get; set; }
    }
}
