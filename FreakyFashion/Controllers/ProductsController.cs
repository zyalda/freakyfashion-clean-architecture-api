using ApplicationLayer.Dto;
using ApplicationLayer.IServices;
using ApplicationLayer.IStorageContainerServices;
using FreakyFashion.PaginationDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FreakyFashion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<DtoProduct> logger;
        private readonly IAzureBlobService azureBlobService;
        private readonly ICloudAssetResolver cloudAssetResolver;
        private readonly IProductService productService;
        public ProductsController(ILogger<DtoProduct> logger, IProductService productService, IAzureBlobService azureBlobService, ICloudAssetResolver cloudAssetResolver)
        {
            this.logger = logger;
            this.azureBlobService = azureBlobService;
            this.productService = productService;
            this.cloudAssetResolver = cloudAssetResolver;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<DtoProduct>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResponse<DtoProduct>>> GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                int currentPage = page < 1 ? 1 : page;
                int currentSize = pageSize < 1 ? 12 : pageSize;

                var response = productService.GetProducts();

                if (response == null)
                    return Ok(new List<DtoProduct>());

                var productList = response.ToList()
                    .Skip((currentPage - 1) * currentSize)
                    .Take(currentSize)
                    .ToList();

                    await cloudAssetResolver.ResolveCollectionAsync(productList);

                var ProductsPagintionModel = new PagedResponse<DtoProduct>
                {
                    EntitiesDto = productList,
                    CurrentPage = currentPage,
                    PageSize = currentSize,
                    TotalRecords = response.Count()
                };
                return Ok(ProductsPagintionModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error caught inside {nameof(GetAllProducts)} endpoint flow.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProductById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoProduct))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DtoProduct>> GetProductById([FromRoute] int id)
        {
            try
            {
                var response = productService.GetProductById(id);
                if (response == null)
                {
                    logger.LogInformation($"The product with id: {id} not found. In {nameof(GetProductById)}");
                    return Ok(new DtoProduct { StatusMessage = $"404 The product with id: {id} not found." });
                }

                await cloudAssetResolver.ResolveSingleAsync(response);
               
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(productService)} in {nameof(GetProductById)}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a single specific product details profile by its unique URL slug text identifier.
        /// </summary>
        /// <param name="slug">The textual unique slug nickname of the specific product item (e.g. 'svart-tshirt').</param>
        [HttpGet]
        [Route("GetAllProductBySlug")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoProduct))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DtoProduct>> GetProductBySlug([FromQuery] string slug)
        {
            try
            {
               if (string.IsNullOrWhiteSpace(slug))
                {
                    return BadRequest(new { Message = "A valid product URL slug parameter string is required." });
                }

                DtoProduct product = productService.GetProductBySlug(slug);

                if (product == null)
                {
                    logger.LogInformation($"Product item matching the unique slug target '{slug}' was not found.");
                    return Ok(new DtoProduct { StatusMessage = $"The product with slug profile '{slug}' was not found." });
                }

                await cloudAssetResolver.ResolveSingleAsync(product);

                return Ok(product);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error inside {nameof(GetProductBySlug)} routine for product item reference: {slug}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddProduct")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoProduct))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DtoProduct>> AddProduct([FromForm] ProductUploadForm request)
        {
            try {
                if (request.ImageFile == null || request.ImageFile.Length == 0)
                    return BadRequest("No file uploaded.");

                var product = productService.AddProduct(request);
                
                if(product.IsAdded == true)
                {
                    DtoBlob uploadResult = await azureBlobService.UploadBlobAsync(request.ImageFile);

                    product.StatusMessage = $"Makeup product and image {uploadResult.Uri} successfully processed!";
                }
                return StatusCode(StatusCodes.Status201Created, product);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(productService)} in {nameof(AddProduct)}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Performs a highly efficient partial update on an existing product's properties using JSON Patch.
        /// </summary>
        [HttpPatch]
        [Route("UpdateProduct/{id}")]
        [Authorize]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoProduct))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult UpdateProduct([FromRoute] int id, [FromBody] JsonPatchDocument<DtoProduct> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    logger.LogError("Invalid input value.");
                    return BadRequest(new { Message = "Invalid patch document payload." });
                }

                var product = productService.GetProductById(id);

                if (product == null)
                {
                    logger.LogInformation($"The product with ID: {id} was not found.");

                    return NotFound(new { Message = $"The product with ID: {id} was not found." });
                }

                patchDoc.ApplyTo(product, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedProduct = productService.UpdateProduct(product);

                if (updatedProduct == null)
                {
                    return BadRequest(new { Message = "Invalid updating. Check your data values." });
                }

                logger.LogInformation($"The {updatedProduct.Name} in {nameof(productService)} in {nameof(UpdateProduct)} is updated.");

                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(productService)} in {nameof(UpdateProduct)}");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a specific category from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique integer ID of the category to remove.</param>
        [HttpDelete]
        [Route("DeleteProduct/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
        {
            try
            {
                var product = productService.GetProductById(id);
                if (product == null)
                {
                    logger.LogError("The product not found.");
                    return Ok("404 The product not found.");
                }
                
                productService.DeleteProduct(product);
                return Ok("200");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(productService)} in {nameof(DeleteProduct)}");
                return BadRequest(ex.Message);
            }
        }

        //In next step could call this endpoint to review an image by name.
        //To be done as summer project + Azure functions to manage orders.
        public async Task<IActionResult> GetProductImage(string fileName)
        {
            DtoBlob blobData = await azureBlobService.DownloadBlobAsync(fileName);

            if (blobData.Stream == null)
            {
                return NotFound(new { State = $"Image '{fileName}' was not found in storage." });
            }

            // Stream the raw byte stream directly into the browser using the correct file format (e.g. "image/jpeg")
            return File(blobData.Stream, blobData.ContentType ?? "image/jpeg");
        }
    }
}
