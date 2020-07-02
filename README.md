# MS Graph Advanced Queries and JSON batching demo

## Highlights

- Use of [consistencyLevel: eventual][0] request header to enable advanced queries.
- Use of [$search][1] query parameter to search applications by `displayName`.
- Use of [$count][1] query parameter to retrieve total number of applications
- Use of [JSON batching][2] to retrieve all application `owners` and `service principals` in a single call.


## How to build

- From Visual Studio:<br/>
Press `CTRL+F8`

- From command line:<br/>
Run `dotnet build` from solution folder<br/>
Run `nppm install` from *ClientApp* folder


## How to run

- From Visual Studio:<br/>
Press `CTRL+F5`

- From command line:<br/>
Run `dotnet run` from solution folder

[0]: https://developer.microsoft.com/en-us/identity/blogs/build-advanced-queries-with-count-filter-search-and-orderby/
[1]: https://docs.microsoft.com/en-us/graph/query-parameters?#search-parameter
[2]: https://docs.microsoft.com/en-us/graph/json-batching

## How to configure client credentials

Edit file *launchSettings.json* and add values for AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_CLIENT_SECRET under `profiles\{profile}\environmentVariables`:

```json
{
  "profiles": {
    "{profile}": {
      "environmentVariables": {
        "AZURE_TENANT_ID": "string",
        "AZURE_CLIENT_ID": "string",
        "AZURE_CLIENT_SECRET": "string"
      }
    }
  }
}
```
## Copyright
Copyright (c) 2020 Microsoft. All rights reserved.
