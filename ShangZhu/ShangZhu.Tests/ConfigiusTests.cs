using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShangZhu.Tests
{
    [Ignore]
    [TestClass]
    public class ConfigiusTests
    {
        [TestMethod]
        public void Get_Should_Return_Empty()
        {
            IConfigius configius = ShangZhu.Connect("", "");
            string value = configius.Get("test");
        }
    }
}
