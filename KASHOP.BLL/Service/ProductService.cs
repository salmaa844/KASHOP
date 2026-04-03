using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using Mapster;
using System.Linq.Expressions;


namespace KASHOP.BLL.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public ProductService(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task CreateProduct(ProductRequest request)
        {
            var product = request.Adapt<Product>();
            if (request.MainImage != null)
            {
                var imagePath = await _fileService.UploadeAsync(request.MainImage);
                product.MainImage = imagePath;
            }
            await _productRepository.CreateAsync(product);
        }

        public async Task<List<ProductResponse>> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync(new string[]
            {
                nameof(Product.Translations),
                nameof(Product.CreatedBy)
            });
            return products.Adapt<List<ProductResponse>>();
        }
        public async Task<ProductResponse> GetProduct(Expression<Func<Product,bool>> filter)
        {
            var product = await _productRepository.GetOne(filter, new string[]
            {
                nameof(Product.Translations),
                nameof(Product.CreatedBy)
            });
            if (product == null) return null;
            
            return product.Adapt<ProductResponse>();

        }
    
        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _productRepository.GetOne(c => c.Id == id);
            if (product == null) return false;

            _fileService.Delete(product.MainImage);

            return await _productRepository.DeleteAsync(product);

        }

        public async Task<bool> UpdateProductAsync(int id, ProductUpdateRequest request)
        {
            var product = await _productRepository.GetOne(p => p.Id == id,new string[]
            {
                nameof(Product.Translations)
               
            });
            if (product == null) return false;

            request.Adapt(product);

            if (request.Translations != null)
            { 
                foreach (var translationRequest in request.Translations)
                {
                    var existing = product.Translations.FirstOrDefault(T=>T.Language == translationRequest.Language);
                    if (existing != null)
                    {
                        if (translationRequest.Name == existing.Name)
                        {
                            existing.Name = translationRequest.Name;
                        }
                        if (translationRequest.Description == existing.Description)
                        {
                            existing.Description = translationRequest.Description;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }

            }

            if (request.MainImage != null)
            {
                _fileService.Delete(product.MainImage);
                product.MainImage = await _fileService.UploadeAsync(request.MainImage);
            }

            return await _productRepository.UpdateAsync(product);
        }

    }
    }

