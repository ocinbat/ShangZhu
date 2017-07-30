using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShangZhu.Tests
{
    [TestClass]
    [Ignore]
    public class ConfigiusTests
    {
        [TestMethod]
        public void Get_Should_Return_Empty()
        {
            IConfigius configius = Spirit.Connect("168529f4-5b2f-4290-9dd9-e3b2c6f643ad", "asdfasdf", "test", "http://localhost:51311/");
            string value = configius.Get("super_config");
        }
    }
}
