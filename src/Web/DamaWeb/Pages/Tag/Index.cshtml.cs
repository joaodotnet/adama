using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using ApplicationCore;

namespace DamaWeb.Pages.Tag
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _service;

        public IndexModel(ICatalogService catalogService)
        {
            _service = catalogService;
        }
        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

        public string TagName { get; set; }

        public string Tag { get; set; }

        public async Task<IActionResult> OnGetAsync(string tagName, string q, int? p)
        {
            if (string.IsNullOrEmpty(tagName) || tagName.Contains(" "))
                return RedirectPermanent(Url.Page("/Index"));

            TagName = tagName;
            Tag = q;

            TagType? tagType = null;
            if(!string.IsNullOrEmpty(q))
            {
                if (q.ToLower() == "tipo")
                    tagType = TagType.CATALOG_TYPE;
                else if (q.ToLower() == "ilustracao")
                    tagType = TagType.ILLUSTRATION;
                else if (q.ToLower() == "ilustracao_tipo")
                    tagType = TagType.ILLUSTRATION_TYPE;
            }

            var tagToSearch = Utils.URLFriendly(tagName);

            p = p < 0 ? 0 : p;

            CatalogModel = await _service.GetCatalogItemsByTag(p ?? 0, Constants.ITEMS_PER_PAGE, tagToSearch, tagType);
            if (CatalogModel == null)
                return NotFound();
            return Page();
        }
    }
}
