namespace ODataPerf.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

public class ODataWeatherForecastController : ODataController
{
    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Data.Get();
    }
    
    [EnableQuery]
    [HttpGet("[controller]/Queryable")]
    public IEnumerable<WeatherForecast> Query()
    {
        return Data.Get();
    }
}