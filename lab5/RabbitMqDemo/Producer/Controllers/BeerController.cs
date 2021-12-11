using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Producer.Services;
using Shared;

namespace Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BeerController : ControllerBase
    {
        private readonly ILogger<BeerController> _logger;
        private readonly BeerProducer _producer;

        public BeerController(ILogger<BeerController> logger, BeerProducer producer)
        {
            _logger = logger;
            _producer = producer;
        }

        [HttpPost]
        public void Produce(Beer beerSample)
        {
            _producer.Produce(beerSample);
        }
    }
}
