using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        //  
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(string? sortBy, int? brandId, int? typeId, string? searchTerm, int pageNumber = 1, int pageSize = 6)
        {
            var queryable = await _productRepository.GetProductsAsync(sortBy, brandId, typeId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryable = queryable.Where(p => !string.IsNullOrWhiteSpace(p.Name) && p.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
            }


            var totalItems = queryable.Count();

            var products = queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var mappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return new Pagination<ProductToReturnDto>(mappedProducts, totalItems, pageNumber, pageSize);
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }
            var mappedProduct = _mapper.Map<Product, ProductToReturnDto>(product);
            return mappedProduct;
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productRepository.GetProductBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProuctTypes()
        {
            return Ok(await _productRepository.GetProductTypesAsync());
        }
    }
}