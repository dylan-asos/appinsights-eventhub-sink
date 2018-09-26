using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace AppInsights.EventHub.Sink.NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static readonly TelemetryClient client = new TelemetryClient();

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            client.TrackEvent("TestEvent");

            return new[] {"value1", "value2"};
        }
    }
}