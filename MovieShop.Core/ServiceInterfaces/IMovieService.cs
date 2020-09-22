using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entities;
using MovieShop.Core.Helpers;
using MovieShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Core.ServiceInterfaces
{
    public interface IMovieService
    {        
        Task<IEnumerable<Movie>> GetTopGrossingMovies();
        Task<Movie> GetMovieById(int id);
        Task<IEnumerable<Movie>> GetMoviesByCast(int castId);
        Task<IEnumerable<Movie>> GetMoviesByGenre(int genreId);

        //Task<IEnumerable<Movie>> GetMoviesByGenreId(int genreId);

        //Task<IEnumerable<Movie>> GetMoviesForCast(int castId);

        Task<PagedResultSet<MovieResponseModel>> GetMoviesByPagination(int pageSize = 20, int page = 0, string title = "");
    }
}
