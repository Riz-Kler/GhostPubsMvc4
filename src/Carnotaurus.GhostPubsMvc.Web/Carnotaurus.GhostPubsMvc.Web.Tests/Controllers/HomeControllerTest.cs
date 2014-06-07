//using Carnotaurus.GhostPubsMvc.Web.Controllers;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Web.Mvc;

//namespace Carnotaurus.GhostPubsMvc.Web.Tests.Controllers
//{
//    [TestClass]
//    public class HomeControllerTest
//    {
//        [TestMethod]
//        public void About()
//        {
//            // Arrange
//            HomeController controller = new HomeController();

//            // Act
//            ViewResult result = controller.About() as ViewResult;

//            // Assert
//            Assert.IsNotNull(result);
//        }

//        [TestMethod]
//        public void Contact()
//        {
//            // Arrange
//            HomeController controller = new HomeController();

//            // Act
//            ViewResult result = controller.Contact() as ViewResult;

//            // Assert
//            Assert.IsNotNull(result);
//        }

//        [TestMethod]
//        public void Index()
//        {
//            // Arrange
//            HomeController controller = new HomeController();

//            // Act
//            ViewResult result = controller.Index() as ViewResult;

//            // Assert
//            Assert.AreEqual("Modify this template to jump-start your ASP.NET MVC application.", result.ViewBag.Message);
//        }
//    }
//}