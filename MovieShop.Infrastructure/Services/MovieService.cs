﻿using MovieShop.Core.Entities;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Helpers;
using System.Linq;
using System.Linq.Expressions;

namespace MovieShop.Infrastructure.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
        }
        public async Task<MovieDetailsResponseModel> GetMovieById(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            var casts = new List<CastResponseModel>();
            foreach (var mc in movie.MovieCasts)
            {
                var castResponse = new CastResponseModel
                {
                    Id = mc.Cast.Id,
                    Name = mc.Cast.Name,
                    Gender = mc.Cast.Gender,
                    ProfilePath = mc.Cast.ProfilePath,
                    TmdbUrl = mc.Cast.TmdbUrl,
                    Character = mc.Character
                };
                casts.Add(castResponse);
            }
            var genres = new List<Genre>();
            foreach (var mg in movie.MovieGenres)
            {
                var genre = new Genre
                {
                    Id = mg.Genre.Id,
                    Name = mg.Genre.Name
                };
                genres.Add(genre);
            }
            var movieDetails = new MovieDetailsResponseModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Overview = movie.Overview,
                Tagline = movie.Tagline,
                Revenue = movie.Revenue,
                Budget = movie.Budget,
                ImdbUrl = movie.ImdbUrl,
                TmdbUrl = movie.TmdbUrl,
                BackdropUrl = movie.BackdropUrl,
                PosterUrl = movie.PosterUrl,
                OriginalLanguage = movie.OriginalLanguage,
                ReleaseDate = movie.ReleaseDate,
                RunTime = movie.RunTime,
                Price = movie.Price,
                Casts = casts,
                Genres = genres
            };
            return movieDetails;
        }

        public async Task<IEnumerable<Movie>> GetTopGrossingMovies()
        {
            var movies = await _movieRepository.GetTopRevenueMovie();
            return movies;
        }

        public async Task<Movie> GetMovieById(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetMoviesByCast(int castId)
        {
            var movies = await _movieRepository.GetMoviesByCast(castId);
            return movies;
        }
        public async Task<IEnumerable<Movie>> GetMoviesByGenre(int genreId)
        {
            var movies = await _movieRepository.GetMoviesByGenre(genreId);
            return movies;
        }
        public async Task<PagedResultSet<MovieResponseModel>> GetMoviesByPagination(int pageSize = 20, int page = 0, string title = "")
        {
            Expression<Func<Movie, bool>> filterExpression = null;

            //contains translate translated to sql like
            if (!string.IsNullOrEmpty(title))
            {
                filterExpression = movie => title != null && movie.Title.Contains(title);
            }

            var pagedMovies = await _movieRepository.GetPagedData(page, pageSize, movies => movies.OrderBy(m => m.Title), filterExpression);

            var pagedMovieResponseModel = _mapper.Map<List<MovieResponseModel>>(pagedMovies);

            var movies = new PagedResultSet<MovieResponseModel>(pagedMovieResponseModel, page, pageSize, pagedMovies.TotalCount);
            return movies;
        }

    }
}
