using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using WebApi.Dtos;
using AutoMapper;
using WebApi.Errors;
using Microsoft.AspNetCore.Http;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    public class ProductsController:BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        
        public ProductsController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<IReadOnlyList<ProductToReturnDto>>>> GetProducts(
           [FromQuery]ProductSpecParams productParams)
        {
            var spec=new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpec=new ProductWithFiltersForCountSpecification(productParams);
            var totalItems=await _unitOfWork.Repository<Product>().CountAsync(countSpec);
            var products= await _unitOfWork.Repository<Product>().ListAsync(spec);
            var data=_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
    

            return Ok(new Pagination<ProductToReturnDto>(productParams.pageIndex,productParams.PageSize,
                totalItems,data));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id )
        {
            var spec=new ProductsWithTypesAndBrandsSpecification(id);
            var product=  await _unitOfWork.Repository<Product>().GetEntityWithSpec(spec);
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }
            return _mapper.Map<Product, ProductToReturnDto>(product);
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _unitOfWork.Repository<ProductBrand>().ListAllAsync());        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _unitOfWork.Repository<ProductType>().ListAllAsync());
        }
        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetAllProducts()
        {
            var spec=new ProductsWithTypesAndBrandsSpecification();
            var products= await _unitOfWork.Repository<Product>().ListAsync(spec);
            var data=_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
            return Ok(data);
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(ProductToCreateDto productToCreateDto)
        {
            var product= _mapper.Map<ProductToCreateDto,Product>(productToCreateDto);
            product.PictureUrl ="images/products/test.png";
            _unitOfWork.Repository<Product>().Add(product);
            var result=await _unitOfWork.Complete();
            if(result<=0) return BadRequest(new ApiResponse(400,"Problem creating Product"));
            return Ok(product);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id,ProductToCreateDto productToUpdate)
        {
            var product=await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            productToUpdate.pictureUrl=product.PictureUrl;
            _mapper.Map(productToUpdate,product);
            _unitOfWork.Repository<Product>().Update(product);
            var result=await _unitOfWork.Complete();
            if(result<=0) return BadRequest(new ApiResponse(400,"Problem updating Product"));
            return Ok(product);
        }



        [HttpDelete("delete/{id}")]
        public async Task DeleteProduct(int id)
        {
            var product= await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            _unitOfWork.Repository<Product>().Delete(product);
            _unitOfWork.Complete();
        }
    }
}
