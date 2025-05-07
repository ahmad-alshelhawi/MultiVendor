using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos.Catalog
{
    public class ProductImageUploadDto
    {
        [Required] public IFormFile File { get; set; }
    }
    public class ProductImageViewDto
    {
        public string Url { get; set; }

    }
    public class ProductVariantImageUploadDto
    {
        [Required] public IFormFile File { get; set; }
    }


    public class ProductVariantImageViewDto
    {
        public string Url { get; set; }

    }
}
