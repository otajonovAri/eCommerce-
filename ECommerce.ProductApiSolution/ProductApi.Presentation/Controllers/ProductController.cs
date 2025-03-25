using EC.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            // Get all Product from repo
            var products = await productInterface.GetAllAsync();
            if(!products.Any())
                return NotFound("No products found");

            // convert data from entity to DTO and return 
            var (x, list) = ProductConversions.FromEntity(null, products);
            return list.Any() ? Ok(list) : NotFound("No products found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            // Get single Product from the Repo
            var product = await productInterface.FindByIdAsync(id);
            if(product is null)
                return NotFound("Product not found");

            // convert from entity to DTO and return
            var (x, y) = ProductConversions.FromEntity(product , null);
            return x is not null ? Ok(x) : NotFound("Product not found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDto product)
        {
            // check model state is all data annotations are passed
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            // converter to Entity and Return
            var getEntity = ProductConversions.ToEntity(product);
            var response  = await productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDto product)
        {
            // check model state is all data annotations are passed
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            // convert to entity 
            var getEntity = ProductConversions.ToEntity(product);
            var response  = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDto product)
        {
            
            // convert to entity 
            var getEntity = ProductConversions.ToEntity(product);
            var response  = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
