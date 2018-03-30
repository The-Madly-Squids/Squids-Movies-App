﻿using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SquidsMovieApp.Core.Factories;
using SquidsMovieApp.Core.Factories.Contracts;
using SquidsMovieApp.DTO;
using SquidsMovieApp.Logic.Contracts;
using SquidsMovieApp.WPF.Controllers;

namespace SquidsMovieApp.Tests.Controller
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public void AddUserShouldCorrectlyInvokeService_WhenCalledWithValidData()
        {

            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var participantServiceMock = new Mock<IParticipantService>();
            var mapperMock = new Mock<IMapper>();
            // : 'Invalid setup on a non-virtual (overridable in VB) member
            // because the mock was not interface
            var factoryMock = new Mock<IUserFactory>();

            var firstName = "Katerina";
            var lastName = "Bozhilova";
            var age = 20;
            var email = "kateto1998@abv.bg";
            var nickName = "masterProgrammer1998";
            var password = "1234567";
            var isAdmin = false;
            var moneyBalance = 128382382.255M;

            // Could use It.IsAny<> here aswell
            factoryMock.Setup(x => x.CreateUserModel(firstName, lastName, age, nickName,
                email, password, isAdmin, moneyBalance))
                .Returns(new UserModel());

            // Act
            var sut = new UserController(userServiceMock.Object, participantServiceMock.Object,
                mapperMock.Object, factoryMock.Object);
            sut.CreateUser(firstName, lastName, nickName, email, password, age, moneyBalance, isAdmin);

            // Assert
            userServiceMock.Verify(x => x.AddUser(It.IsAny<UserModel>()), Times.Once);
        }

        [TestMethod]
        public void RemoveUserShouldCorrectlyInvokeService_WhenCalledWithValidUserName()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var participantServiceMock = new Mock<IParticipantService>();
            var mapperMock = new Mock<IMapper>();
            var factoryMock = new Mock<IUserFactory>();

            userServiceMock.Setup(x => x.GetUser(It.IsAny<string>()))
                .Returns(new UserModel());

            var sut = new UserController(userServiceMock.Object, participantServiceMock.Object,
              mapperMock.Object, factoryMock.Object);

            // Act
            sut.RemoveUser("randomString@abv.com");

            // Assert
            userServiceMock.Verify(x => x.RemoveUser(It.IsAny<UserModel>()), Times.Once);
        }

        [TestMethod]
        public void GetAllUsersShouldCorrectlyInvokeService_WhenCalled()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var participantServiceMock = new Mock<IParticipantService>();
            var mapperMock = new Mock<IMapper>();
            var factoryMock = new Mock<IUserFactory>();

            userServiceMock.Setup(x => x.GetAllUsers())
                .Returns(new List<UserModel>());

            var sut = new UserController(userServiceMock.Object, participantServiceMock.Object,
              mapperMock.Object, factoryMock.Object);

            // Act
            sut.GetAllUsers();

            // Assert
            userServiceMock.Verify(x => x.GetAllUsers(), Times.Once);
        }

        [TestMethod]
        public void GetUserShouldCorrectlyInvokeService_WhenCalledWithValidUserName()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var participantServiceMock = new Mock<IParticipantService>();
            var mapperMock = new Mock<IMapper>();
            var factoryMock = new Mock<IUserFactory>();

            userServiceMock.Setup(x => x.GetUser(It.IsAny<string>()))
                .Returns(new UserModel());

            var sut = new UserController(userServiceMock.Object, participantServiceMock.Object,
              mapperMock.Object, factoryMock.Object);

            // Act
            sut.GetUser("randomName");

            // Assert
            userServiceMock.Verify(x => x.GetUser(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GetUserByEmailShouldCorrectlyInvokeService_WhenCalledWithValidUserEmail()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var participantServiceMock = new Mock<IParticipantService>();
            var mapperMock = new Mock<IMapper>();
            var factoryMock = new Mock<IUserFactory>();

            userServiceMock.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .Returns(new UserModel());

            var sut = new UserController(userServiceMock.Object, participantServiceMock.Object,
              mapperMock.Object, factoryMock.Object);

            // Act
            sut.GetUserByEmail("randomEmail@abv.com");

            // Assert
            userServiceMock.Verify(x => x.GetUserByEmail(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GetLikedParticipantsShouldCorrectlyInvokeService_WhenCalledWithValidUserName()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var participantServiceMock = new Mock<IParticipantService>();
            var mapperMock = new Mock<IMapper>();
            var factoryMock = new Mock<IUserFactory>();

            userServiceMock.Setup(x => x.GetLikedParticipants(It.IsAny<UserModel>()))
                .Returns(new List<ParticipantModel>());

            var sut = new UserController(userServiceMock.Object, participantServiceMock.Object,
              mapperMock.Object, factoryMock.Object);

            // Act
            sut.GetLikedParticipants("randomUserName");

            // Assert
            userServiceMock.Verify(x => x.GetLikedParticipants(It.IsAny<UserModel>()), Times.Once);
        }

        [TestMethod]
        public void GetLikedMoviesShouldCorrectlyInvokeService_WhenCalledWithValidUserName()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var participantServiceMock = new Mock<IParticipantService>();
            var mapperMock = new Mock<IMapper>();
            var factoryMock = new Mock<IUserFactory>();

            userServiceMock.Setup(x => x.GetLikedMovies(It.IsAny<UserModel>()))
                .Returns(new List<MovieModel>());

            var sut = new UserController(userServiceMock.Object, participantServiceMock.Object,
              mapperMock.Object, factoryMock.Object);

            // Act
            sut.GetLikedMovies("randomUserName");

            // Assert
            userServiceMock.Verify(x => x.GetLikedMovies(It.IsAny<UserModel>()), Times.Once);
        }
    }
}
