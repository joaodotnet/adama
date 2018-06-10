using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;

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

        [BindProperty]
        public string TagName { get; set; }

        [BindProperty]
        public string Tag { get; set; }

        public async Task<IActionResult> OnGetAsync(CatalogIndexViewModel catalogModel, string tagName, string q, int? p)
        {
            if (string.IsNullOrEmpty(tagName))
                return NotFound();

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

            var tagToSearch = Utils.StringToUri(tagName);

            CatalogModel = await _service.GetCatalogItemsByTag(p ?? 0, Constants.ITEMS_PER_PAGE, tagToSearch, tagType, catalogModel.TypesFilterApplied, catalogModel.IllustrationFilterApplied);
            return Page();
        }
    }
}