﻿using SquidsMovieApp.Data.Context;
using SquidsMovieApp.Data.Models;
using SquidsMovieApp.Utilities.Parsers;
using SquidsMovieApp.Utilities.Parsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquidsMovieApp.Utilities.Converters
{
    /// <summary>
    /// Converts parsed models from json to DbContext model.
    /// </summary>
    public class MovieConverter
    {
        private readonly MovieAppDBContext ctx;
        private readonly WebConverter webConverter;
        private readonly Parser parser;

        public MovieConverter(MovieAppDBContext ctx, WebConverter webConverter, Parser parser)
        {
            this.ctx = ctx;
            this.webConverter = webConverter;
            this.parser = parser;
        }

        public void AddOrUpdateMovies(ICollection<MovieParsedModel> movies)
        {
            foreach (var movie in movies)
            {
                var movieFound = this.ctx.Movies.FirstOrDefault(m => m.Title == movie.Title);

                if (movieFound == null)
                {
                    var newMovie = new Movie
                    {
                        Title = movie.Title,
                        Plot = movie.Plot,
                        Year = movie.Year,
                        Rated = movie.Rated,
                        Price = 30,
                        ImdbRating = double.Parse(movie.ImdbRating),
                        Runtime = ParseRuntime(movie.Runtime)
                    };

                    var actorsList = ConvertStringToListOfStrings(movie.Actors);

                    AddRolesToDbContext(actorsList, newMovie, "Actor");

                    var directorsList = ConvertStringToListOfStrings(movie.Director);

                    AddRolesToDbContext(directorsList, newMovie, "Director");

                    var genresList = ConvertStringToListOfStrings(movie.Genres);

                    foreach (var genre in genresList)
                    {
                        var currentGenre = AddGenreToDbContext(genre);
                        newMovie.Genres.Add(currentGenre);
                    }

                    var poster = new MoviePoster()
                    {
                        Poster = this.webConverter.ImageFromUrlToByreArray(movie.PosterUrl)
                    };

                    newMovie.Poster = poster;
                    
                    this.ctx.Movies.Add(newMovie);
                    //this.ctx.SaveChanges();
                }
            }
        }

        private Genre AddGenreToDbContext(string genre)
        {
            var genreFound = this.ctx.Genres.FirstOrDefault(g => g.GenreType == genre);

            if (genreFound == null)
            {
                genreFound = new Genre
                {
                    GenreType = genre
                };

                this.ctx.Genres.Add(genreFound);
            }

            return genreFound;
        }

        private void AddRolesToDbContext(List<string> participantList, Movie movie, string roleName)
        {
            foreach (var currentParticipant in participantList)
            {
                var participant = AddOrUpdateParticipant(currentParticipant);

                var role = new Role
                {
                    Movie = movie,
                    Participant = participant,
                    RoleName = roleName
                };

                this.ctx.Roles.Add(role);
            }
        }

        private Participant AddOrUpdateParticipant(string participantName)
        {
            var fullName = participantName.Split(' ');
            var firstName = fullName[0];
            var lastName = fullName[1];

            var participant = this.ctx.Participants.FirstOrDefault(p => p.FirstName == firstName && p.LastName == lastName);

            if (participant == null)
            {
                participant = new Participant
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Age = 30, // get from bio
                    Bio = GetParticipantBio(participantName)
                };

                this.ctx.Participants.Add(participant);
            }

            return participant;
        }

        private string GetParticipantBio(string participantName)
        {
            var url = $@"https://en.wikipedia.org/w/api.php?format=jsonfm&action=query&prop=extracts&exintro=&explaintext=&exsentences=3&titles={participantName}";

            var participantBioFromUrl = this.webConverter.JsonFromUrlToString(url);

            var participant = this.parser.ParseParticipantBio(participantBioFromUrl);

            return participant.Bio;
        }

        private int ParseRuntime(string runtime)
        {
            int time = int.Parse(runtime.Split(' ')[0]);

            return time;
        }

        private List<string> ConvertStringToListOfStrings(string str)
        {
            var list = str.Split(',').Select(p => p.Trim()).ToList();
            return list;
        }
    }
}
