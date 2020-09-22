using MovieShop.Core.ApiModels.Request;
using MovieShop.Core.ApiModels.Response;
using MovieShop.Core.Entities;
using MovieShop.Core.RepositoryInterfaces;
using MovieShop.Core.ServiceInterfaces;
using MovieShop.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Http;
using MovieShop.Core.Exceptions;
using MovieShop.Core.Helpers;

namespace MovieShop.Infrastructure.Services
{
    public class UserService : IUserService
    {
        //private readonly IUserService : IUserService
        private readonly IUserRepository _userRepository;
        private readonly ICryptoService _cryptoService;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IAsyncRepository<Review> _reviewRepository;
        private readonly IMovieService _movieService;
        public UserService(IUserRepository userRepository, ICryptoService cryptoService,
                                IPurchaseRepository purchaseRepository, IMapper mapper, 
                                ICurrentUserService currentUserService, IFavoriteRepository favoriteRepository,
                                IAsyncRepository<Review> reviewRepository, IMovieService movieService)
        {
            _userRepository = userRepository;
            _cryptoService = cryptoService;
            _purchaseRepository = purchaseRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _favoriteRepository = favoriteRepository;
            _reviewRepository = reviewRepository;
            _movieService = movieService;
        }
        public async Task<UserRegisterResponseModel> CreateUser(UserRegiesterRequestModel requestModel)
        {

            //1. call GetUserByEmail with requestModel.Email to check if the email exists in the User Table or not 
            //if user exists return Email already exists and throw an Conflict exceotion

            //if email does not exists then we can proceed in creating the User record
            //1. Generate a random salt
            //2. var hashedPassword = We take requestModel.Password and add Salt from above step and Hash them to generate Unique Hash
            //3. Save Email, Salt, hashedPassword along with other details that user sent like FirstName, LastName etc
            //4. return the /userRegisterResponseModel object with newly created Id for the User

            var dbUser = await _userRepository.GetUserByEmail(requestModel.Email);
            if(dbUser != null)
            {
                throw new Exception("Email already exists");
            }
            var salt = _cryptoService.CreateSalt();
            var hashedPassword = _cryptoService.HashPassword(requestModel.Password, salt);

            var user = new User
            {
                Email = requestModel.Email,
                Salt = salt,
                HashedPassword = hashedPassword,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName
            };

            var createdUser = await _userRepository.AddAsync(user);
            var response = new UserRegisterResponseModel
            {
                Id = createdUser.Id,
                Email = requestModel.Email,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName
            };
            return response;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);

        }

        public async Task<User> ValidateUser(string email, string password)
        {
            //1 Go to database and get the whole record for this email, so that the object includes salt and hashedpassword

            var user = await _userRepository.GetUserByEmail(email);
            if(user == null)
            {
                // User did not even registered in our database
                return null;
            }
            //now User Registered 
            //hash the password with user entered password and database Salt
            var hashedPassword = _cryptoService.HashPassword(password, user.Salt);
            //we never generate the new salt
            if(hashedPassword == user.HashedPassword)
            {
                return user;
            }else return null;
        }

