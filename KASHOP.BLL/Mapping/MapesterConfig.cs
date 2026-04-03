using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace KASHOP.BLL.Mapping
{
    using KASHOP.DAL.DTO.Request;
    using Mapster;
    using System.Globalization;

    public static class MapsterConfig
    {
        public static void MapesterConfigRegister(IFileService fileService)
        {
            TypeAdapterConfig<Category, CategoryResponse>.NewConfig()
                .Map(des => des.Category_Id, src => src.Id)
                .Map(des => des.UserCreated, src => src.CreatedBy.UserName)
                .Map(des => des.Name, src => src.Translations
                    .Where(t => t.Language == CultureInfo.CurrentCulture.Name)
                    .Select(t => t.Name).FirstOrDefault());

            TypeAdapterConfig<Product, ProductResponse>.NewConfig()
                .Map(des => des.UserCreated, src => src.CreatedBy.UserName)
                .Map(des => des.Name, src => src.Translations
                    .Where(t => t.Language == CultureInfo.CurrentCulture.Name)
                    .Select(n => n.Name).FirstOrDefault())
                .Map(dest => dest.MainImage, src => fileService.GetImageUrl(src.MainImage));

            TypeAdapterConfig<ProductUpdateRequest, Product>.NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<Brand, BrandResponse>.NewConfig()
                 .Map(dest => dest.Brand_Id, src => src.Id)
                 .Map(dest => dest.Logo,  src => fileService.GetImageUrl(src.Logo))
                 .Map(dest => dest.Name, src => src.Translations
                    .Where(t => t.Language == CultureInfo.CurrentCulture.Name)
                    .Select(t => t.Name)
                 .FirstOrDefault());
        }
    }

}
