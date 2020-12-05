using System;
using System.Collections.Generic;
using System.Data.Entity;
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

                productlist = ctx.Products.Include("ProductCategory")
                            .Select(p => new ProductViewModel()
                            {
                                ProductId = p.ProductId,
                                ProdCatId = p.ProdCatId,
                                ProductName = p.ProdName,
                                ProductDescription = p.ProdDescription,
                                CategoryName = p.ProductCategory.CategoryName
                                //ListProductAttributes = {prductid= p.Pr
                            }).OrderByDescending(p => p.ProductId).ToList<ProductViewModel>();


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
                                ProdCatId = p.ProdCatId,
                                ProductName = p.ProdName,
                                ProductDescription = p.ProdDescription,
                                //ListProductAttributes = {prductid= p.Pr
                            }).Where(p => p.ProductId == Id).FirstOrDefault();

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

        public IHttpActionResult GetAllCategories()
        {
            IList<ProductCategoryViewModel> productCatlist = null;

            using (var ctx = new ECommerceDemoEntities())
            {
                //var s = ctx.Products.Include("ProductAttribute").ToList();

                productCatlist = ctx.ProductCategories
                            .Select(p => new ProductCategoryViewModel()
                            {
                                ProdCatId = p.ProdCatId,
                                CategoryName = p.CategoryName

                            }).ToList<ProductCategoryViewModel>();
            }

            if (productCatlist.Count == 0)
            {
                //return NotFound();
            }

            return Ok(productCatlist);
        }

        public IHttpActionResult GetAllProductAttributeLookup(int ProdCatId, long? ProductId)
        {
            IList<ProductAttributeLookupViewModel> productAttributelist = null;

            using (var ctx = new ECommerceDemoEntities())
            {
                //var s = ctx.Products.Include("ProductAttribute").ToList();

                productAttributelist = ctx.ProductAttributeLookups
                            .Where(p => p.ProdCatId == ProdCatId)
                            .Select(p => new ProductAttributeLookupViewModel()
                            {
                                AttributeId = p.AttributeId,
                                AttributeName = p.AttributeName,
                                ProdCatId = p.ProdCatId
                            }).ToList<ProductAttributeLookupViewModel>();

                //If in edit mode then also pass attribute value
                if (ProductId != null && ProductId != 0)
                {
                    for (int i = 0; i < productAttributelist.Count; i++)
                    {
                        int AttributeId = productAttributelist[i].AttributeId;

                        string AttributeValue = ctx.ProductAttributes
                                            .Where(p => p.ProductId == ProductId && p.AttributeId == AttributeId)
                                            .Select(p => p.AttributeValue).FirstOrDefault();

                        productAttributelist[i].AttributeValue = AttributeValue;
                    }
                }
            }

            if (productAttributelist.Count == 0)
            {
                //return NotFound();
            }

            return Ok(productAttributelist);
        }

        public IHttpActionResult PostNewProduct(ProductViewModel objproduct)
        {
            ProductViewModel product = objproduct;
            

            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            using (var ctx = new ECommerceDemoEntities())
            {
                //Check Duplicate record
                var CheckDuplicate = ctx.Products.Where(p => p.ProdCatId == product.ProdCatId && p.ProdName == product.ProductName).FirstOrDefault();
                if(CheckDuplicate != null)
                {
                    return Conflict();
                }

                //Save Product
                ctx.Products.Add(new Product()
                {
                    //ProductId = product.ProductId,
                    ProdCatId = product.ProdCatId,
                    ProdName = product.ProductName,
                    ProdDescription = product.ProductDescription
                });

                ctx.SaveChanges();
                long ProductId = product.ProductId;
                //ctx.Entry(product).GetDatabaseValues();

                //Save Product Attribute
                ProductId = ctx.Products.OrderByDescending(p => p.ProductId).Select(p => p.ProductId).FirstOrDefault();


                foreach (var item in product.ListProductAttributes)
                {
                    ctx.ProductAttributes.Add(new ProductAttribute()
                    {
                        ProductId = ProductId,
                        AttributeId = item.AttributeId,
                        AttributeValue = item.AttributeValue
                    });

                }

                ctx.SaveChanges();
            }

            return Ok();
        }

        public IHttpActionResult PutProduct(ProductViewModel objproduct)
        {
            ProductViewModel product = objproduct;

            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            using (var ctx = new ECommerceDemoEntities())
            {
                //Check Duplicate record
                var CheckDuplicate = ctx.Products.Where(p => p.ProductId != product.ProductId && p.ProdCatId == product.ProdCatId && p.ProdName == product.ProductName).FirstOrDefault();
                if (CheckDuplicate != null)
                {
                    return Conflict();
                }

                //Save Record
                var existingProdut = ctx.Products.Where(p => p.ProductId == product.ProductId).FirstOrDefault();
                if (existingProdut != null)
                {
                    existingProdut.ProdName = product.ProductName;
                    existingProdut.ProdDescription = product.ProductDescription;
                    existingProdut.ProdCatId = product.ProdCatId;

                    ctx.SaveChanges();
                }
                else
                {
                    return NotFound();
                }

                long ProductId = product.ProductId;
                //ctx.Entry(product).GetDatabaseValues();

                //Delete and Save Product Attribute
                ctx.ProductAttributes.RemoveRange(ctx.ProductAttributes.Where(p => p.ProductId == product.ProductId));
                //var existingAttribute = ctx.ProductAttributes.Where(p => p.ProductId == product.ProductId); 
                //ctx.Entry(existingAttribute).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
                
                foreach (var item in product.ListProductAttributes)
                {
                    ctx.ProductAttributes.Add(new ProductAttribute()
                    {
                        ProductId = ProductId,
                        AttributeId = item.AttributeId,
                        AttributeValue = item.AttributeValue
                    });
                }

                ctx.SaveChanges();
            }

            return Ok();
        }

        public IHttpActionResult DeleteProduct(long ProductId)
        {
            ProductViewModel product = new ProductViewModel();

            using (var ctx = new ECommerceDemoEntities())
            {
                //Delete Product Attribute
                ctx.ProductAttributes.RemoveRange(ctx.ProductAttributes.Where(p => p.ProductId == ProductId));
                ctx.SaveChanges();

                //Delete Product
                var existingProduct = ctx.Products.Where(p => p.ProductId == ProductId).FirstOrDefault(); 
                ctx.Entry(existingProduct).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok();
        }
    }
}
