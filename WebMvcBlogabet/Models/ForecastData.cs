using System;

namespace WebMvcBlogabet.Models
{
    public class ForecastData
    {
        public string NameBetter { get; set; }
        public string NameBet { get; set; }
        public string Coeff { get; set; }
        public int Percent { get; set; }
        public int CountBet { get; set; }
        public string HtmlPayload { get; set; }
        public DateTime TimeBet { get; set; }
        public DateTime TimeEndBet { get; set; }
    }
}
