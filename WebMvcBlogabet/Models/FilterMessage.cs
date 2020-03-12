using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMvcBlogabet.Models
{
    public class FilterMessage
    {
        public string Percent { get; set; }
        public string CountBet { get; set; }

        public FilterMessage(string percent, string countBet)
        {
            Percent = percent;
            CountBet = countBet;
        }
    }
}
