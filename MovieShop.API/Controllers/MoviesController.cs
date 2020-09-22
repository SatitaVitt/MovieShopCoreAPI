using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Core.ServiceInterfaces;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private IMovieService _movieService;
        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        [Route("top")]
        public async Task<IActionResult> GetTopGrossingMovies()
        {
            var movies = await _movieService.GetTopGrossingMovies();
            return Ok(movies);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _movieService.GetMovieById(id);
            return Ok(movie);
        }

        [HttpGet]
        [Route("CastId/{castId}")]
        public async Task<IActionResult> GetMoviesByCast(int castId)
        {
            var movies = await _movieService.GetMoviesByCast(castId);
            return Ok(movies);
        }

        [HttpGet]
        [Route("genre/{genreid}/{page:int?}")]
        public async Task<IActionResult> GetAllMoviesByGenre(int genreId, int? page = 1)
        {
            var movies = await _movieService.GetMoviesByGenre(genreId);
            return Ok(movies);
            /*
            var movies = await _movieRepository.GetMoviesByGenre(Utility.GetGenreById(genreId));
            var response = movies != null
                ? Request.CreateResponse(HttpStatusCode.OK,
                    AutoMapper.Mapper.Map<IList<SearchMovie>, IList<Movie>>(movies.Results)
                )
                : Request.CreateResponse(HttpStatusCode.NotFound, "No Movies were found");
            return ResponseMessage(response);
            */
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMovieByPagination([FromQuery] int pageSize = 20, [FromQuery] int pageIndex = 1, string title = ""){
            var movies = await _movieService.GetMovieByPagination(pageSize, pageIndex, title);
            return DayOfWeek(movies);
        }
    }
}