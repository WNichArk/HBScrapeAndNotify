using HiBidAPI.Models.HiBid;
using HtmlAgilityPack;
using System.Net.Http;
using System;
using System.Web;
using System.Text.RegularExpressions;
using System.Net;
using LiteDB;
using HiBidAPI.Services.Interfaces;

namespace HiBidAPI.Services
{
    public class HiBidScraper
    {
        public ICommService _commService;
        private ILiteDatabase _db;
        private HttpClient _httpClient = new HttpClient()
        {
            DefaultRequestHeaders =
            {
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:124.0) Gecko/20100101 Firefox/124.0" }
            }
        };
        private static CookieContainer _homeCookies = new CookieContainer();
        private static CookieContainer _searchCookies = new CookieContainer();

        public HiBidScraper(ICommService commService, ILiteDatabase db)
        {
            _commService = commService;
            _db = db;
        }

        public List<string> FindAuctions(HiBidSearch search)
        {
            var links = new List<string>();

            var url = BuildSearchURI(search);
            if(_homeCookies.Count == 0)
            {
                PopulateHibidHomeCookies();
            }
            var handler = new HttpClientHandler
            {
                CookieContainer = _homeCookies
            };

            //change url querystring miles to -1
            url = new UriBuilder(url) { Query = url.Query.Replace($"miles={search.Radius}", "miles=-1") }.Uri;

            //_httpClient = new HttpClient(handler);

            var res = _httpClient.GetAsync(url).Result;
            //add response cookies to search cookies
            //var cookies = res.Headers.GetValues("Set-Cookie");
            //foreach (var cookie in cookies)
            //{
            //    var split = cookie.Split(";");
            //    var cookieName = split[0].Split("=")[0];
            //    var cookieValue = split[0].Split("=")[1];
            //    var domain = url.Host;
            //    _searchCookies.Add(new Cookie(cookieName, cookieValue, "/", domain));
            //}

            //change url back
            url = new UriBuilder(url) { Query = url.Query.Replace("miles=-1", $"miles={search.Radius}") }.Uri;

            var response = _httpClient.GetStringAsync(url).Result;
            var pages = GetNumberOfPages(response);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);

            var auctionDivs = htmlDocument.DocumentNode.SelectNodes("//div[@class='auction-header-title list-group-item']/a");

            if (auctionDivs != null)
            {
                foreach (var div in auctionDivs)
                {
                    var href = div.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrEmpty(href))
                    {
                        links.Add($"https://hibid.com{href}");
                    }
                }
            }
            return links;
        }

        public List<HiBidAuctionItem> GetAuctionItems(string url)
        {
            var auctionItems = new List<HiBidAuctionItem>();
            var htmlDocument = new HtmlDocument();
            var mainHtmlContent = _httpClient.GetStringAsync(url).Result;
            var pages = GetNumberOfPages(mainHtmlContent);

            for (int i = 1; i <= pages; i++)
            {
                Random random = new Random();
                int sleepTime = random.Next(1000, 2000);
                System.Threading.Thread.Sleep(sleepTime);
                var res = _httpClient.GetAsync($"{url}?apage={i}").Result;
                htmlDocument.LoadHtml(res.Content.ReadAsStringAsync().Result);
                var lotTiles = htmlDocument.DocumentNode.SelectNodes("//app-lot-tile");
                
                //add _searchCookies to request
                var handler = new HttpClientHandler
                {
                    CookieContainer = _searchCookies
                };
                _httpClient = new HttpClient(handler);

                if (lotTiles != null)
                {
                    foreach (var tile in lotTiles)
                    {
                        var titleNode = tile.SelectSingleNode(".//h2[@class='lot-title']");
                        var currentBidNode = tile.SelectSingleNode(".//span[contains(@class, 'lot-high-bid')]");
                        var numberOfBidsNode = tile.SelectSingleNode(".//a[contains(@class, 'lot-bid-history')]");
                        var hrefNode = tile.SelectSingleNode(".//a[@class='lot-link lot-preview-link link']");
                        var timeLeftNode = tile.SelectSingleNode("//div[contains(@class, 'lot-time-left')]");
                        var timeLeft = timeLeftNode?.InnerText.Trim() ?? "N/A";

                        if (titleNode != null && currentBidNode != null && numberOfBidsNode != null && hrefNode != null)
                        {
                            var auctionItem = new HiBidAuctionItem
                            {
                                Title = titleNode.InnerText.Trim(),
                                CurrentBid = currentBidNode.InnerText.Trim(),
                                NumberOfBids = numberOfBidsNode.InnerText.Trim(),
                                Href = $"https://hibid.com{hrefNode.GetAttributeValue("href", string.Empty)}",
                                TimeLeftStr = timeLeft,
                                TimeLeft = ParseDuration(timeLeft),
                                EndTime = DateTime.Now.Add(ParseDuration(timeLeft))
                            };

                            auctionItems.Add(auctionItem);
                        }
                    }
                }
            }

            return auctionItems;
        }

        public void PopulateHibidHomeCookies()
        {
            var cookieContainer = new CookieContainer();
            var url = "https://hibid.com";
            var response = _httpClient.GetAsync(url).Result;
            var cookies = response.Headers.GetValues("Set-Cookie");
            foreach (var cookie in cookies)
            {
                var split = cookie.Split(";");
                var cookieName = split[0].Split("=")[0];
                var cookieValue = split[0].Split("=")[1];
                var domain = new Uri(url).Host;
                cookieContainer.Add(new Cookie(cookieName, cookieValue, "/", domain));
            }
            _homeCookies = cookieContainer;
        }

        //public CookieContainer GetHibidSearchCookies()
        //{

        //}

        public int GetNumberOfPages(string htmlContent)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            var pageLinks = htmlDocument.DocumentNode.SelectNodes("//a[contains(@class, 'page-link')]");

            int maxPage = 1;
            if (pageLinks != null)
            {
                foreach (var link in pageLinks)
                {
                    if (int.TryParse(link.InnerText, out int pageNumber))
                    {
                        if (pageNumber > maxPage)
                        {
                            maxPage = pageNumber;
                        }
                    }
                }
            }

            return maxPage;
        }

        public static TimeSpan ParseDuration(string input)
        {
            // Regular expression to match the pattern "Xd Yh Zm Ws"
            Regex regex = new Regex(@"((?<days>\d+)d\s*)?((?<hours>\d+)h\s*)?((?<minutes>\d+)m\s*)?((?<seconds>\d+)s\s*)?");
            Match match = regex.Match(input);

            int days = 0, hours = 0, minutes = 0, seconds = 0;

            if (match.Success)
            {
                if (match.Groups["days"].Success)
                {
                    days = int.Parse(match.Groups["days"].Value);
                }
                if (match.Groups["hours"].Success)
                {
                    hours = int.Parse(match.Groups["hours"].Value);
                }
                if (match.Groups["minutes"].Success)
                {
                    minutes = int.Parse(match.Groups["minutes"].Value);
                }
                if (match.Groups["seconds"].Success)
                {
                    seconds = int.Parse(match.Groups["seconds"].Value);
                }
            }

            return new TimeSpan(days, hours, minutes, seconds);
        }

        private Uri BuildSearchURI(HiBidSearch search)
        {
            var baseUrl = "https://hibid.com/auctions";
            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["q"] = search.SearchTerm;
            query["zip"] = search.ZipCode;
            query["miles"] = search.Radius.ToString();

            if (search.ShippingOffered)
            {
                query["shippingoffered"] = search.ShippingOffered.ToString().ToLower();
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }
    }
}
