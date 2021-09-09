using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using AutoMapper;

namespace DamaAdmin.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
        }
    }
}
