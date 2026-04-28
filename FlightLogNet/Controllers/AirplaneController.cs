namespace FlightLogNet.Controllers
{
    using System.Collections.Generic;

    using Facades;
    using Models;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [EnableCors]
    [Route("[controller]")]
    public class AirplaneController(ILogger<AirplaneController> logger, AirplaneFacade airplaneFacade)
        : ControllerBase
    {
        [HttpGet]
        public IEnumerable<AirplaneModel> Get()
        {
            logger.LogDebug("Get club airplanes.");
            return airplaneFacade.GetClubAirplanes();
        }
    }
}
