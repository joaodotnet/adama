using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaWeb.ViewModels
{
    //public class MenuComponentViewModel
    //{
    //    public List<MenuItemComponentViewModel> Left { get; set; } = new List<MenuItemComponentViewModel>();
    //    public List<MenuItemComponentViewModel> Right { get; set; } = new List<MenuItemComponentViewModel>();
    //}

    public class MenuItemComponentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameUri { get; set; }
        public string TypeUri { get; set; }
        public List<MenuItemComponentViewModel> Childs { get; set; } = new List<MenuItemComponentViewModel>();
    }
}
