using AutoMapper;

namespace MyRoverServiceAPI.Mappings
{
    public class RoverMappingProfile : Profile
    {
        public RoverMappingProfile()
        {
            CreateMap<MarsRoverEarthDayPhotos, MyRoverEarthDayViewModel>()
                .ForMember(m => m.ImageLinks, mr => mr.MapFrom(mre => mre.ImageFileNames))
                .ReverseMap();
        }

    }
}
