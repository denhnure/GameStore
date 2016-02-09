using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GameStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void CanAddNewLines()
        {
            var game1 = new Game {GameId = 1, Name = "Game1"};
            var game2 = new Game {GameId = 2, Name = "Game2"};

            var cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);

            List<CartLine> results = cart.Lines.ToList();

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results[0].Game, game1);
            Assert.AreEqual(results[1].Game, game2);
        }

        [TestMethod]
        public void CanAddQuantityForExistingLines()
        {
            var game1 = new Game { GameId = 1, Name = "Game1" };
            var game2 = new Game { GameId = 2, Name = "Game2" };

            var cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 5);

            List<CartLine> results = cart.Lines.OrderBy(c => c.Game.GameId).ToList();

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results[0].Quantity, 6);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void CanRemoveLine()
        {
            var game1 = new Game { GameId = 1, Name = "Game1" };
            var game2 = new Game { GameId = 2, Name = "Game2" };
            var game3 = new Game { GameId = 3, Name = "Game3" };

            var cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 4);
            cart.AddItem(game3, 2);
            cart.AddItem(game2, 1);

            cart.RemoveLine(game2);

            Assert.AreEqual(cart.Lines.Count(c => c.Game == game2), 0);
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void CalculateCartTotal()
        {
            var game1 = new Game { GameId = 1, Name = "Game1", Price = 100 };
            var game2 = new Game { GameId = 2, Name = "Game2", Price = 55 };

            var cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 5);

            decimal result = cart.ComputeTotalValue();

            Assert.AreEqual(result, 655);
        }

        [TestMethod]
        public void CanClearContents()
        {
            var game1 = new Game { GameId = 1, Name = "Game1", Price = 100 };
            var game2 = new Game { GameId = 2, Name = "Game2", Price = 55 };

            var cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 5);
            cart.Clear();

            Assert.AreEqual(cart.Lines.Count(), 0);
        }

        [TestMethod]
        public void CanAddToCart()
        {
            var mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1", Category = "Cat1"}
            }.AsQueryable());

            var cart = new Cart();

            var controller = new CartController(mock.Object);

            controller.AddToCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToList()[0].Game.GameId, 1);
        }

        [TestMethod]
        public void AddingGameToCartGoesToCartScreen()
        {
            var mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game {GameId = 1, Name = "Game1", Category = "Cat1"}
            }.AsQueryable());

            var cart = new Cart();

            var controller = new CartController(mock.Object);

            RedirectToRouteResult result = controller.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void CanViewCartContents()
        {
            var cart = new Cart();

            var target = new CartController(null);

            var result = (CartIndexViewModel) target.Index(cart, "myUrl").ViewData.Model;

            Assert.AreEqual(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
    }
}
