using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVCApp;

namespace MVCAppTest
{
    [TestClass]
    public class RouteTests
    {
        [ClassInitialize]
        public static void Init(TestContext contex)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        [TestMethod]
        public void RouteEmptyUrlMustRouteToIndex()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~\");
            var routeData = RouteTable.Routes.GetRouteData(context.Object);
            Assert.IsNotNull(routeData, "Route not found");
            Assert.AreEqual("Home", routeData.Values["controller"], "Wrong controller");
            Assert.AreEqual("Index", routeData.Values["action"], "Wrong action");
        }
        [TestMethod]
        public void RouteWithNoActionUseIndexAsDefault()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~\Notes");
            var routeData = RouteTable.Routes.GetRouteData(context.Object);
            Assert.IsNotNull(routeData, "Route not found");
            Assert.AreEqual("Notes", routeData.Values["controller"], "Wrong controller");
            Assert.AreEqual("Index", routeData.Values["action"], "Wrong action");
        }
    }
}
