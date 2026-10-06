using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyApp.Web.Tests
{
    [TestClass]
    public class StartupTests
    {
        [TestMethod]
        [DataRow("/api/profile", true)]
        [DataRow("/api", true)]
        [DataRow("/API/content", true)]
        [DataRow("/apis", false)]
        [DataRow("/account/login", false)]
        public void IsApiRequest_MatchesApiSegment(string path, bool expected)
        {
            var context = new OwinContext();
            context.Request.Path = new PathString(path);

            Assert.AreEqual(expected, Startup.IsApiRequest(context.Request));
        }
    }
}
