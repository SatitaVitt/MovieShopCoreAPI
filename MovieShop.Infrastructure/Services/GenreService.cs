using MovieShop.Core.Entities;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Services
{
    public class GenreService : IGenreService
    {
        private readonly IAsyncRepository<Genre> _genreRepository;
        public GenreService(IAsyncRepository<Genre> genreRepository)
        {
            _genreRepository = genreRepository;
        }
        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
            var genres = await _genreRepository.ListAllAsync();
            return genres.OrderBy(g => g.Name);
        }
        public async Task<IEnumerable<Genre>> GetGenresByMovieId(int movieId)
        {
            return await _genreRepository.GetGenresByMovieId(movieId);
        }
    }
}
