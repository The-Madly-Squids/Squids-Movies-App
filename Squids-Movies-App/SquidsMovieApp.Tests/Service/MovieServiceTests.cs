﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SquidsMovieApp.Common;
using SquidsMovieApp.Data.Context;
using SquidsMovieApp.Models;
using SquidsMovieApp.DTO;
using SquidsMovieApp.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SquidsMovieApp.Tests.Service
{
    [TestClass]
    public class MovieServiceTests
    {
        [ClassInitialize]
        public static void InitilizeAutomapper(TestContext context)
        {
            AutomapperConfiguration.Initialize();
        }

        [TestMethod]
        public void AddMovieShouldCorrectlyAddMovieToDb_WhenCalledWithValidData()
        {
            // arrange
            var effortContext = new MovieAppDBContext(
                        Effort.DbConnectionFactory.CreateTransient());
            var mapperMock = new Mock<IMapper>();

            var movieObjectToReturn = new Movie()
            {
                Title = "Test Movie Object",
                Runtime = 120
            };

            mapperMock.Setup(x => x.Map<Movie>(It.IsAny<MovieModel>()))
                .Returns(movieObjectToReturn);

            var movieModelToAdd = new MovieModel()
            {
                Title = "Test Movie model",
                Runtime = 120
            };

            // Act
            var sut = new MovieService(effortContext, mapperMock.Object);
            sut.AddMovie(movieModelToAdd);

            Assert.AreEqual(1, effortContext.Movies.Count());
        }

        [TestMethod]
        public void AddMovieShould_ThrowWhenCalledWithInvalidData()
        {

            // arrange
            var effortContext = new MovieAppDBContext(
                        Effort.DbConnectionFactory.CreateTransient());
            var mapperMock = new Mock<IMapper>();

            var movieObjectToReturn = new Movie()
            {
                Title = "Test Movie Object",
                Runtime = 120
            };

            mapperMock.Setup(x => x.Map<Movie>(It.IsAny<MovieModel>()))
                .Returns(movieObjectToReturn);

            var movieToAdd = new MovieModel();
            var sut = new MovieService(effortContext, mapperMock.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => sut.AddMovie((movieToAdd)));
        }

        [TestMethod]
        public void RemoveMovieShould_CorrectlyRemoveMovieFromDB()
        {
            // Arrange
            var effortContext = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());

            var mapperMock = new Mock<IMapper>();

            var movieObjectToRemove = new Movie()
            {
                Title = "Test title",
                Runtime = 120
            };

            effortContext.Movies.Add(movieObjectToRemove);
            effortContext.SaveChanges();

            // when testing do not use automapper - you are coupling yourself
            // to autommaper and if its broken/ or you change it - the test will
            // also fail - so do it manually
            //var movieDtoToRemove = Mapper.Map<MovieModel>(movieObjectToRemove);

            var movieDtoToRemove = new MovieModel()
            {
                MovieId = movieObjectToRemove.MovieId,
                Title = movieObjectToRemove.Title,
                Runtime = movieObjectToRemove.Runtime
            };

            // Act
            var sut = new MovieService(effortContext, mapperMock.Object);
            sut.RemoveMovie(movieDtoToRemove);

            //Assert
            Assert.AreEqual(0, effortContext.Movies.Count());
        }

        [TestMethod]
        public void RemoveMovieShould_ThrowWhenCalledWithInvalidData()
        {

            // Arrange
            var effortContext = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());

            var mapperMock = new Mock<IMapper>();

            var movieObjectToRemove = new Movie()
            {
                Title = "Test title",
                Runtime = 120
            };

            effortContext.Movies.Add(movieObjectToRemove);
            effortContext.SaveChanges();

            var movieDtoToRemove = new MovieModel()
            {
                MovieId = 1000000,
                Title = movieObjectToRemove.Title,
                Runtime = movieObjectToRemove.Runtime
            };

            var sut = new MovieService(effortContext, mapperMock.Object);

            // Act & Assert
            Assert.ThrowsException<System.ArgumentNullException>(() => sut.RemoveMovie(movieDtoToRemove));
        }

        [TestMethod]
        public void RemoveMovieShould_ThrowWhenMovieToRemoveNotFoundInDataBase()
        {
            // Arrange
            var effortContext = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());

            var mapperMock = new Mock<IMapper>();

            var movieObjectToRemove = new Movie()
            {
                Title = "Test title",
                Runtime = 120
            };

            //effortContext.Movies.Add(movieObjectToRemove);
            //effortContext.SaveChanges();

            var movieDtoToRemove = new MovieModel()
            {
                MovieId = movieObjectToRemove.MovieId,
                Title = movieObjectToRemove.Title,
                Runtime = movieObjectToRemove.Runtime
            };

            var sut = new MovieService(effortContext, mapperMock.Object);

            // Act & Assert     
            Assert.ThrowsException<ArgumentNullException>(() => sut.RemoveMovie(movieDtoToRemove));
        }

        [TestMethod]
        public void GetAllMoviesShould_ReturnCorrectDataWhenCalled()
        {
            // Arrange
            var effortContext = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());

            var mapperMock = new Mock<IMapper>();

            for (var i = 0; i < 10; i++)
            {
                var movieToAdd = new Movie()
                {
                    Title = "Test title" + i,
                    Runtime = 120
                };

                effortContext.Movies.Add(movieToAdd);
            }
            effortContext.SaveChanges();

            var moviesDTOsListToReturn = new List<MovieModel>();
            foreach (var movie in effortContext.Movies)
            {
                var movieDto = new MovieModel()
                {
                    MovieId = movie.MovieId,
                    Title = movie.Title,
                    Runtime = movie.Runtime
                };

                moviesDTOsListToReturn.Add(movieDto);
            }

            mapperMock.Setup(x => x.Map<IList<MovieModel>>(
                                                    It.IsAny<IList<Movie>>()))
                .Returns(moviesDTOsListToReturn);


            // Act
            var sut = new MovieService(effortContext, mapperMock.Object);
            var result = sut.GetAllMovies();

            // Assert
            //foreach (var movieDto in result)
            //{
            //    var exists = effortContext.Movies.Any(x => x.MovieId == movieDto.MovieId);
            //    Assert.IsTrue(exists);
            //}

            // Or Another Assert  - just checks if the count is the same
            // both work - second is easier
            Assert.AreEqual(10, result.Count());

        }

        [TestMethod]
        public void GetAllParticipantsPerMovieShould_ReturnCorrectParticipants()
        {

            // Arrange
            var effortContext = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());
            var mapperMock = new Mock<IMapper>();
            var sut = new MovieService(effortContext, mapperMock.Object);

            var movieToAdd = new Movie()
            {
                Title = "Test title",
                Runtime = 120
            };

            var movieDto = new MovieModel()
            {
                MovieId = movieToAdd.MovieId,
                Title = movieToAdd.Title,
                Runtime = movieToAdd.Runtime
            };

            effortContext.Movies.Add(movieToAdd);

            for (int i = 0; i < 10; i++)
            {
                var participant = new Participant()
                {
                    FirstName = "FirstName" + i,
                    LastName = "LastName" + i
                };
                effortContext.Participants.Add(participant);
                participant.Movies.Add(movieToAdd);
                var participant2 = effortContext.Participants
                    .Where(x => x.FirstName == "FirstName" + i)
                    .FirstOrDefault();
                movieToAdd.Participants.Add(participant2);
                sut.AddMovieParticipant(movieDto, new ParticipantModel()
                {
                    FirstName = participant.FirstName,
                    LastName = participant.LastName
                },
                 $"someRole + {i}");
                effortContext.SaveChanges();
            }

            var participantsDTOsListToReturn = new List<ParticipantModel>();
            //effortContext.SaveChanges();



            mapperMock.Setup(x => x.Map<IList<ParticipantModel>>(
                                                    It.IsAny<IList<Participant>>()))
                .Returns(participantsDTOsListToReturn);


            // Act & Assert
            var result = sut.GetAllParticipantsPerMovie(movieDto);

            Assert.AreEqual(10, result.Count());
        }

        [TestMethod]
        public void AddMovieParticipantShouldCorrectlyAddParticipantToMovie_WhenCalledWithValidData()
        {
            // Assert
            var effort = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());

            var mapperMock = new Mock<IMapper>();

            var movieObject = new Movie()
            {
                Title = "Test Movie",
                Year = 1990
            };

            var participantObject = new Participant()
            {
                FirstName = "Arnold",
                LastName = "Ivanov"
            };

            effort.Movies.Add(movieObject);
            effort.Participants.Add(participantObject);
            effort.SaveChanges();

            var participantDto = new ParticipantModel()
            {
                ParticipantId = participantObject.ParticipantId,
                FirstName = participantObject.FirstName,
                LastName = participantObject.LastName
            };

            var movieDto = new MovieModel()
            {
                MovieId = movieObject.MovieId,
                Title = movieObject.Title,
                Runtime = movieObject.Runtime
            };

            var roleName = "Actor";

            // Act
            var sut = new MovieService(effort, mapperMock.Object);
            sut.AddMovieParticipant(movieDto, participantDto, roleName);

            // Assert
            var participantAddedToMovie = movieObject.Participants.FirstOrDefault();
            var movieAddedToParticipant = participantObject.Movies.FirstOrDefault();
            var roleIsCorrect = effort.Roles.FirstOrDefault().RoleName == "Actor";

            // Unit test should fail for only one reason
            // either change the method to be SRP or separate the test into 3 parts
            Assert.IsTrue(participantAddedToMovie == participantObject);
            Assert.IsTrue(movieAddedToParticipant == movieObject);
            Assert.IsTrue(roleIsCorrect);
        }

        [TestMethod]
        public void AddMovieParticipantShould_ThrowWhenMovieNotFoundInDataBase()
        {
            // For Paco
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddMovieParticipantShould_ThrowWhenParticipantNotFoundInDataBase()
        {
            // For Toni
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetRatingShould_ReturnCorrectValueWhenCalled()
        {
            // for Paco
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetActorsShould_ReturnCorrectValueWhenCalled()
        {
            // Arrange
            var effort = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());

            var mapperMock = new Mock<IMapper>();

            var movieObject = new Movie()
            {
                Title = "Test Movie",
                Year = 1990
            };
            effort.Movies.Add(movieObject);

            for (int i = 0; i < 10; i++)
            {
                var participantObject = new Participant()
                {
                    FirstName = "Test" + i,
                    LastName = "Testov"
                };

                var roleObject = new Role()
                {
                    Movie = movieObject,
                    Participant = participantObject,
                    RoleName = "Actor"
                };

                effort.Participants.Add(participantObject);
                effort.Roles.Add(roleObject);
            }
            effort.SaveChanges();

            var participantDTOsListToReturn = Mapper.Map<IList<ParticipantModel>>(
                effort.Participants);

            var movieDtoArgument = Mapper.Map<MovieModel>(movieObject);

            mapperMock.Setup(x => x.Map<IList<ParticipantModel>>(
                                                    It.IsAny<IList<Participant>>()))
                .Returns(participantDTOsListToReturn);

            // Act
            var sut = new MovieService(effort, mapperMock.Object);
            var result = sut.GetActors(movieDtoArgument);

            // Assert
            foreach (var actorPoco in effort.Participants)
            {
                var exists = result.Any(x => x.ParticipantId == actorPoco.ParticipantId);
                Assert.IsTrue(exists);
            }
        }

        [TestMethod]
        public void GetDirectorsShould_ReturnCorrectValueWhenCalled()
        {
            // Act
            var effort = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());

            var mapperMock = new Mock<IMapper>();

            var movieObject = new Movie()
            {
                Title = "Test Movie",
                Year = 1990
            };

            effort.Movies.Add(movieObject);

            for (int i = 0; i < 10; i++)
            {
                var participantObject = new Participant()
                {
                    FirstName = "Test" + i,
                    LastName = "Testov"
                };

                var roleObject = new Role()
                {
                    Movie = movieObject,
                    Participant = participantObject,
                    RoleName = "Director"
                };

                effort.Participants.Add(participantObject);
                effort.Roles.Add(roleObject);
            }
            effort.SaveChanges();

            var expectedResult = effort.Roles
                .Where(x => x.Movie.MovieId == movieObject.MovieId && x.RoleName == "Director")
                .Select(p => p.Participant).ToList();

            var movieDtoArgument = Mapper.Map<MovieModel>(movieObject);

            // Act
            var sut = new MovieService(effort, mapperMock.Object);
            var result = sut.GetDirectors(movieDtoArgument);

            // Assert
            foreach (var p in expectedResult)
            {
                //var exists = result.Movies.Any(x => x.MovieId == movieDto.MovieId);
                var exists = result.Any(x => x.ParticipantId == p.ParticipantId);
                Assert.IsTrue(exists);
            }
        }

        [TestMethod]
        public void GetMovieGenresShould_ReturnCorrectValueWhenCalled()
        {
            // Act
            var effort = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());
            var mapperMock = new Mock<IMapper>();

            var movieObject = new Movie()
            {
                Title = "Test Movie",
                Year = 1990
            };

            effort.Movies.Add(movieObject);

            for (int i = 0; i < 10; i++)
            {
                var genre = new Genre()
                {
                    GenreType = "SomeGenre" + i
                };
                effort.Genres.Add(genre);
                movieObject.Genres.Add(genre);
            }

            effort.SaveChanges();

            var expectedResult = effort.Movies
                .Where(x => x.Title == "Test Movie")
                .FirstOrDefault()
                .Genres
                .ToList();

            var movieDtoArgument = Mapper.Map<MovieModel>(movieObject);

            // Act
            var sut = new MovieService(effort, mapperMock.Object);
            var result = sut.GetMovieGenres(movieDtoArgument);

            // Assert

            //Assert.AreEqual(10, result.Count()); Also works OK

            foreach (var p in expectedResult)
            {
                var exists = result.Any(x => x.GenreType == p.GenreType);
                Assert.IsTrue(exists);
            }
        }

        [TestMethod]
        public void GetUsersWhoBoughtMovieShould_ReturnCorrectValueWhenCalled()
        {
            // Act
            var effort = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());
            var mapperMock = new Mock<IMapper>();

            var movieObject = new Movie()
            {
                Title = "Test Movie",
                Year = 1990
            };

            effort.Movies.Add(movieObject);

            for (int i = 0; i < 10; i++)
            {
                var user = new User()
                {
                    FirstName = "Pesho" + i,
                    LastName = "Peshev" + i,
                    Username = "UserName" + i,
                    Email = $"someMail{i}@mail.com",
                    Password = $"somepassword{i}"
                };
                effort.Users.Add(user);
                movieObject.BoughtBy.Add(user);
            }

            effort.SaveChanges();

            var expectedResult = effort.Movies
                .Where(x => x.Title == "Test Movie")
                .FirstOrDefault()
                .BoughtBy
                .ToList();

            var movieDtoArgument = Mapper.Map<MovieModel>(movieObject);

            // Act
            var sut = new MovieService(effort, mapperMock.Object);
            var result = sut.GetUsersWhoBoughtIt(movieDtoArgument);

            // Assert

            //Assert.AreEqual(10, result.Count()); Also works OK

            foreach (var p in expectedResult)
            {
                var exists = result.Any(x => x.Username == p.Username);
                Assert.IsTrue(exists);
            }
        }

        [TestMethod]
        public void GetUsersWhoLikedMovieShould_ReturnCorrectValueWhenCalled()
        {
            // Act
            var effort = new MovieAppDBContext(
                Effort.DbConnectionFactory.CreateTransient());
            var mapperMock = new Mock<IMapper>();

            var movieObject = new Movie()
            {
                Title = "Test Movie",
                Year = 1990
            };

            effort.Movies.Add(movieObject);

            for (int i = 0; i < 10; i++)
            {
                var user = new User()
                {
                    FirstName = "Pesho" + i,
                    LastName = "Peshev" + i,
                    Username = "UserName" + i,
                    Email = $"someMail{i}@mail.com",
                    Password = $"somepassword{i}"
                };
                effort.Users.Add(user);
                movieObject.LikedBy.Add(user);
            }

            effort.SaveChanges();

            var expectedResult = effort.Movies
                .Where(x => x.Title == "Test Movie")
                .FirstOrDefault()
                .LikedBy
                .ToList();

            var movieDtoArgument = Mapper.Map<MovieModel>(movieObject);

            // Act
            var sut = new MovieService(effort, mapperMock.Object);
            var result = sut.GetUsersWhoLikedtIt(movieDtoArgument);

            // Assert

            //Assert.AreEqual(10, result.Count()); Also works OK

            foreach (var p in expectedResult)
            {
                var exists = result.Any(x => x.Username == p.Username);
                Assert.IsTrue(exists);
            }
        }
    }
}
