# MovieShopCoreAPI
This is a MovieShop API connecting the front-end MovieShopSPA (Single Page Application) 

    MovieShopSPA is located in another repo in my github account
and backend MovieShop (implemented with .NET MVC) 

    this is also located in another repo of my github account, named MovieShop

## Features : using .NET Core
## Language : C#

## API : 
  Controllers
  
  Appsetting.json : that contains connection string and token settings
  
  startip.cs file : 
  
## Core : 
  API Models (both Request models and Response models)
  
  Entities : all the individual entities that combine to set up this project (example entity could be movies, casts of movies, genres of movies, etc.)
  
  Exceptions : Custome exception middlewares
  
  Helpers : this folder is mainly set up for pagination feature
  
  Mapping Profiles : mapp movies to user profiles 
  
    -explain in details : user profile could contain favorited movies of this particular user, movie this.user purchased, etc.
    
  Repository Interfaces
  
  Service Interfaces
 
## Infrastrucutre :
  Data : "MovieShopDbContext.cs" use to make a connection to our database
  
  Migrations : autogenerated when using command line to generate tables to databases (linq -> sql query)
  
  Repositories
  
  Services
  
## Unit test :
  Unit testing of this project, using Postman
