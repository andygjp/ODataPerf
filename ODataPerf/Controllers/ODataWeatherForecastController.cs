namespace ODataPerf.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

public class ODataWeatherForecastController : ODataController
{
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Data.Get();
    }
}