using KASHOP.DAL.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Response
{
    public class CategoryResponse
    {
        public List<CategoryTranslarionResponse> Translations { get; set; }
    }
}
