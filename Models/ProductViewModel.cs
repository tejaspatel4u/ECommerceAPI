using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceAPI.Models
{
    public class ProductViewModel
    {
        public long ProductId { get; set; }
        public int ProdCatId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public ProductAttributeViewModel objProductAttribute { get; set; }
        public ICollection<ProductAttributeViewModel> ListProductAttributes { get; set; }
        //public ICollection<ProductCategoryViewModel> ListProductCategory { get; set; }
    }

    public class ProductAttributeViewModel
    {
        public long ProductId { get; set; }
        public int AttributeId { get; set; }
        public string AttributeValue { get; set; }
    }

    public class ProductCategoryViewModel
    {
        public int ProdCatId { get; set; }
        public string CategoryName { get; set; }
    }

    public class ProductAttributeLookupViewModel
    {
        public int AttributeId { get; set; }
        public int ProdCatId { get; set; }
        public string AttributeName { get; set; }
    }
}