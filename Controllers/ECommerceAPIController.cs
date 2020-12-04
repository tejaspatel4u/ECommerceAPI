using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    public class ECommerceAPIController : ApiController
    {
        //http://localhost/ECommerceAPI/api/ECommerceAPI/GetAllProducts
        public IHttpActionResult GetAllProducts()
        {
            IList<ProductViewModel> productlist = null;

            using (var ctx = new ECommerceDemoEntities())
            {
                //var s = ctx.Products.Include("ProductAttribute").ToList();
                
                productlist = ctx.Products.Include("ProductAttribute")
                            .Select(p => new ProductViewModel()
                            {
                                ProductId = p.ProductId,
                                ProductName = p.ProdName,
                                ProductDescription = p.ProdDescription,
                                //ListProductAttributes = {prductid= p.Pr
                            }).ToList<ProductViewModel>();


                for (int i = 0; i < productlist.Count(); i++)
                {
                    long ProductId = productlist[i].ProductId;
                    productlist[i].ListProductAttributes = ctx.ProductAttributes.Where(p => p.ProductId == ProductId).Select(p => new ProductAttributeViewModel()
                    {
                        ProductId = p.ProductId,
                        AttributeId = p.AttributeId,
                        AttributeValue = p.AttributeValue
                    }).ToList<ProductAttributeViewModel>();
                }
                //productlist[0].ListProductAttributes = ctx.ProductAttributes.Where(p => p.ProductId == 1).Select(r => new {r.AttributeId }).ToList<ProductAttributeViewModel>();
            }

            if (productlist.Count == 0)
            {
                //return NotFound();
            }

            return Ok(productlist);
        }

        public IHttpActionResult GetAllProducts(long Id)
        {
            ProductViewModel productlist = null;

            using (var ctx = new ECommerceDemoEntities())
            {
                //var s = ctx.Products.Include("ProductAttribute").ToList();

                productlist = ctx.Products.Include("ProductAttribute")
                            .Select(p => new ProductViewModel()
                            {
                                ProductId = p.ProductId,
                                ProductName = p.ProdName,
                                ProductDescription = p.ProdDescription,
                                //ListProductAttributes = {prductid= p.Pr
                            }).Where(p=>p.ProductId == Id).FirstOrDefault();

                if (productlist != null)
                {
                    productlist.ListProductAttributes = ctx.ProductAttributes.Where(p => p.ProductId == Id).Select(p => new ProductAttributeViewModel()
                    {
                        ProductId = p.ProductId,
                        AttributeId = p.AttributeId,
                        AttributeValue = p.AttributeValue
                    }).ToList<ProductAttributeViewModel>();
                }
                //productlist[0].ListProductAttributes = ctx.ProductAttributes.Where(p => p.ProductId == 1).Select(r => new {r.AttributeId }).ToList<ProductAttributeViewModel>();
            }

            if (productlist == null)
            {
                //return NotFound();
            }

            return Ok(productlist);
        }
    }
}
