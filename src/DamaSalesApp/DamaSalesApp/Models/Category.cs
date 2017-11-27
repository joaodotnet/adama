using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaSalesApp.Models
{
    public class Category
    {
        [PrimaryKey]
        public int Id { get; set; }        
        public string Name { get; set; }
        public string ImgSource { get; set; }
    }
}
