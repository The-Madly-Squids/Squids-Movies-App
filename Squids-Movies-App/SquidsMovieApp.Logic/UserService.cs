﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using SquidsMovieApp.Data.Context;
using SquidsMovieApp.Data.Models;
using SquidsMovieApp.DTO;
using SquidsMovieApp.Logic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquidsMovieApp.Logic
{
    public class UserService : IUserService
    {
        private readonly IMovieAppDBContext movieAppDbContext;
        private readonly IMapper mapper;

        public UserService(IMovieAppDBContext movieAppDbContext, IMapper mapper)
        {
            this.movieAppDbContext = movieAppDbContext;
            this.mapper = mapper;
        }

        public void AddUser(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentException();
            }

            var userToAdd = this.mapper.Map<User>(user);
            this.movieAppDbContext.Users.Add(userToAdd);
            this.movieAppDbContext.SaveChanges();
        }

        public void RemoveUser(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentException();
            }

            var userToRemove = Mapper.Map<User>(user);
            movieAppDbContext.Users.Remove(userToRemove);
            movieAppDbContext.SaveChanges();
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            var users = this.movieAppDbContext.Users.ProjectTo<UserModel>();
            return users;
        }

        public IEnumerable<ParticipantModel> GetLikedParticipants(UserModel user)
        {
            // this was valid before but now when DTO holds DTO collections
            // you must first find the user in the DB 
            // and then project his collecition to DTO object and return it

            var userObject = this.movieAppDbContext.Users
                .Where(x => x.UserId == user.UserId)
                .FirstOrDefault();

            if (userObject == null)
            {
                throw new ArgumentNullException("User not found!");
            }

            var likedDirectors = userObject.LikedParticipants;

            throw new NotImplementedException();
        }

        public IEnumerable<MovieModel> GetLikedMovies(UserModel user)
        {
            var likedMovies = user.LikedMovies;
            return likedMovies;
        }

        public IEnumerable<MovieModel> GetBoughtMovies(UserModel user)
        {
            var boughtMovies = user.BoughtMovies;
            return boughtMovies;
        }

        public IEnumerable<UserModel> GetFollowers(UserModel user)
        {
            var followers = user.Followers;
            return followers;
        }

        public IEnumerable<UserModel> GetFollowed(UserModel user)
        {
            var followedUsers = user.Following;
            return followedUsers;
        }

        public decimal GetMoneyBalance(UserModel user)
        {
            decimal moneyBallance = user.MoneyBalance;
            return moneyBallance;
        }

        public void AddMoneyToBalance(UserModel user, decimal amount)
        {
            if (amount < 1)
            {
                throw new ArgumentException("Amount cannot be less then 1!");
            }
            var userObject = this.movieAppDbContext.Users
                                .Where(x => x.UserId == user.UserId)
                                .FirstOrDefault();

            if (userObject == null)
            {
                throw new ArgumentNullException("User not found!");
            }

            decimal moneyBalance = userObject.MoneyBalance;
            moneyBalance += amount;
            movieAppDbContext.SaveChanges();
        }

        public void LikeParticipant(UserModel user, ParticipantModel participant)
        {
            //var actorToAdd = mapper.Map<Participant>(actor);
            //user.LikedActors.Add(actorToAdd);
            //movieAppDbContext.SaveChanges();

            var userObject = this.movieAppDbContext.Users
                            .Where(x => x.UserId == user.UserId)
                            .FirstOrDefault();

            var participantObject = this.movieAppDbContext.Participants
                              .Where(x => x.ParticipantId == participant.ParticipantId)
                              .FirstOrDefault();

            if (userObject == null)
            {
                throw new ArgumentNullException("User not found!");
            }

            if (participantObject == null)
            {
                throw new ArgumentNullException("Participant not found!");
            }

            userObject.LikedParticipants.Add(participantObject);
            participantObject.ParticipantLikedByUser.Add(userObject);
            this.movieAppDbContext.SaveChanges();
        }

        public void FollowUser(UserModel user, UserModel userToFollow)
        {
            var userObject = this.movieAppDbContext.Users
                             .Where(x => x.UserId == user.UserId)
                             .FirstOrDefault();

            var userToFollowObject = this.movieAppDbContext.Users
                                     .Where(x => x.UserId == userToFollow.UserId)
                                     .FirstOrDefault();

            if (userObject == null)
            {
                throw new ArgumentNullException("User that wants to follow not found!");
            }

            if (userToFollowObject == null)
            {
                throw new ArgumentNullException("User that will be followed not found!");
            }

            userObject.Following.Add(userToFollowObject);
            userToFollowObject.Followers.Add(userObject);
            this.movieAppDbContext.SaveChanges();
        }

        public void BuyMovie(UserModel user, MovieModel movie, decimal price)
        {

            var userObject = this.movieAppDbContext.Users
                            .Where(x => x.UserId == user.UserId)
                            .FirstOrDefault();

            var movieObject = this.movieAppDbContext.Movies
                              .Where(x => x.MovieId == movie.MovieId)
                              .FirstOrDefault();

            if (userObject == null)
            {
                throw new ArgumentNullException("User not found!");
            }

            if (movieObject == null)
            {
                throw new ArgumentNullException("Movie not found!");
            }

            //In this case we should use transaction-like pattern
            try
            {
                if (userObject.MoneyBalance < price)
                {
                    throw new ArgumentException("Unsufficient money ballance to buy this movie!");
                }
                userObject.MoneyBalance -= price;
                userObject.BoughtMovies.Add(movieObject);
                movieObject.BoughtBy.Add(userObject);
                movieAppDbContext.SaveChanges();
            }
            catch (Exception)
            {
                Console.WriteLine("Sorry pich!");
            }
        }

        public void GiveReview(UserModel user, MovieModel movie, int reviewRating,
            string reviewDescription)
        {

            var userObject = this.movieAppDbContext.Users
                                .Where(x => x.UserId == user.UserId)
                                .FirstOrDefault();

            var movieObject = this.movieAppDbContext.Movies
                                .Where(x => x.MovieId == movie.MovieId)
                                .FirstOrDefault();

            if (userObject == null)
            {
                throw new ArgumentNullException("user not found!");
            }

            if (movieObject == null)
            {
                throw new ArgumentNullException("movie not found!");
            }

            var reviewObject = new Review()
            {
                Movie = movieObject,
                MovieId = movieObject.MovieId,
                Rating = reviewRating,
                Description = reviewDescription,
                User = userObject,
                UserId = userObject.UserId
            };

            this.movieAppDbContext.Reviews.Add(reviewObject);
            this.movieAppDbContext.SaveChanges();

            //Movie movieToReview = mapper.Map<Movie>(movie);
            //review.Movie = movieToReview;

            //var reviewToAdd = mapper.Map<Review>(review);
            //user.Reviews.Add(reviewToAdd);

            //movieAppDbContext.SaveChanges();
        }
    }
}