namespace HiBidAPI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using ClosedXML.Excel;
    using HiBidAPI.Models.HiBid;
    using HtmlAgilityPack;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class HiBidSchedulerService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10); // Set the interval as needed
        private readonly List<HiBidSearch> _searches;

        public HiBidSchedulerService(IServiceProvider serviceProvider, List<HiBidSearch> searches)
        {
            _serviceProvider = serviceProvider;
            _searches = searches;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, _interval);
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var hiBidScraper = scope.ServiceProvider.GetRequiredService<HiBidScraper>();
                Dictionary<string, List<HiBidAuctionItem>> allItems = new Dictionary<string, List<HiBidAuctionItem>>();
                foreach (var search in _searches)
                {
                    try
                    {
                        var links = hiBidScraper.FindAuctions(search);
                        var matchingItems = new List<HiBidAuctionItem>();
                        foreach (var link in links)
                        {
                            try
                            {
                                var auctionItems = hiBidScraper.GetAuctionItems(link);
                                matchingItems.AddRange(auctionItems.FindAll(x => x.Title.Contains(search.SearchTerm, StringComparison.OrdinalIgnoreCase)));
                            }
                            catch (Exception ex)
                            {
                                // TODO add logger
                                continue;
                            }
                        }
                        allItems.TryAdd(search.SearchTerm, matchingItems);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }

                BuildExcelDocument(allItems, $@"c:\TestItems\text{DateTime.Now.ToLongDateString}.xlsx");
            }
        }

        public void BuildExcelDocument(Dictionary<string, List<HiBidAuctionItem>> data, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var addedItems = new HashSet<string>(); 

                foreach (var key in data.Keys)
                {
                    var items = data[key].Where(item => addedItems.Add(item.Title)).ToList(); 

                    var worksheet = workbook.Worksheets.Add(key);

                    // Adding headers
                    worksheet.Cell(1, 1).Value = "Title";
                    worksheet.Cell(1, 2).Value = "Current Bid";
                    worksheet.Cell(1, 3).Value = "Number of Bids";
                    worksheet.Cell(1, 4).Value = "Href";
                    worksheet.Cell(1, 5).Value = "Time Left String";
                    worksheet.Cell(1, 6).Value = "Time Left";
                    worksheet.Cell(1, 7).Value = "End Time";

                    // Adding data
                    for (int i = 0; i < items.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = items[i].Title;
                        worksheet.Cell(i + 2, 2).Value = items[i].CurrentBid;
                        worksheet.Cell(i + 2, 3).Value = items[i].NumberOfBids;
                        worksheet.Cell(i + 2, 4).Value = items[i].Href;
                        worksheet.Cell(i + 2, 5).Value = items[i].TimeLeftStr;
                        worksheet.Cell(i + 2, 6).Value = items[i].TimeLeft.ToString();
                        worksheet.Cell(i + 2, 7).Value = items[i].EndTime.ToString();
                    }

                    worksheet.Columns().AdjustToContents();
                }

                workbook.SaveAs(filePath);
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
