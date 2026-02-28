using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryRequeste> CreateCategoryAsync(CategoryRequeste requeste)
        {
            var category = requeste.Adapt<Category>();
            await _categoryRepository.CreateAsync(category);
            return category.Adapt<CategoryRequeste>();
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync( new string[] { nameof(Category.Translations) });
            return categories.Adapt<List<CategoryResponse>>();
        }
    }
}
