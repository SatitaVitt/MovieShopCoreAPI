using Microsoft.EntityFrameworkCore;
using MovieShop.Core.Entities;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Repositories
{
    public class MovieRepository : EfRepository<Movie>, IMovieRepository
    {
        public MovieRepository(MovieShopDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Movie>> GetMoviesByCast(int castId)
        {
            var movies = await _dbContext.MovieCasts.Where(mc => mc.CastId == castId).Include(c => c.Movie).Select(m => m.Movie).ToListAsync();
            return movies;
        }

        public async Task<IEnumerable<Movie>> GetTopRevenueMovie()
        {
            var movie = await _dbContext.Movies.OrderByDescending(m => m.Revenue).Take(50).ToListAsync();
            return movie;
        }
        public async Task<IEnumerable<Movie>> GetMoviesByGenre(int genreId)
        {
            var movies = await _dbContext.MovieGenres.Where(mc => mc.GenreId == genreId).Include(c => c.Movie).Select(m => m.Movie).ToListAsync();
            return movies;
        }


    }
}
