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
var edmModel = modelBuilder.GetEdmModel();
edmModel.MarkAsImmutable();

builder.Services.AddControllers()
    .AddOData(opt =>
    {
        opt.EnableQueryFeatures();
        
        opt.AddRouteComponents("", edmModel, routeServices =>
        {
            routeServices.AddSingleton(new ODataMessageWriterSettings
            {
                Validations = ValidationKinds.None
            });
            routeServices.AddSingleton<ODataBatchHandler, DefaultODataBatchHandler>();
            routeServices.AddSingleton<IStreamBasedJsonWriterFactory>(_ =>
                DefaultStreamBasedJsonWriterFactory.Default);
        });
    });

builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

var app = builder.Build();

// app.MapControllers();

app
    .UseResponseCompression()
    .UseODataQueryRequest()
    .UseODataBatching()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(routeBuilder => routeBuilder.MapControllers());

app.Run();