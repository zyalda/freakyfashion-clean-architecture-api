using ApplicationLayer.Dto;
using ApplicationLayer.IServices;
using ApplicationLayer.IStorageContainerServices;
using FreakyFashion.PaginationDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FreakyFashion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   public class CategoryController : ControllerBase
    {
        private readonly ILogger<DtoCategory> logger;
        private readonly IAzureBlobService azureBlobService;
        private readonly ICloudAssetResolver cloudAssetResolver;
        private readonly ICategoryService categoryService;
        public CategoryController(ILogger<DtoCategory> logger, ICategoryService categoryService, IAzureBlobService azureBlobService, ICloudAssetResolver cloudAssetResolver)
        {
            this.logger = logger;
            this.azureBlobService = azureBlobService;
            this.categoryService = categoryService;
            this.cloudAssetResolver = cloudAssetResolver;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<CategoryWithProductsDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CategoryWithProductsDto>>> GetAllCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                int currentPage = page < 1 ? 1 : page;
                int currentSize = pageSize < 1 ? 12 : pageSize;

                var response = categoryService.GetCategories();

                if(response == null)
                    return Ok(new List<CategoryWithProductsDto>());
                
                var categoryList = response
                    .Skip((currentPage - 1) * currentSize)
                    .Take(currentSize)
                    .ToList();

                    var categories = categoryList.Select(x => x.Category);
                    await cloudAssetResolver.ResolveCollectionAsync(categories);

                    var allChildProducts = categoryList.SelectMany(c => c.Products);
                    await cloudAssetResolver.ResolveCollectionAsync(allChildProducts);

                var categoriesPagintionModel = new PagedResponse<CategoryWithProductsDto>
                {
                    EntitiesDto = categoryList,
                    CurrentPage = currentPage,
                    PageSize = currentSize,
                    TotalRecords = response.Count()
                };
                return Ok(categoriesPagintionModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error caught inside {nameof(GetAllCategories)} endpoint flow.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific category along with all its associated products by ID.
        /// </summary>
        /// <param name="id">The unique integer identifier of the category.</param>
        [HttpGet]
        [Route("GetCategoryById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryWithProductsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryWithProductsDto>> GetCategoryById([FromRoute] int id)
        {
            try
            {
                var response = categoryService.GetCategoryById(id);

                if (response == null)
                {
                    logger.LogInformation($"The category with id: {id} not found. In {nameof(GetCategoryById)}");
                    return Ok($"404 The category with id: {id} not found.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(categoryService)} in {nameof(GetCategoryById)}");
                return BadRequest(ex.Message);
            }
        }

            /// <summary>
            /// Retrieves a specific category along with all its associated products by its unique URL slug text.
            /// </summary>
            /// <param name="urlSlug">The unique string URL text nickname of the category.</param>
            [HttpGet]
            [Route("GetCategoryBySlug/{urlSlug}")]
            [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryWithProductsDto))]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<ActionResult<CategoryWithProductsDto>> GetCategoryBySlug([FromRoute] string urlSlug)
            {
                try
            {
                if (string.IsNullOrWhiteSpace(urlSlug))
                {
                    logger.LogError($"Slug cannot be empty in {nameof(categoryService)} in {nameof(GetCategoryBySlug)}");
                    return BadRequest("Slug cannot be empty.");
                }

                var response = categoryService.GetCategoryBySlug(urlSlug);

                if (response == null)
                {
                    logger.LogInformation($"The category with slug {urlSlug} not found. In {nameof(GetCategoryBySlug)}");
                    
                    return Ok($"The category with slug {urlSlug} not found.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(categoryService)} in {nameof(GetCategoryBySlug)}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddCategory")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoCategory))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DtoCategory>> AddCategory([FromForm] CategoryUploadForm request)
        {
            try
            {
                if (request.ImageFile == null || request.ImageFile.Length == 0)
                    return BadRequest("No file uploaded.");

                var category = categoryService.AddCategory(request);

                // Stream the category banner up to storage container
                DtoBlob uploadResult = await azureBlobService.UploadBlobAsync(request.ImageFile);

                category.StatusMessage = $"Category created successfully! and image {uploadResult.Uri} is loaded";

                return Ok(category);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(categoryService)} in {nameof(AddCategory)}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Performs a highly efficient partial update on an existing category's properties using JSON Patch.
        /// </summary>
        [HttpPatch]
        [Route("UpdateCategory/{id}")]
        [Authorize]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoCategory))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult UpdateCategory([FromRoute] int id, [FromBody] JsonPatchDocument<DtoCategory> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    logger.LogError("Invalid input value.");
                    return BadRequest(new { Message = "A valid JSON patch document payload is required." });
                }

                var response = categoryService.GetCategoryById(id);
                if (response == null)
                {
                    logger.LogError($"Category tracking profile with ID: {id} was not found.");

                    return NotFound(new { Message = $"Category tracking profile with ID: {id} was not found." });
                }

                patchDoc.ApplyTo(response.Category, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedCategory = categoryService.UpdateCategory(response.Category);
                if (updatedCategory == null)
                {
                    logger.LogWarning($"Update failed: The target UrlSlug constraint already exists in database context.");

                    return Conflict(new { Message = "There is another category with the same UrlSlug. Choose another name." });
                }

                logger.LogInformation($"The {updatedCategory.Name} in {nameof(categoryService)} in {nameof(UpdateCategory)} is updated.");

                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error inside {nameof(categoryService)} during {nameof(UpdateCategory)} execution routing.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a specific category from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique integer ID of the category to remove.</param>
        [HttpDelete]
        [Route("DeleteCategory/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteCategory([FromRoute] int id)
        {
            try
            {
                var response = categoryService.GetCategoryById(id);

                if (response == null)
                {
                    logger.LogError($"The category with id {id} not found.");
                    return Ok($"404 The category with id {id} not found.");
                }
                if(response.Products.Count() > 0)
                {
                    logger.LogError($"The category with id {id} still has products related to.");
                    return Ok($"404 The category with id {id} still has products related to.");
                }

                categoryService.DeleteCategory(response.Category);
                return Ok($"The category {response.Category.Name} is deleted.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in {nameof(categoryService)} in {nameof(DeleteCategory)}");
                return BadRequest(ex.Message);
            }
        }
    }
}
