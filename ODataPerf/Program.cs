#define USE_OPTIMISATIONS

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.ModelBuilder;
using ODataPerf;

var builder = WebApplication.CreateBuilder(args);

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<WeatherForecast>("ODataWeatherForecast");
modelBuilder.EntitySet<WeatherForecast>("ODataWeatherForecastWithQuery");
var edmModel = modelBuilder.GetEdmModel();

#if USE_OPTIMISATIONS
edmModel.MarkAsImmutable();
#endif

builder.Services.AddControllers()
    .AddOData(opt =>
    {
        opt.EnableQueryFeatures();
        
        opt.AddRouteComponents("", edmModel, routeServices =>
        {
            routeServices.AddSingleton<ODataBatchHandler, DefaultODataBatchHandler>();
#if USE_OPTIMISATIONS
            routeServices.AddSingleton(new ODataMessageWriterSettings
            {
                Validations = ValidationKinds.None
            });
            routeServices.AddSingleton<IStreamBasedJsonWriterFactory>(_ =>
                DefaultStreamBasedJsonWriterFactory.Default);
#endif
        });
    });

builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

var app = builder.Build();

app
    .UseResponseCompression()
    .UseODataRouteDebug()
    .UseODataQueryRequest()
    .UseODataBatching()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

app.Run();