using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CacheManagerApp.Models
{
    [Serializable]
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}