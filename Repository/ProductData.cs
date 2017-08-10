using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Repository
{
    public class ProductData
    {

        #region private fields

            private readonly DatabaseEntities _db;

        #endregion


        #region Constructor

            public ProductData(string connectionString)
            {
                _db = new DatabaseEntities(connectionString);
            }

        #endregion


        #region Public Methods

            public async Task<List<Product>> GetAllProducts()
            {
                var items = await _db.Products.Select(item => item).ToListAsync(); 
                return items;
            }

            public async Task<List<Product>> SearchProductsByName(string name)
            {
                var items = await _db.Products.Where(p => p.Name.ToLower().Contains(name.ToLower())).ToListAsync();
                return items;
            }

            public async Task<Product> GetProductById(Guid id)
            {
                var item = await _db.Products.Where(p => p.Id == id).ToListAsync();
                return item.FirstOrDefault();
            }

            public async Task<Guid> CreateProduct(Product product)
            {
                product.Id = Guid.NewGuid();
                _db.Products.Add (product);
                await _db.SaveChangesAsync();
                return product.Id;
            }

            public async Task UpdateProduct(Product product)
            {
                var existedProduct = _db.Products.Where(p => p.Id == product.Id).FirstOrDefault();
                if (existedProduct != null)
                {
                    existedProduct.Name = product.Name;
                    existedProduct.Description = product.Description;
                    existedProduct.Price = product.Price;
                    existedProduct.DeliveryPrice = product.DeliveryPrice;

                    await _db.SaveChangesAsync();
                }
            }

            public async Task DeleteProuct(Guid id)
            {
                var existedProduct = _db.Products.Where(p => p.Id == id).FirstOrDefault();
                if (existedProduct != null)
                {
                    //delete options related to the product
                    var removingItems = _db.ProductOptions.Where(x => x.ProductId == id);
                    _db.ProductOptions.RemoveRange(removingItems);

                    _db.Products.Remove(existedProduct);
                    await _db.SaveChangesAsync();
                }
            }

            public async Task<List<ProductOption>> GetProductOptionsByProductId(Guid productId)
            {
                var items = await _db.ProductOptions.Where(x => x.ProductId == productId).ToListAsync();
                return items;
            }


            public async Task<ProductOption> GetProductOption(Guid optionId)
            {
                var item = await _db.ProductOptions.Where(p => p.Id == optionId).ToListAsync();
                return  item.FirstOrDefault();
            }

            public async Task<Guid> CreateProductOption(ProductOption productOption)
            {
                productOption.Id = Guid.NewGuid();
                _db.ProductOptions.Add(productOption);
                await _db.SaveChangesAsync();
                return productOption.Id;
            }

            public async Task UpdateProductOption(ProductOption productOption)
            {
                var existedProductOption = _db.ProductOptions.Where(p => p.Id == productOption.Id).FirstOrDefault();
                if (existedProductOption != null)
                {
                    existedProductOption.ProductId = productOption.ProductId;
                    existedProductOption.Name = productOption.Name;
                    existedProductOption.Description = productOption.Description;

                    await _db.SaveChangesAsync();
                }
            }

            public async Task DeleteProductOption(Guid id)
            {
                var existedProductOption = _db.ProductOptions.Where(p => p.Id == id).FirstOrDefault();
                if (existedProductOption != null)
                {
                    _db.ProductOptions.Remove(existedProductOption);
                    await _db.SaveChangesAsync();
                }
            }

        #endregion

    }
}
