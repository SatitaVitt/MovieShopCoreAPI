﻿using Microsoft.EntityFrameworkCore;
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
        public override async Task<Movie> GetByIdAsync(int id)
        {
            var movie = await _dbContext.Movies.Where(m=>m.Id == id)
                        .Include(m => m.MovieCasts).ThenInclude(mc => mc.Cast)
                        .Include(m => m.MovieGenres)
                        .ThenInclude(mg => mg.Genre)
                        .FirstOrDefaultAsync();
            //var movie = await _dbContext.Set<Movie>().Where(m=>m.Id==id)
            //                //.Include(m=>m.MovieGenres).ThenInclude(mg=>mg.Genre)
            //                //.Include(m=>m.MovieCasts).ThenInclude(mc=>mc.Cast)
            //                .FirstOrDefaultAsync();
            if (movie == null) return null;
            var movieRating = await _dbContext.Reviews.Where(r => r.MovieId == id)
                                .AverageAsync(r=>r.Rating);
            if (movieRating > 0) movie.Rating = movieRating;
            return movie;
        }
        public Task<IEnumerable<Review>> GetMovieReviews(int id)
        {
            throw new NotImplementedException();
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

        public async Task<IEnumerable<Movie>> GetMoviesByTitle(string title, int pageIndex)
        {
            var movies = await _dbContext.Movies.Where(m => m.Title.StartsWith(title)).OrderBy(m => m.Title).Skip(20 * pageIndex).Take(20).ToListAsync();
            return movies;
        }

        public async Task<IEnumerable<Movie>> GetTopRatedMovies()
        {
            var movies = await _dbContext.Movies.OrderByDescending(m => m.Reviews.Average(r => r.Rating)).Take(50).ToListAsync();
            return movies;
        }


    }
}
