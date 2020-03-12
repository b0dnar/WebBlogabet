using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMvcBlogabet.Models;

namespace WebMvcBlogabet.DataStorage
{
    public static class ForecastDataStorage
    {
        private static Dictionary<int, ForecastData> DataSet { get; } = new Dictionary<int, ForecastData>();

        private static bool _stateFilterPercent = false;
        private static bool _stateFilterCountBet = false;
        private static int _filterPercent;
        private static int _filterCountBet;
        
        static ForecastDataStorage()
        {
            var _cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                try
                {
                    var ct = _cts.Token;

                    while (true)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            return;
                        }

                        await Task.Delay(TimeSpan.FromMinutes(5), ct);

                        var expiredData = DataSet.Where(x => x.Value.TimeEndBet < TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"))).ToList();

                        foreach (var d in expiredData)
                        {
                            lock(DataSet)
                            {
                                DataSet.Remove(d.Key);
                            }
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception e)
                {
                  //  Log.Error(e, "ExpiringDataSet Periodic Clean Job");
                    throw;
                }
            });
        }

        public static void SetFilter(FilterMessage filterMessage)
        {
            if(!String.IsNullOrEmpty(filterMessage.Percent))
            {
                _stateFilterPercent = Int32.TryParse(filterMessage.Percent, out var percent);
                _filterPercent = percent;
            }
            else
            {
                _stateFilterPercent = false;
            }

            if (!String.IsNullOrEmpty(filterMessage.CountBet))
            {
                _stateFilterCountBet = Int32.TryParse(filterMessage.CountBet, out var countBet);
                _filterCountBet = countBet;
            }
            else
            {
                _stateFilterCountBet = false;
            }
        }

        public static void UnSetFilter()
        {
            _stateFilterPercent = false;
            _stateFilterCountBet = false;
        }

        public static List<ForecastData> GetItems()
        {
            lock(DataSet)
            {
                var items = DataSet.Select(x => x.Value).ToList();

                if (_stateFilterPercent && _stateFilterCountBet)
                {
                    return items.Where(x => x.Percent >= _filterPercent && x.CountBet >= _filterCountBet).Select(x => x).ToList();
                }
                else if (_stateFilterPercent)
                {
                    return items.Where(x => x.Percent >= _filterPercent).Select(x => x).ToList();
                }
                else if (_stateFilterCountBet)
                {
                    return items.Where(x => x.CountBet >= _filterCountBet).Select(x => x).ToList();
                }
                else
                {
                    return items;
                }
            }
        }

        public static void Add(IEnumerable<ForecastData> items)
        {
            lock (DataSet)
            {
                foreach (var item in items)
                {
                    var hash = GetHash(item);
                    if (!DataSet.ContainsKey(hash))
                    {
                        DataSet.Add(hash, item);
                    }
                }
            }
        }

        private static int GetHash(ForecastData data)
        {
            var time = data.TimeEndBet.ToShortTimeString();
            var myStr = $"{data.NameBetter}{data.NameBet}";

            return myStr.GetHashCode();
        }
    }
}
