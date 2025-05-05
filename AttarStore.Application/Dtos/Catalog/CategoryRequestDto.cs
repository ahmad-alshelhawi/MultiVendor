using AttarStore.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos.Catalog
{
    public class CategoryRequestCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
    public class CategoryRequestUpdateStatusDto
    {
        [Required]
        public RequestStatus Status { get; set; }
    }
}