        public async Task AddFavorite(FavoriteRequestModel favoriteRequest)
        {
            //verify two things:user is currentUser, favorite hasn't been added.
            if (_currentUserService.UserId != favoriteRequest.UserId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to purchase");
            // See if Movie is already Favorited.
            if (await FavoriteExists(favoriteRequest.UserId, favoriteRequest.MovieId))
                throw new ConflictException("Movie already Favorited");

            var favorite = _mapper.Map<Favorite>(favoriteRequest);
            await _favoriteRepository.AddAsync(favorite);
        }

        public async Task AddMovieReview(ReviewRequestModel reviewRequest)
        {
            if (_currentUserService.UserId != reviewRequest.UserId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to Review");

            var review = _mapper.Map<Review>(reviewRequest);
            await _reviewRepository.AddAsync(review);
        }
        public async Task DeleteMovieReview(int userId, int movieId)
        {
            if (_currentUserService.UserId != userId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to delete review.");

            var review = await _reviewRepository.ListAsync(r => r.UserId == userId && r.MovieId == movieId);
            await _reviewRepository.DeleteAsync(review.First());
        }

        public async Task<bool> FavoriteExists(int id, int movieId)
        {
            return await _favoriteRepository.GetExistsAsync(f => f.UserId == id && f.MovieId == movieId);
        }

        public async Task<FavoriteResponseModel> GetAllFavoritesForUser(int id)
        {
            if (_currentUserService.UserId != id)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not Authorized to get favorites.");

            var favorites = await _favoriteRepository.ListAllWithIncludesAsync(
                f => f.UserId == id,
                f=>f.Movie);
            return _mapper.Map<FavoriteResponseModel>(favorites);
        }

        public async Task<PurchaseResponseModel> GetAllPurchasedMoviesByUser(int id)
        {
            return await _purchaseRepository.GetAllPurchasedMoviesByUser(id);
        }

        public async Task<PurchaseResponseModel> GetAllPurchasesForUser(int id)
        {
            if (_currentUserService.UserId != id)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not authorized to view the purchases.");

            var purchases = await _purchaseRepository.ListAllWithIncludesAsync(p => p.UserId == id, p => p.Movie);
            return _mapper.Map<PurchaseResponseModel>(purchases);
        }

        public async Task<ReviewResponseModel> GetAllReviewsByUser(int id)
        {
            if (_currentUserService.UserId != id)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not authorized to view the reviews.");

            var reviews = await _reviewRepository.ListAllWithIncludesAsync(r => r.UserId == id,r=>r.Movie);
            return _mapper.Map<ReviewResponseModel>(reviews);
        }

        public async Task<PagedResultSet<User>> GetAllUsersByPagination(int pageSize = 20, int page = 0, string lastName = "")
        {
            Expression<Func<User, bool>> filter = null;

            if(!string.IsNullOrEmpty(lastName))
            {
                filter = user => lastName != null && user.LastName == lastName;
            }

            var pagedUsers = await _userRepository.GetPagedData(page, pageSize,user=>user.OrderBy(u=>u.LastName), filter);

            var pagedResults = new PagedResultSet<User>(pagedUsers,page,pageSize,pagedUsers.TotalCount);
            return pagedResults;
        }

        public async Task<bool> IsMoviePurchased(PurchaseRequestModel purchaseRequest)
        {
            return await _purchaseRepository.GetExistsAsync(p => p.UserId == purchaseRequest.UserId
                                                                && p.MovieId == purchaseRequest.MovieId);
        }

        public async Task PurchaseMovie(PurchaseRequestModel purchaseRequest)
        {
            if (_currentUserService.UserId != purchaseRequest.UserId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not authorized to purchase.");

            if (await IsMoviePurchased(purchaseRequest))
                throw new ConflictException("Movie has already purchased.");

            var movie = await _movieService.GetMovieById(purchaseRequest.MovieId);
            purchaseRequest.TotalPrice = movie.Price;
            purchaseRequest.PurchaseDateTime = DateTime.Now;
            purchaseRequest.PurchaseNumber = Guid.NewGuid();

            var purchase = _mapper.Map<Purchase>(purchaseRequest);
            await _purchaseRepository.AddAsync(purchase);
        }

        public async Task RemoveFavorite(FavoriteRequestModel favoriteRequest)
        {
            if (_currentUserService.UserId != favoriteRequest.UserId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not authorized to delete the favorites.");

            if (!await FavoriteExists(favoriteRequest.UserId, favoriteRequest.MovieId))
                throw new ConflictException("Favorite does not exist.");
            var dbFavorite = await _favoriteRepository.ListAsync(f => f.UserId == favoriteRequest.UserId
                                                                    && f.MovieId == favoriteRequest.MovieId);
            //var favorite = _mapper.Map<Favorite>(favoriteRequest);
            await _favoriteRepository.DeleteAsync(dbFavorite.First());

        }

        public async Task UpdateMovieReview(ReviewRequestModel reviewRequest)
        {
            if (_currentUserService.UserId != reviewRequest.UserId)
                throw new HttpException(HttpStatusCode.Unauthorized, "You are not authorized to update review.");

            var review = _mapper.Map<Review>(reviewRequest);
            await _reviewRepository.UpdateAsync(review);
        }

    }
}
