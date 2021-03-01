using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skinet.Core.Entities;
using Skinet.Core.Interfaces;
using Skinet.Core.Specifications;
using Skinet.WebApi.Dtos;
using Skinet.WebApi.Errors;
using Skinet.WebApi.Helpers;

namespace Skinet.WebApi.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductBrand> _productBrandRepository;
        private readonly IRepository<ProductType> _productTypeRepository;
        private readonly IMapper _mapper;

        public ProductsController(IRepository<Product> productRepository, IRepository<ProductBrand> productBrandRepository, IRepository<ProductType> productTypeRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _productBrandRepository = productBrandRepository;
            _productTypeRepository = productTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery] ProductSpecParams productParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

            var countSpec = new ProductWithFiltersForCountSpecification(productParams);

            var totalItems = await _productRepository.CountAsync(countSpec);

            var products = await _productRepository.ListAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products);

            return Ok(new Pagination<ProductDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productRepository.GetEntityWithSpecAsync(spec);

            if (product == null) return NotFound(new ApiResponse(404));

            return _mapper.Map<Product, ProductDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepository.GetAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _productTypeRepository.GetAllAsync());
        }
    }
}
