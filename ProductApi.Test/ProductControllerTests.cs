using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using refactor_me.Controllers;
using Repository;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;

namespace ProductApi.Test
{
    [TestClass]
    public class ProductControllerTests
    {

        #region Private fields

            private readonly ProductsController _productController;
            private readonly ProductData _productRepository;

        #endregion


        #region Constructor

            public ProductControllerTests()
            {
                var conn = ConfigurationManager.ConnectionStrings["dbConnection"].ToString();               
                _productRepository = new ProductData(conn);
                _productController = new ProductsController();
                _productController.Request = new HttpRequestMessage();
                _productController.Configuration = new HttpConfiguration();
            }

        #endregion


        #region Public test methods

            [TestMethod]
            public async Task GetAllProducts_ShouldReturnAllProducts()
            {
                var testProducts = await _productRepository.GetAllProducts();

                var response = await _productController.GetAll();
                ObjectContent content = response.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(testProducts.Count, ((List<Product>)content.Value).Count);
            }

            [TestMethod]
            public async Task GetProductById_ShouldReturnOneSpecificProduct()
            {
                var products = await _productRepository.GetAllProducts();
                var testProduct = products[0];

                if (testProduct == null)
                {
                    var product = new Repository.Product
                    {
                        Name = "testName",
                        Description = "testDesc",
                        Price = 356,
                        DeliveryPrice = 13
                    };
                    await _productRepository.CreateProduct(product);
                    testProduct = product;
                }

                var response = await _productController.GetProduct(testProduct.Id);
                ObjectContent content = response.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(testProduct.Id, ((Product)content.Value).Id);
            }

            [TestMethod]
            public async Task SearchProductByName_ShouldReturnProductsContainTheName()
            {
                string name = "Samsung";
                var products = await _productRepository.SearchProductsByName(name);

                if (products == null || products.Count == 0)
                {
                    var product = new Repository.Product
                    {
                        Name = "Samsung Note3 Mobile",
                        Description = "testDesc",
                        Price = 356,
                        DeliveryPrice = 13
                    };
                    await _productRepository.CreateProduct(product);
                    products[0] = product;
                }

                var response = await _productController.SearchByName(name);
                ObjectContent content = response.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(products.Count, ((List<Product>)content.Value).Count);
            }

            [TestMethod]
            public async Task CreateProduct_ShouldAddProductIntoDb()
            {
                string name = Guid.NewGuid().ToString();
                var product = new Repository.Product
                {
                    Name = name,
                    Description = "DESC",
                    Price = 333,
                    DeliveryPrice = 11
                };
                await _productController.Create(product);

                var response = await _productController.SearchByName(name);
            
                ObjectContent content = response.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(name, ((List<Product>)content.Value)[0].Name);
            }

            [TestMethod]
            public async Task UpdateProduct_ShouldChangeProductIntoDb()
            {
                Product testProduct;
                var product = new Repository.Product
                {
                    Name = "testName",
                    Description = "testDesc",
                    Price = 356,
                    DeliveryPrice = 13
                };

                product.Id = await _productRepository.CreateProduct(product);
                testProduct = product;

                testProduct.Name = Guid.NewGuid().ToString();
                testProduct.Description = DateTime.Now.ToString();

                var response = await _productController.Update(testProduct);

                var responseGet = await _productController.GetProduct(testProduct.Id);
                ObjectContent content = responseGet.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(testProduct.Name, ((Product)content.Value).Name);
                Assert.AreEqual(testProduct.Description, ((Product)content.Value).Description);
            }

            [TestMethod]
            public async Task DeleteProduct_ShouldRemoveProductFromDb()
            {
                string name = Guid.NewGuid().ToString();
                Product testProduct;

                var product = new Repository.Product
                {
                    Name = name,
                    Description = "testDesc",
                    Price = 356,
                    DeliveryPrice = 13
                };

                product.Id = await _productRepository.CreateProduct(product);
                testProduct = product;

                var beforeDeletingResult = await _productRepository.GetProductById(testProduct.Id);

                Assert.IsNotNull(beforeDeletingResult);

                var response = await _productController.Delete(testProduct.Id);

                var responseSearch = await _productController.SearchByName(name);
                ObjectContent content = responseSearch.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(0, ((List<Product>)content.Value).Count); 
            }

