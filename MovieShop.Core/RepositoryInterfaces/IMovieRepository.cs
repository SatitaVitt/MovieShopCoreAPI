﻿using MovieShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Core.RepositoryInterfaces
{
    public interface IMovieRepository : IAsyncRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetTopRevenueMovie();
        Task<IEnumerable<Movie>> GetTopGrossingMovies();//?
        Task<IEnumerable<Movie>> GetMoviesByCast(int castId);

        Task<IEnumerable<Movie>> GetMoviesByGenre(int genreId);
        Task<IEnumerable<Movie>> GetMoviesByTitle(string title, int pageIndex);

        Task<IEnumerable<Movie>> GetTopRatedMovies();

        Task<IEnumerable<Review>> GetMovieReviews(int id);

    }
}
