namespace ODataPerf.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

public class ODataWeatherForecastWithPagingController : ODataController
{
    [HttpGet]
    [EnableQuery(PageSize = 10)]
    public IEnumerable<WeatherForecast> Get()
    {
        return Data.Get();
    }
}