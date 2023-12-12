using AutoMapper;
using MovieLibraryEntities.Models;
using MovieLibraryOO.Dto;

namespace MovieLibraryOO.Mappers
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, MovieDto>();
            
        }
    }
}
