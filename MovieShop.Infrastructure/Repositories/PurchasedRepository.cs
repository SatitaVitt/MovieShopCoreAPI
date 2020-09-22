using MovieShop.Core.Entities;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MovieShop.Core.ApiModels.Response;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShop.Infrastructure.Repositories
{
    public class PurchasedRepository : EfRepository<Purchase>, IPurchaseRepository
    {
        public PurchasedRepository(MovieShopDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<PurchaseResponseModel> GetAllPurchasedMoviesByUser(int userId)
        {
            var movies = await _dbContext.Purchases.Where(p => p.UserId == userId)
                                .Include(f => f.Movie)
                                .Select(f => new PurchaseResponseModel.PurchasedMovieResponseModel
                                {
                                    Id = userId,
                                    Title = f.Movie.Title,
                                    PosterUrl = f.Movie.PosterUrl,
                                    PurchaseDateTime = f.PurchaseDateTime
                                }).ToListAsync();
            PurchaseResponseModel purchaseResponse = new PurchaseResponseModel
            {
                UserId = userId,
                PurchasedMovies = movies
            };

            return purchaseResponse;
        }
    }
}
