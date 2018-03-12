using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Tag
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _service;

        public IndexModel(ICatalogService catalogService)
        {
            _service = catalogService;
        }
        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

        [BindProperty]
        public string TagName { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string q)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            TagName = id;
            
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

            var tagToSearch = Utils.StringToUri(id);

            CatalogModel = await _service.GetCatalogItemsByTag(tagToSearch, tagType);
            return Page();
        }
    }
}