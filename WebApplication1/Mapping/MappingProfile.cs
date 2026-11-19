using AutoMapper;
using WebApplication1.Contracts;
using Cinema.Business.Dtos;
using Cinema.Domain.Entities;

namespace WebApplication1.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieResponse>();

            CreateMap<CreateMovieRequest, Movie>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Comments, o => o.Ignore())
                .ForMember(d => d.Sessions, o => o.Ignore());

            CreateMap<UpdateMovieRequest, Movie>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Comments, o => o.Ignore())
                .ForMember(d => d.Sessions, o => o.Ignore());

            CreateMap<MovieSearchRequest, MovieSearchDto>();

            CreateMap<MovieComment, MovieCommentResponse>();

            CreateMap<CreateMovieCommentRequest, MovieComment>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.MovieId, o => o.Ignore())
                .ForMember(d => d.Movie, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateMovieCommentRequest, MovieComment>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.MovieId, o => o.Ignore())
                .ForMember(d => d.Movie, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());

            CreateMap<MovieSession, MovieSessionResponse>()
                .ForMember(d => d.MovieTitle, o => o.MapFrom(s => s.Movie.Title))
                .ForMember(d => d.StartsAt, o => o.MapFrom(s => DateTime.SpecifyKind(s.StartsAt, DateTimeKind.Utc)));

            CreateMap<CreateMovieSessionRequest, MovieSession>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Movie, o => o.Ignore());

            CreateMap<UpdateMovieSessionRequest, MovieSession>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Movie, o => o.Ignore());

            CreateMap<IdentityLoginRequest, LoginDto>().ReverseMap();

            CreateMap<User, UserResponse>();

            CreateMap<IdentityRegisterRequest, User>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
