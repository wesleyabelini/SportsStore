﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Web.UI.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.Web.UI.Models;
using SportsStore.Web.UI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductID=1, Name="P1" },
                new Product{ProductID=2, Name="P2" },
                new Product{ProductID=3, Name="P3" },
                new Product{ProductID=4, Name="P4" },
                new Product{ProductID=5, Name="P5" }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //Arrange - define an html helper - we need to do this
            //in order to apply the extension method

            HtmlHelper myHelper = null;

            //Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            //Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //Assert
            Assert.AreEqual(@" <a class=""btn btn-default""href=""Page1"">1</a><a class=""btm btm-default btn-primary selected"" href=""
Page2"">2</a><a class""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //Arrage
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID=1, Name="P1"},
                new Product{ProductID=2, Name="P2"},
                new Product{ProductID=3, Name="P3"},
                new Product{ProductID=4, Name="P4"},
                new Product{ProductID=5, Name="P5"}
            });

            //Arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //Assert
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //Arrange
            // - create de mock repository

            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1", Category="Apples"},
                new Product { ProductID=2, Name="P2", Category="Apples"},
                new Product { ProductID=3, Name="P3", Category="Plums"},
                new Product { ProductID=4, Name="P4", Category="Oranges"},
            });

            //Arrange - create the controller
            NavController target = new NavController(mock.Object);

            //Act = get the set of categories
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //Arrange
            // - create the mock repository

            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID=1, Name="P1", Category="Apples"},
                new Product { ProductID=4, Name="P2", Category="Oranges"},
            });

            //Arrange - create the controller
            NavController target = new NavController(mock.Object);

            //Arrange - define the category to selected
            string categoryToSelect = "Apples";

            //Action
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //Assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //Arrange
            // - create the mock repository

            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID=1, Name="P1", Category="Cat1"},
                new Product { ProductID=2, Name="P2", Category="Cat2"},
                new Product { ProductID=3, Name="P3", Category="Cat1"},
                new Product { ProductID=4, Name="P4", Category="Cat2"},
                new Product { ProductID=5, Name="P5", Category="Cat3"},
            });

            //Arrange - create a controller and make the page size 3 items
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            //Action - test the product counts for different categories
            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            //Assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }

        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange - create a new cart
            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            //Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //Arrange - create a new cart
            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        public void Can_Remove_Line()
        {
            //Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            //Arrange - create a new cart
            Cart target = new Cart();

            //Arrange - add some products to the cart
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            //Act
            target.RemoveLine(p2);

            //Assert
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //Arrange  - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //Arrange - create a new cart
            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            //Assert
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //Arrange - create a new cart
            Cart target = new Cart();

            //Arrange - add some items
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            //Act - reset the cart
            target.Clear();

            //Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        //[TestMethod]
        //public void Can_Add_To_Cart()
        //{
        //    //Arrange - create the mock repository
        //    Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
        //    mock.Setup(m => m.Products).Returns(new Product[]
        //    {
        //        new Product { ProductID=1, Name="P1", Category="Apples"},
        //    }.AsQueryable());

        //    //Arrange - create a Cart
        //    Cart cart = new Cart();

        //    //Arrange - create the controller
        //    CartController target = new CartController(mock.Object);

        //    //Act - add a product to the cart
        //    target.AddToCart(cart, 1, null);

        //    //Assert
        //    Assert.AreEqual(cart.Lines.Count(), 1);
        //    Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        //}

        //[TestMethod]
        //public void Adding_Product_To_Cart_Screen()
        //{
        //    //Arrange - create the mock repository
        //    Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
        //    mock.Setup(m => m.Products).Returns(new Product[]
        //    {
        //        new Product { ProductID=1, Name="P1", Category="Apples"},
        //    }.AsQueryable());

        //    //Arrange - create a Cart
        //    Cart cart = new Cart();

        //    //Arrange - create the controller
        //    CartController target = new CartController(mock.Object);

        //    //Act - add a product to the cart
        //    RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

        //    //Assert
        //    Assert.AreEqual(result.RouteValues["action"], "Index");
        //    Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        //}

        //[TestMethod]
        //public void Can_View_Cart_Contents()
        //{
        //    //Arrange - create a Cart
        //    Cart cart = new Cart();

        //    //Arrange - create the controller
        //    CartController target = new CartController(null);

        //    //Act - call the Index action method
        //    CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

        //    //Assert
        //    Assert.AreSame(result.Cart, cart);
        //    Assert.AreEqual(result.ReturnUrl, "myUrl");
        //}

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //Arrange - create an empty cart
            Cart cart = new Cart();

            //Arrange - create a shipping details
            ShippingDetails shippingDetails = new ShippingDetails();

            //Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            //Act
            ViewResult result = target.Checkout(cart, shippingDetails);

            //Assert - check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            //Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);

            //Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            //Arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");

            //Act - try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //Assert - check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            //Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);

            //Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            //Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //Arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            //Act - try to chackout
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //Assert - check that the order has been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());

            //Assert - check that the method is returning the Completed view
            Assert.AreEqual("Completed", result.ViewName);

            //Assert - check that I am passing a valid model to thte view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
