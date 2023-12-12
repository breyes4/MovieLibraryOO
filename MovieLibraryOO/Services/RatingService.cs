using ConsoleTables;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Dao;
using MovieLibraryEntities.Models;
using MovieLibraryOO.Dto;
using MovieLibraryOO.Mappers.UserMovieMap;
using System;
using System.Linq;

namespace MovieLibraryOO.Services
{
    public class RatingService : IRatingService
    {
        private readonly ILogger<MainService> _logger;
        private readonly IUserMovieMapper _userMovieMapper;
        private readonly IRepository _repository;

        public RatingService(ILogger<MainService> logger, IUserMovieMapper userMovieMapper, IRepository repository)
        {
            _logger = logger;
            _userMovieMapper = userMovieMapper;
            _repository = repository;
        }
        public void Invoke()
        {
            var menu = new Menu();

            Menu.RatingMenuOptions menuChoice;
            do
            {
                menuChoice = menu.ChooseRatingAction();
                switch (menuChoice)
                {
                    
                    case Menu.RatingMenuOptions.ListFromDb:
                        _logger.LogInformation("Listing ratings from database");
                        _logger.LogInformation("Warning! Rating database is extreamly long!");
                        _logger.LogInformation(" It will take awhile to load, and only the ratings towards the end will stick around...");
                        var allUserMovies = _repository.GetAllUserMovie();
                        var userMovies = _userMovieMapper.Map(allUserMovies);
                        ConsoleTable.From<UserMovieDto>(userMovies).Write();
                        break;
                    case Menu.RatingMenuOptions.AddRating:
                        _logger.LogInformation("Adding a new rating");

                        
                        var ratingUserId = menu.GetUserResponse("To Rate A Movie, Enter your", "User Id:", "green");

                        
                        try
                        {
                            var ratingUserIdNum = int.Parse(ratingUserId);
                            using (var db = new MovieContext())
                            {
                                var ratingUser = db.Users.FirstOrDefault(x => x.Id == ratingUserIdNum);

                                if (ratingUser == null)
                                {
                                    Console.WriteLine($"There is no user with the id {ratingUserIdNum}");
                                }
                                else
                                {
                                    
                                    var ratingMovieTitle = menu.GetUserResponse("To Rate A Movie, Enter the", "Movie Title:", "green");

                                   
                                    var ratingMovie = db.Movies.FirstOrDefault(x => x.Title == ratingMovieTitle);
                                    if (ratingMovie == null)
                                    {
                                        Console.WriteLine($"There is no movie titled {ratingMovieTitle}");
                                    }
                                    else
                                    {
                                        
                                        var usersRatingList = db.UserMovies.ToList().Where(x => x.UserId.Equals(ratingUserIdNum));
                                        var presentMovieRating = usersRatingList.FirstOrDefault(x => x.MovieId == ratingMovie.Id);

                                        if (presentMovieRating != null)
                                        {
                                            Console.WriteLine($"The user with id {ratingUserIdNum}, has already rated the movie {ratingMovieTitle}");
                                        }
                                        else
                                        {
                                            
                                            var theRatingString = menu.GetUserResponse("For this movie, Enter the User's", "Rating From 1 to 5:", "green");

                                            
                                            try
                                            {
                                                var theRatingNum = int.Parse(theRatingString);

                                                if (theRatingNum > 0 && theRatingNum < 6)
                                                {
                                                    
                                                    DateTime currentDate = DateTime.UtcNow;

                                                    
                                                    var rating = new UserMovie()
                                                    {
                                                        Rating = theRatingNum,
                                                        RatedAt = currentDate,
                                                        UserId = ratingUser.Id,
                                                        MovieId = ratingMovie.Id
                                                    };

                                                    db.UserMovies.Add(rating);
                                                    db.SaveChanges();

                                                    
                                                    var usersRatingListCheck = db.UserMovies.ToList().Where(x => x.UserId.Equals(ratingUserIdNum));
                                                    var resultingRating = usersRatingListCheck.FirstOrDefault(x => x.MovieId == ratingMovie.Id);

                                                    Console.WriteLine($"({resultingRating.Id}), rating: {resultingRating.Rating}, user id: {resultingRating.UserId}, movie id: {resultingRating.MovieId}, rating given: {resultingRating.RatedAt}");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Rating must be from 1 to 5");
                                                }
                                            }
                                            catch (FormatException ex)
                                            {
                                                _logger.LogInformation(ex.Message);
                                                Console.WriteLine("Rating must be an integer");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (FormatException ex)
                        {
                            _logger.LogInformation(ex.Message);
                            Console.WriteLine("User ID must be an integer");
                        }

                        break;
                    case Menu.RatingMenuOptions.DeleteRating:
                        _logger.LogInformation("Deleting a rating");

                        
                        var removeRatingUserId = menu.GetUserResponse("To remove a Rating, Enter your", "User Id:", "yellow");

                        
                        try
                        {
                            var ratingUserIdNum = int.Parse(removeRatingUserId);
                            using (var db = new MovieContext())
                            {
                                var ratingUser = db.Users.FirstOrDefault(x => x.Id == ratingUserIdNum);

                                if (ratingUser == null)
                                {
                                    Console.WriteLine($"Warning! There is no user with the id {ratingUserIdNum} within the database");
                                }

                               
                                var removeRatingMovieId = menu.GetUserResponse("To remove a Rating, Enter the", "Movie Id:", "yellow");
                                var ratingMovieIdNum = int.Parse(removeRatingMovieId);

                                
                                var ratingMovie = db.Movies.FirstOrDefault(x => x.Id == ratingMovieIdNum);

                                if (ratingMovie == null)
                                {
                                    Console.WriteLine($"Warning! There is no movie with the id {ratingUserIdNum} within the database");
                                }

                                
                                var usersRatingList = db.UserMovies.ToList().Where(x => x.UserId.Equals(ratingUserIdNum));
                                var presentMovieRating = usersRatingList.FirstOrDefault(x => x.MovieId == ratingMovieIdNum);

                                if (presentMovieRating == null)
                                {
                                    Console.WriteLine($"The user with id {ratingUserIdNum}, has not rated the movie with id {ratingMovieIdNum}");
                                }
                                else
                                {
                                    Console.WriteLine($"({presentMovieRating.Id}), rating: {presentMovieRating.Rating}, user id: {presentMovieRating.UserId}, movie id: {presentMovieRating.MovieId}, rating given: {presentMovieRating.RatedAt}");

                                    
                                    db.UserMovies.Remove(presentMovieRating);
                                    db.SaveChanges();

                                    
                                    Console.WriteLine("This Rating has been deleted");
                                }
                            }
                        }
                        catch (FormatException ex)
                        {
                            _logger.LogInformation(ex.Message);
                            Console.WriteLine("IDs must be integers");
                        }

                        break;
                }
            }
            while (menuChoice != Menu.RatingMenuOptions.BackToMovies);
        }
    }
}