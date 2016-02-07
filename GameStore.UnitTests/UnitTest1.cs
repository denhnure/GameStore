using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.HtmlHelpers;
using GameStore.WebUI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GameStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CanPaginate()
        {
            var mock = new Mock<IGameRepository>();
            
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1"},
                new Game {GameId = 2, Name = "Game2"},
                new Game {GameId = 3, Name = "Game3"},
                new Game {GameId = 4, Name = "Game4"},
                new Game {GameId = 5, Name = "Game5"}
            });

            var controller = new GameController(mock.Object) {pageSize = 3};

            var result = (GameListViewModel) controller.List(null, 2).Model;

            var games = result.Games.ToList();
            Assert.IsTrue(games.Count == 2);
            Assert.AreEqual(games[0].Name, "Game4");
            Assert.AreEqual(games[1].Name, "Game5");
        }

        [TestMethod]
        public void CanGeneratePageLinks()
        {
            HtmlHelper myHelper = null;

            var pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                            + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                result.ToString());
        }

        [TestMethod]
        public void CanSendPaginationViewModel()
        {
            var mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1"},
                new Game {GameId = 2, Name = "Game2"},
                new Game {GameId = 3, Name = "Game3"},
                new Game {GameId = 4, Name = "Game4"},
                new Game {GameId = 5, Name = "Game5"}
            });

            var controller = new GameController(mock.Object) { pageSize = 3 };

            var result = (GameListViewModel)controller.List(null, 2).Model;

            PagingInfo pageInfo = result.PagingInfo;

            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void CanFilterGames()
        {
            var mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1", Category = "Cat1"},
                new Game {GameId = 2, Name = "Game2", Category = "Cat2"},
                new Game {GameId = 3, Name = "Game3", Category = "Cat1"},
                new Game {GameId = 4, Name = "Game4", Category = "Cat2"},
                new Game {GameId = 5, Name = "Game5", Category = "Cat3"}
            });

            var controller = new GameController(mock.Object) { pageSize = 3 };

            var result = ((GameListViewModel)controller.List("Cat2", 1).Model).Games.ToList();

            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Game2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "Game4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void CanCreateCategories()
        {
            var mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1", Category = "Simulator"},
                new Game {GameId = 2, Name = "Game2", Category = "Simulator"},
                new Game {GameId = 3, Name = "Game3", Category = "Shooter"},
                new Game {GameId = 4, Name = "Game4", Category = "RPG"}
            });

            var target = new NavController(mock.Object);

            var result = ((IEnumerable<string>)target.Menu().Model).ToList();

            Assert.AreEqual(result.Count(), 3);
            Assert.IsTrue(result[0] == "RPG");
            Assert.IsTrue(result[1] == "Shooter");
            Assert.IsTrue(result[2] == "Simulator");
        }

        [TestMethod]
        public void IndicatesSelectedCategory()
        {
            var mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1", Category = "Simulator"},
                new Game {GameId = 2, Name = "Game2", Category = "Shooter"}
            });

            var target = new NavController(mock.Object);

            const string categoryToSelect = "Shooter";

            var result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void GenerateCategorySpecificGameCount()
        {
            var mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1", Category = "Cat1"},
                new Game {GameId = 2, Name = "Game2", Category = "Cat2"},
                new Game {GameId = 3, Name = "Game3", Category = "Cat1"},
                new Game {GameId = 4, Name = "Game4", Category = "Cat2"},
                new Game {GameId = 5, Name = "Game5", Category = "Cat3"}
            });

            var controller = new GameController(mock.Object) { pageSize = 3 };

            int res1 = ((GameListViewModel) controller.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((GameListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((GameListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((GameListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
