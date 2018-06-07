using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dama.IdentityServer.Models.AccountViewModels
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; }
    }
}
