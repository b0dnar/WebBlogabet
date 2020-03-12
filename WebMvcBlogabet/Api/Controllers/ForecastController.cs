using Microsoft.AspNetCore.Mvc;
using WebMvcBlogabet.DataStorage;
using WebMvcBlogabet.Models;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace WebMvcBlogabet.Api.Controllers
{
    [Route("api/[controller]")]
    public class ForecastController : Controller
    {

        [HttpGet]
        [Route("update")]
        public JsonResult Get()
        {
            var items = ForecastDataStorage.GetItems();
            items = items.OrderByDescending(x => x.TimeBet).ToList();

            return new JsonResult(JsonConvert.SerializeObject(items));
        }

        [HttpGet]
        [Route("nofilter")]
        public ActionResult GetCastFilter()
        {
            ForecastDataStorage.UnSetFilter();

            return StatusCode(200);
        }

        [HttpPost]
        [Route("filter")]
        public ActionResult Post([FromBody]FilterMessage message)
        {
            if (message == null)
            {
                return StatusCode(415);
            }

            ForecastDataStorage.SetFilter(message);

            return StatusCode(200);
        }
    }
}
