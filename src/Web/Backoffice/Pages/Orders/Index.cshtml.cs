using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Interfaces;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Backoffice.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IBackofficeService _service;
        private readonly IAuthConfigRepository _authService;
        private BackofficeSettings _settings;

        public IndexModel(IBackofficeService service, IAuthConfigRepository authConfigRepository, IOptions<BackofficeSettings> options)
        {
            _service = service;
            _authService = authConfigRepository;
            _settings = options.Value;
        }
        public List<OrderViewModel> OrdersModel { get; set; } = new List<OrderViewModel>();
        public async Task<IActionResult> OnGetAsync()
        {
            //Check if has Access Token
            // var authConfig = await _authService.GetAuthConfigAsync(ApplicationCore.Entities.DamaApplicationId.DAMA_BACKOFFICE);
            // if(authConfig == null)
            // {
            //     return Redirect(string.Format(_settings.AuthorizationURL, _settings.ClientId, _settings.CallbackURL));
            // }
            OrdersModel = await _service.GetOrders();
            return Page();
        }
    }
}