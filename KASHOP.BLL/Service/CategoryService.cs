using Azure.Core;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<CategoryTranslarionResponse?> UpdateCategory(int id, UpdateCategoryTranslationRequest request)
        {
            var category = await _categoryRepository.GetOne(
                c => c.Id == id,
                new string[] { nameof(Category.Translations) } 
            );

            if (category == null) return null;

            foreach (var tran in category.Translations)
            {
                if (tran.Language == request.Language)
                {
                    tran.Name = request.Name;          
                    await _categoryRepository.UpdateAsync(category); 
                    return tran.Adapt<CategoryTranslarionResponse>(); 
                }
            }

            return null; // اللغة غير موجودة        
        }
        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetOne(c => c.Id == id);
            if (category == null) return false;
            return  await _categoryRepository.DeleteAsync(category);
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync(new string[] { nameof(Category.Translations) , nameof(Category.CreatedBy)});
            return categories.Adapt<List<CategoryResponse>>();
        }

        public async Task<CategoryResponse?> GetCategoriesAsync(Expression<Func<Category,bool>> filter)
        {
            var categories = await _categoryRepository.GetOne(filter,new string[] { nameof(Category.Translations) });
            return categories.Adapt<CategoryResponse>();
        }

       
    }
}
