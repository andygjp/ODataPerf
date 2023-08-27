namespace ODataPerf.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

public class ODataWeatherForecastWithQueryController : ODataController
{
    [HttpGet]
    [EnableQuery]
    public IEnumerable<WeatherForecast> Get()
    {
        return Data.Get();
    }
}