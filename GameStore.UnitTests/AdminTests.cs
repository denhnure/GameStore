﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GameStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void IndexContainsAllGames()
        {
            var mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Game1"},
                new Game { GameId = 2, Name = "Game2"},
                new Game { GameId = 3, Name = "Game3"},
                new Game { GameId = 4, Name = "Game4"},
                new Game { GameId = 5, Name = "Game5"}
            });

            var controller = new AdminController(mock.Object);

            List<Game> result = ((IEnumerable<Game>) controller.Index().ViewData.Model).ToList();

            Assert.AreEqual(result.Count(), 5);
            Assert.AreEqual("Game1", result[0].Name);
            Assert.AreEqual("Game2", result[1].Name);
            Assert.AreEqual("Game3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Game()
        {
            var mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1"},
                new Game {GameId = 2, Name = "Игра2"},
                new Game {GameId = 3, Name = "Игра3"},
                new Game {GameId = 4, Name = "Игра4"},
                new Game {GameId = 5, Name = "Игра5"}
            });

            var controller = new AdminController(mock.Object);

            var game1 = controller.Edit(1).ViewData.Model as Game;
            var game2 = controller.Edit(2).ViewData.Model as Game;
            var game3 = controller.Edit(3).ViewData.Model as Game;

            Assert.AreEqual(1, game1.GameId);
            Assert.AreEqual(2, game2.GameId);
            Assert.AreEqual(3, game3.GameId);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Game()
        {
            var mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1"},
                new Game {GameId = 2, Name = "Игра2"},
                new Game {GameId = 3, Name = "Игра3"},
                new Game {GameId = 4, Name = "Игра4"},
                new Game {GameId = 5, Name = "Игра5"}
            });

            var controller = new AdminController(mock.Object);

            var result = controller.Edit(6).ViewData.Model as Game;

            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            var mock = new Mock<IGameRepository>();

            var controller = new AdminController(mock.Object);

            var game = new Game { Name = "Test" };

            ActionResult result = controller.Edit(game);

            mock.Verify(m => m.SaveGame(game));

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            var mock = new Mock<IGameRepository>();

            var controller = new AdminController(mock.Object);

            var game = new Game { Name = "Test" };

            controller.ModelState.AddModelError("error", "error");

            ActionResult result = controller.Edit(game);

            mock.Verify(m => m.SaveGame(It.IsAny<Game>()), Times.Never());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Games()
        {
            var game = new Game {GameId = 2, Name = "Игра2"};

            var mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Игра1"},
                new Game {GameId = 2, Name = "Игра2"},
                new Game {GameId = 3, Name = "Игра3"},
                new Game {GameId = 4, Name = "Игра4"},
                new Game {GameId = 5, Name = "Игра5"}
            });

            var controller = new AdminController(mock.Object);

            controller.Delete(game.GameId);

            mock.Verify(m => m.DeleteGame(game.GameId));
        }
    }
}
