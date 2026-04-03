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
        public int Category_Id { get; set; }

        public string UserCreated { get; set; } 
        //public List<CategoryTranslarionResponse> Translations { get; set; }
        
        public string Name { get; set; }
    }
}
