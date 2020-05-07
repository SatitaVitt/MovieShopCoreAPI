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

namespace MovieShop.Infrastructure.Services
{
    public class UserService : IUserService
    {
        //private readonly IUserService : IUserService
        private readonly IUserRepository _userRepository;
        private readonly ICryptoService _cryptoService;
        public UserService(IUserRepository userRespository, ICryptoService cryptoService)
        {
            _userRepository = userRespository;
            _cryptoService = cryptoService;
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

        public Task<PurchasedResponseModel> GetAllPurchasedMoviesByUser(int id)
        {
            throw new NotImplementedException();
            //HW
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
    }
}
