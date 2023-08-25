# Setup

1. Install [crank](https://github.com/dotnet/crank/blob/main/docs/getting_started.md#installing-crank)
2. Start `crank-agent`
3. Run a test scenario (see [Tests](#tests))

# Tests

All commands ran from the root of the repo.

```shell
# plain asp.net core controller
crank --config Tests/local.benchmarks.yml --scenario plain --profile local --json Tests/Results/plain-results.json
```

```shell
# odata asp.net core controller
crank --config Tests/local.benchmarks.yml --scenario odata --profile local --json Tests/Results/odata-results.json
```

```shell
# odata asp.net core controller that supports querying
crank --config Tests/local.benchmarks.yml --scenario odata-queryable --profile local --json Tests/Results/odata-queryable-results.json
```

```shell
# odata asp.net core controller that supports querying with $select
crank --config Tests/local.benchmarks.yml --scenario odata-queryable-with-select --profile local --json Tests/Results/odata-queryable-with-select-results.json
```