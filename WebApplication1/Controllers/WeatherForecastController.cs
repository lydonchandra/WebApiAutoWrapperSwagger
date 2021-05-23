using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Controllers;

//[assembly: ApiConventionType( typeof( MyAppConventions ) )]
namespace WebApplication1.Controllers
{
    public static class MyAppConventions
    {
        [ProducesResponseType( 200, Type = typeof( ResponseWrapper<IEnumerable<WeatherForecast>> ) )]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
        public static void Get ()
        {
        }
    }

    public class ResponseWrapper<T>
    {
        public string Message { get; set; }

        public T Result { get; set; }
    }

    public class ResponseWrapper2
    {
        public string Message { get; set; }

        public string Result { get; set; }
    }

    [ApiController]
    [Route( "[controller]" )]    
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController ( ILogger<WeatherForecastController> logger )
        {
            _logger = logger;
        }

        [HttpGet]
        //[ProducesResponseType( 200, Type = typeof( ResponseWrapper<IEnumerable<WeatherForecast>> ) )]
        public IEnumerable<WeatherForecast> Get ()
        {
            var rng = new Random();
            return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays( index ),
                TemperatureC = rng.Next( -20, 55 ),
                Summary = Summaries[rng.Next( Summaries.Length )]
            } )
            .ToArray();
        }

        [HttpGet("get2")]
        public WeatherForecast Get2 ()
        {
            var rng = new Random();
            return new WeatherForecast
            {
                Date = DateTime.Now.AddDays( 1 ),
                TemperatureC = rng.Next( -20, 55 ),
                Summary = Summaries[rng.Next( Summaries.Length )]
            };
            
        }
    }
}
