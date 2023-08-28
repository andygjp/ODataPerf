# README

I wanted to understand how AspNetCoreOData affected the performance of an AspNetCore project, by comparing
a "plain AspNetCore controller" to an ODataController, and different features, on my local machine.

# Setup

1. Install [crank](https://github.com/dotnet/crank/blob/main/docs/getting_started.md#installing-crank). (The same tooling [AspNetCore Benchmarks](https://github.com/aspnet/Benchmarks) uses.)
2. Start `crank-agent`
3. Run a test scenario (see [Tests](#tests))

# Tests

The tests make http calls to 1 of 4 controllers:

1. [`WeatherForecastController`](ODataPerf/Controllers/WeatherForecastController.cs) - plain AspNetCore controller that is generated when creating an empty project.
2. [`ODataWeatherForecastController`](ODataPerf/Controllers/ODataWeatherForecastController.cs) - controller that derives from ODataController.
3. [`ODataWeatherForecastWithQueryController`](ODataPerf/Controllers/ODataWeatherForecastWithQueryController.cs) - like the controller above, except it supports querying.
3. [`ODataWeatherForecastWithPagingController`](ODataPerf/Controllers/ODataWeatherForecastWithPagingController.cs) - like the controller above, except `PageSize` is set to 10.

In every case, the endpoints return a collection containing 5 `WeatherForecast` objects.

All the commands below are ran from the root of the repo.

```shell
# can run many test runs at once by specifying --iterations option
crank --config Tests/local.benchmarks.yml --description "Plain (/WeatherForecast)" \
  --scenario plain --profile local --json Tests/Results/plain-results.json
```

```shell
crank --config Tests/local.benchmarks.yml --description "OData (/ODataWeatherForecast)" \
  --scenario odata --profile local --json Tests/Results/odata-results.json
```

```shell
crank --config Tests/local.benchmarks.yml --description "Queryable (/ODataWeatherForecastWithQuery)" \
  --scenario odata-queryable --profile local --json Tests/Results/odata-queryable-results.json
```

```shell
crank --config Tests/local.benchmarks.yml --description "With-select (/ODataWeatherForecastWithQuery?\$select=Id)" \
  --scenario odata-queryable-with-select --profile local --json Tests/Results/odata-queryable-with-select-results.json
```

```shell
crank --config Tests/local.benchmarks.yml --description "Paged (/ODataWeatherForecastWithPaging)" \
  --scenario odata-paged --profile local --json Tests/Results/odata-paged-results.json
```

```shell
crank --config Tests/local.benchmarks.yml --description "Paged (/ODataWeatherForecastWithPaging ‘Prefer: maxpagesize=3’)" \
  --scenario odata-paged-max --profile local --json Tests/Results/odata-paged-max-results.json
```

```shell
crank --config Tests/local.benchmarks.yml --description "Paged (/ODataWeatherForecastWithPaging?\$select=Id ‘Prefer: maxpagesize=3’)" \
  --scenario odata-paged-max-and-select --profile local --json Tests/Results/odata-paged-max-and-select-results.json
```

The scenarios are defined within [local.benchmarks.yml](Tests/local.benchmarks.yml). Each run comprises of 30 second
warm-up and 120 second load test.

# Results

These results are from my local machine - an Apple M1 Max Mac Studio with 32 GB RAM running MacOS Ventura 13.5.1.

|                        | Plain (/WeatherForecast) | OData (/ODataWeatherForecast) | Queryable (/ODataWeatherForecastWithQuery) | With-select (/ODataWeatherForecastWithQuery?$select=Id) | Paged (/ODataWeatherForecastWithPaging) | Paged (/ODataWeatherForecastWithPaging ‘Prefer: maxpagesize=3’) | Paged (/ODataWeatherForecastWithPaging?$select=Id ‘Prefer: maxpagesize=3’) |
|------------------------|--------------------------|-------------------------------|--------------------------------------------|---------------------------------------------------------|-----------------------------------------|-----------------------------------------------------------------|----------------------------------------------------------------------------|
| Build Time (ms)        | 1,512                    | 1,490                         | 1,513                                      | 1,390                                                   | 1,349                                   | 1,327                                                           | 1,461                                                                      |
| Start Time (ms)        | 322                      | 302                           | 283                                        | 284                                                     | 213                                     | 290                                                             | 273                                                                        |
| Published Size (KB)    | 78,872                   | 78,872                        | 78,872                                     | 78,872                                                  | 78,872                                  | 78,872                                                          | 78,872                                                                     |
| Symbols Size (KB)      | 0                        | 0                             | 0                                          | 0                                                       | 0                                       | 0                                                               | 0                                                                          |
| .NET Core SDK Version  | 7.0.400                  | 7.0.400                       | 7.0.400                                    | 7.0.400                                                 | 7.0.400                                 | 7.0.400                                                         | 7.0.400                                                                    |
| ASP.NET Core Version   | 7.0.10+5a4c82ec57fa      | 7.0.10+5a4c82ec57fa           | 7.0.10+5a4c82ec57fa                        | 7.0.10+5a4c82ec57fa                                     | 7.0.10+5a4c82ec57fa                     | 7.0.10+5a4c82ec57fa                                             | 7.0.10+5a4c82ec57fa                                                        |
| .NET Runtime Version   | 7.0.10+a6dbb800a477      | 7.0.10+a6dbb800a477           | 7.0.10+a6dbb800a477                        | 7.0.10+a6dbb800a477                                     | 7.0.10+a6dbb800a477                     | 7.0.10+a6dbb800a477                                             | 7.0.10+a6dbb800a477                                                        |
| First Request (ms)     | 224                      | 274                           | 287                                        | 332                                                     | 232                                     | 322                                                             | 333                                                                        |
| Requests               | 8,989,030                | 5,506,676                     | 4,826,382                                  | 1,159,368                                               | 1,309,219                               | 1,274,447                                                       | 757,584                                                                    |
| Bad responses          | 0                        | 0                             | 0                                          | 0                                                       | 0                                       | 0                                                               | 0                                                                          |
| Latency 50th (ms)      | 3.27                     | 5.41                          | 6.23                                       | 26.97                                                   | 23.77                                   | 24.39                                                           | 40.93                                                                      |
| Latency 75th (ms)      | 4.02                     | 6.68                          | 7.59                                       | 29.05                                                   | 25.50                                   | 26.09                                                           | 43.16                                                                      |
| Latency 90th (ms)      | 4.88                     | 7.87                          | 8.84                                       | 31.42                                                   | 27.58                                   | 28.07                                                           | 46.52                                                                      |
| Latency 95th (ms)      | 5.50                     | 8.70                          | 9.70                                       | 33.28                                                   | 29.18                                   | 29.76                                                           | 49.47                                                                      |
| Latency 99th (ms)      | 7.41                     | 11.35                         | 12.50                                      | 38.30                                                   | 34.04                                   | 34.66                                                           | 58.67                                                                      |
| Mean latency (ms)      | 3.42                     | 5.58                          | 6.36                                       | 26.50                                                   | 23.46                                   | 24.11                                                           | 40.55                                                                      |
| Max latency (ms)       | 127.30                   | 132.84                        | 131.03                                     | 60.91                                                   | 69.92                                   | 57.45                                                           | 85.21                                                                      |
| Requests/sec           | 74,921                   | 45,888                        | 40,240                                     | 9,698                                                   | 10,941                                  | 10,656                                                          | 6,376                                                                      |
| Requests/sec (max)     | 90,899                   | 57,749                        | 55,313                                     | 17,838                                                  | 20,164                                  | 19,601                                                          | 16,732                                                                     |
| Read throughput (MB/s) | 41.56                    | 27.97                         | 24.86                                      | 3.37                                                    | 6.75                                    | 6.14                                                            | 2.73                                                                       |

Each column is the output printed in the console after a run. The column header is the friendly name, followed by the request
path and any request headers inside parenthesis, eg: Paged (/ODataWeatherForecastWithPaging ‘Prefer: maxpagesize=3’). The
header matches the description used in the commands [above](#tests).

The [results](Tests/Results) directory contains samples of the JSON output generated during a run.

# Analysis

I'll just focus on the mean latency and requests/sec when comparing columns. The mean latency and requests/sec for "Plain"
is 3.42ms and 74,921, respectively. Compared to "OData" which is 3.42ms and 45,888 - the OData controller is slower.

Adding queryable support (`[EnableQuery]`) to an OData controller, the "Queryable" column, increases mean latency to 6.36ms
and decreases requests/sec to 40,240 - about 5000 fewer requests by adding `EnableQueryAttribute` to the action.

Next, "With-select" calls the same controller and specifies a query: `/ODataWeatherForecastWithQuery?$select=Id`, which causes
mean latency to increase to 26.50ms and deceases requests/sec to 9,698. Comparing that with "Paged", which is a different
controller, but still one that derives from `ODataController` and supports server paged query, `[EnableQuery(PageSize = 10)]`,
has similar performance (mean latency 69.92ms and 10,941 requests/sec).

# Conclusions

I did expect that an OData controller would be slower than a plain controller, but I didn't expect that adding query support
would make it worse. A queryable endpoint completes about half the number of requests per second of a plain controller!
Worse, if I set `EnableQueryAttribute.PageSize`, then the number of requests per second falls to about an 1/8th of a plain
controller. Thats quite concerning because I set the PageSize on all my controllers!