            [TestMethod]
            public async Task GetOptions_ShouldReturnAllOptionsByProductId()
            {
                string name = "Samsung Note3 Mobile";
                var products = await _productRepository.SearchProductsByName(name);
                Product testProduct;

                if (products == null || products.Count == 0)
                {
                    var product = new Repository.Product
                    {
                        Name = name,
                        Description = "testDesc",
                        Price = 356,
                        DeliveryPrice = 13
                    };

                    product.Id = await _productRepository.CreateProduct(product);
                    testProduct = product;
                }
                else
                {
                    testProduct = products[0];
                }

                var productOption = new ProductOption
                {
                    ProductId = testProduct.Id,
                    Name = "test",
                    Description = "test as well"
                };
                await _productRepository.CreateProductOption(productOption);


                var testProductOptions = await _productRepository.GetProductOptionsByProductId(testProduct.Id);

                var response = await _productController.GetOptions(testProduct.Id);
                ObjectContent content = response.Content as ObjectContent;
            
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(testProductOptions.Count, ((List<ProductOption>)content.Value).Count);
            }

            [TestMethod]
            public async Task GetProductOption_ShouldReturnOneOptionById()
            {           
                var product = new Repository.Product
                {
                    Name = "Samsung Note3 Mobile",
                    Description = "testDesc",
                    Price = 356,
                    DeliveryPrice = 13
                };
                product.Id = await _productRepository.CreateProduct(product);

                var option = new Repository.ProductOption
                {
                    Name = "White",
                    Description = "White Samsung Note3 Mobile",
                    ProductId = product.Id
                };
                option.Id = await _productRepository.CreateProductOption(option);       
         
                var response = await _productController.GetOption(option.Id);
                ObjectContent content = response.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(option.Name, ((ProductOption)content.Value).Name);
                Assert.AreEqual(option.Description, ((ProductOption)content.Value).Description);
            }

            [TestMethod]
            public async Task CreateProductOption_ShouldAddProductOptionIntoDb()
            {
                var products = await _productRepository.GetAllProducts();
                var testProduct = products[0];

                string name = Guid.NewGuid().ToString();
                var productOption = new Repository.ProductOption
                {
                    ProductId = testProduct.Id,
                    Name = name,
                    Description = "DESC",
                };
                var response = await _productController.CreateOption(testProduct.Id, productOption);
                ObjectContent content = response.Content as ObjectContent;

                var responseGet = await _productController.GetOption((Guid)content.Value);
                content = responseGet.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(name, ((ProductOption)content.Value).Name);
            }

            [TestMethod]
            public async Task CreateProductOption_ShouldReturnNotFoundIfProductNotExisted()
            {
                var productId = Guid.NewGuid();

                string name = Guid.NewGuid().ToString();
                var productOption = new Repository.ProductOption
                {
                    ProductId = productId,
                    Name = name,
                    Description = "DESC",
                };
                var response = await _productController.CreateOption(productId, productOption);
                ObjectContent content = response.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                Assert.AreEqual("Product not existed in DB", content.Value.ToString());
            }

            [TestMethod]
            public async Task UpdateProductOption_ShouldChangeOptionIntoDb()
            {
                ProductOption testProductOption;
                var product = new Repository.Product
                {
                    Name = "testName",
                    Description = "testDesc",
                    Price = 356,
                    DeliveryPrice = 13
                };

                product.Id = await _productRepository.CreateProduct(product);


                var productOption = new Repository.ProductOption
                {
                    ProductId = product.Id,
                    Name = "testOption",
                    Description = "desc"
                };

                productOption.Id = await _productRepository.CreateProductOption(productOption);
                testProductOption = productOption;
                testProductOption.Name = Guid.NewGuid().ToString();
            
                var response = await _productController.UpdateOption(productOption.Id, testProductOption);

                var responseGet = await _productController.GetOption(testProductOption.Id);
                ObjectContent content = responseGet.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                Assert.AreEqual(testProductOption.Name, ((ProductOption)content.Value).Name);
                Assert.AreEqual(testProductOption.Description, ((ProductOption)content.Value).Description);
            }

            [TestMethod]
            public async Task DeleteProductOption_ShouldRemoveOptionFromDb()
            {
                ProductOption testProductOption;
                var product = new Repository.Product
                {
                    Name = "testName",
                    Description = "testDesc",
                    Price = 356,
                    DeliveryPrice = 13
                };

                product.Id = await _productRepository.CreateProduct(product);


                var productOption = new Repository.ProductOption
                {
                    ProductId = product.Id,
                    Name = "testOption",
                    Description = "desc"
                };

                productOption.Id = await _productRepository.CreateProductOption(productOption);
                testProductOption = productOption;

                var beforeDeletingResponse = await _productController.GetOption(testProductOption.Id);
                ObjectContent content = beforeDeletingResponse.Content as ObjectContent;

                Assert.IsNotNull((ProductOption)content.Value);

                var response = await _productController.DeleteOption(testProductOption.Id);

                var responseGet = await _productController.GetOption(testProductOption.Id);
                content = responseGet.Content as ObjectContent;

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.IsNull((ProductOption)content.Value);
            }

        #endregion

    }
}
