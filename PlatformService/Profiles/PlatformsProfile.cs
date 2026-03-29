using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {

            // Source ---> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Platform, PlatformCreateDto>();
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto,PlatformPublishedDto>();
        }
    }
}
