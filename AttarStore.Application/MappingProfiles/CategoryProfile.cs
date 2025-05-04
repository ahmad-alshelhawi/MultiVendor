using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AutoMapper;

namespace AttarStore.Api.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            // Category
            CreateMap<Category, CategoryMapperView>();
            CreateMap<CategoryMapperCreate, Category>();
            CreateMap<CategoryMapperUpdate, Category>()
                .ForAllMembers(opts => opts.Condition((src, _, srcMember) => srcMember != null));

            // Subcategory
            CreateMap<Subcategory, SubcategoryMapperView>();
            CreateMap<SubcategoryMapperCreate, Subcategory>();
            CreateMap<SubcategoryMapperUpdate, Subcategory>()
                .ForAllMembers(opts => opts.Condition((src, _, srcMember) => srcMember != null));
        }
    }
}
