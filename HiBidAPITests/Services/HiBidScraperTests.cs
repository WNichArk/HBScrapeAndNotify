using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiBidAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiBidAPI.Services.Tests
{
    [TestClass()]
    public class HiBidScraperTests
    {
        [TestMethod()]
        public void GetHiBidCookiesTest()
        {
            var scraper = new HiBidScraper(new TwilioService());
            var cookies = scraper.PopulateHibidHomeCookies();
            Assert.Fail();
        }
    }
}