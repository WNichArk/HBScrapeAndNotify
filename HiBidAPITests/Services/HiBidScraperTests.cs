using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiBidAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace HiBidAPI.Services.Tests
{
    [TestClass()]
    public class HiBidScraperTests
    {
        [TestMethod()]
        public void GetHiBidCookiesTest()
        {
            var scraper = new HiBidScraper(new TwilioService(), new LiteDatabase(@"HiBid.db"));
            scraper.PopulateHibidHomeCookies();
            Assert.Fail();
        }
    }
}