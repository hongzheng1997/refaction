using System;
using System.Web.Http;
using Repository;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace refactor_me.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {

        #region Private fields

            private readonly ProductData _productRepository;

        #endregion


        #region Constructor

            public ProductsController()
            {
                var conn = ConfigurationManager.ConnectionStrings["dbConnection"].ToString();
                _productRepository = new ProductData(conn);
            }

        #endregion


        #region Public methods

            [Route]
            [HttpGet]
            public async Task<HttpResponseMessage>  GetAll()
            {
                try
                {
                    var products = await _productRepository.GetAllProducts();
                    return Request.CreateResponse(HttpStatusCode.OK, products);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }                
            }

            [Route]
            [HttpGet]
            public async Task<HttpResponseMessage> SearchByName(string name)
            {          
                try
                {
                    var products = await _productRepository.SearchProductsByName(name);
                    if(products == null || products.Count == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, products);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, products);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            [Route("{id}")]
            [HttpGet]
            public async Task<HttpResponseMessage> GetProduct(Guid id)
            {
                try
                {
                    var product = await _productRepository.GetProductById(id);
                    if (product == null)
                    {
                       return  Request.CreateResponse(HttpStatusCode.NotFound, product);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, product);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            [Route]
            [HttpPost]
            public async Task<HttpResponseMessage> Create( Repository.Product product)
            {
                try
                {
                    var newId = await _productRepository.CreateProduct(product);
                    return Request.CreateResponse(HttpStatusCode.OK, newId);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            [Route("{id}")]
            [HttpPut]
            public async Task<HttpResponseMessage> Update(Repository.Product product)
            {
                try
                {
                    var existedProduct = await _productRepository.GetProductById(product.Id);

                    if (existedProduct == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                    await _productRepository.UpdateProduct(product);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            [Route("{id}")]
            [HttpDelete]
            public async Task<HttpResponseMessage> Delete(Guid id)
            {
                try
                {
                    var existedProduct = await _productRepository.GetProductById(id);
                    if (existedProduct == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                    await _productRepository.DeleteProuct(id);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            [Route("{productId}/options")]
            [HttpGet]
            public async Task<HttpResponseMessage> GetOptions(Guid productId)
            {
                try
                {
                    var productOptions = await _productRepository.GetProductOptionsByProductId(productId);

                    if (productOptions == null || productOptions.Count == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, productOptions);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, productOptions);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            [Route("{productId}/options/{id}")]
            [HttpGet]
            public async Task<HttpResponseMessage> GetOption(Guid id)
            {
                try
                {
                    var productOption = await _productRepository.GetProductOption(id);

                    if (productOption == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, productOption);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, productOption);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            [Route("{productId}/options")]
            [HttpPost]
            public async Task<HttpResponseMessage> CreateOption(Guid productId, Repository.ProductOption option)
            {
                try
                {
                    var product = await _productRepository.GetProductById(productId);

                    if (product == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Product not existed in DB");
                    }

                    option.ProductId = productId;
                    var newId = await _productRepository.CreateProductOption(option);
                    return Request.CreateResponse(HttpStatusCode.OK, newId);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }            
            }

            [Route("{productId}/options/{id}")]
            [HttpPut]
            public async Task<HttpResponseMessage> UpdateOption(Guid id, Repository.ProductOption option)
            {
                option.Id = id;
                try
                {
                    var existedProductOption = await _productRepository.GetProductOption(id);

                    if (existedProductOption == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                    await _productRepository.UpdateProductOption(option);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            [Route("{productId}/options/{id}")]
            [HttpDelete]
            public async Task<HttpResponseMessage> DeleteOption(Guid id)
            {
                try
                {
                    var existedProductOption = await _productRepository.GetProductOption(id);

                    if (existedProductOption == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                    await _productRepository.DeleteProductOption(id);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

        #endregion
    }
}
