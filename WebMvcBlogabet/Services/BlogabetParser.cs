using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebMvcBlogabet.DataStorage;
using WebMvcBlogabet.Models;
using System.Linq;

namespace WebMvcBlogabet.Services
{
    public class BlogabetParser
    {
        private readonly int _timeSleep = 7000;
        private readonly int _countSeeOld = 10;
        private CancellationToken cancellationToken;
        private BlogabetWeb _web;

        public BlogabetParser(CancellationToken token)
        {
            cancellationToken = token;
            _web = new BlogabetWeb();
        }

        private readonly int countError = 5;
        private static int currentErr = 0;
        private static bool isFirst = true;

        public async Task Run()
        {
            try
            {
                label1:
                var stateLogin = await _web.Login();
                var timeEnd = DateTime.Now.AddMinutes(2);

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if(DateTime.Now > timeEnd)
                    {
                        await _web.Logout();
                        await Task.Delay(30000);
                        goto label1;
                    }

                    var resultContent = await _web.UpdatePage();
                    var list = ParsePage(resultContent);
                    ForecastDataStorage.Add(list);

                    if (isFirst)
                    {
                        for (int i = 0; i < _countSeeOld; i++)
                        {
                            var point = GetIdTime(resultContent);
                            if(string.IsNullOrEmpty(point))
                            {
                                break;
                            }

                            resultContent = await _web.SeeOld(point);
                            list = ParsePage(resultContent);
                            ForecastDataStorage.Add(list);
                        }

                        isFirst = false;
                    }

                    await Task.Delay(_timeSleep, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                currentErr++;
                if (countError < currentErr)
                {
                    await Task.Delay(100000);
                    await Run();
                }
                else
                {
                    throw;
                }
            }
        }

        private string GetIdTime(HtmlDocument html)
        {
            if(html.Text.Contains("See older"))
            {
                return html.DocumentNode.SelectSingleNode("//input[@id='last_comment']").GetAttributeValue("value", String.Empty);
            }
            else
            {
                return String.Empty;
            }
        }

        private List<ForecastData> ParsePage(HtmlDocument html)
        {
            List<ForecastData> forecastData = new List<ForecastData>();

            try
            {
                var listForecast = html.DocumentNode.SelectNodes("//li");

                foreach (var item in listForecast)
                {
                    if (!item.InnerHtml.Contains("u-dp data-info"))
                    {
                        continue;
                    }

                    ForecastData data = new ForecastData();

                    try
                    {
                        var info = item.SelectSingleNode(".//span[@class='u-dp data-info']").InnerText.TrimStart().TrimEnd().Split('%');
                        Int32.TryParse(info[0].Replace("-", "").Replace("+", ""), out var percent);
                        Int32.TryParse(info[1].Replace("(", "").Replace(")", ""), out var countBet);

                        data.NameBetter = item.SelectSingleNode(".//div[@class='title-name']/div/a").InnerText.TrimStart().TrimEnd();
                        data.NameBet = item.SelectSingleNode(".//div[@class='pick-line']").InnerText.TrimStart().TrimEnd();
                        data.Percent = info[0].Contains("-") ? 0 - percent : percent;
                        data.CountBet = countBet;
                        data.HtmlPayload = item.SelectSingleNode(".//div[@class='feed-pick-title']").InnerHtml.TrimStart().TrimEnd();
                        data.TimeBet = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
                      //  data.Coeff = item.SelectSingleNode(".//span[@class='feed-odd']").InnerText;

                        if (!item.InnerText.Contains("Show details"))
                        {
                            var timeStartMatch = item.SelectSingleNode(".//small[@class='text-muted']").InnerText.Split('\n').Where(x=>x.Contains("Kick off")).First().TrimStart().TrimEnd().Replace("Kick off: ","");
                            data.TimeEndBet = DateTime.Parse(timeStartMatch).AddHours(2);
                        }
                        else
                        {
                            data.TimeEndBet = DateTime.Now.AddHours(5);
                        }

                        forecastData.Add(data);
                    }
                    catch { }
                }

                return forecastData;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
