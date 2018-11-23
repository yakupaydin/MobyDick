using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MobyDickShowWords;
using MobyDickShowWords.Controllers;

namespace MobyDickShowWords.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void Index()
        {

            string path = System.Web.HttpContext.Current.Server.MapPath("~/XML/MobyDickText.xml");
            // Arrange

            var result = HomeController.ListofWords(path);
            Console.WriteLine("Result: {0}",result.Count());



            // Assert
            Assert.AreEqual(10, result.Count());
        }
    }
}
