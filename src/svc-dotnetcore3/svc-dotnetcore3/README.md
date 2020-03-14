# Server

## View full API version with Swagger UI
Go to `http(s)://<domain>/swagger` to view the API's

## Restore/Install packages for all of API/Tests/Database
```[cmd]
dotnet restore src\svc-dotnetcore3
```

## Run Development Mode (Bypass all authorisations)
```[cmd]
dotnet watch --project=src\svc-dotnetcore3\svc-dotnetcore3 run --launch-profile=dev
```

## Run Staging Mode - Testing right before Production (Authorisation should work here)
```[cmd]
dotnet watch --project=src\svc-dotnetcore3\svc-dotnetcore3 run --launch-profile=stage
```

## Build Mode (only works on Tests and API)
```[cmd]
dotnet build src\svc-dotnetcore3\svc-dotnetcore3
dotnet build src\svc-dotnetcore3\Tests
```

## Deployment Testing
### Add Info to appsettings.json!!!
### Publish Mode (Build for Client as well here to deploy)
```[cmd]
dotnet publish -c release src\svc-dotnetcore3\svc-dotnetcore3
```
### Go to \src\svc-dotnetcore3\svc-dotnetcore3\bin\release\netcoreapp3.1\publish
```[cmd]
dotnet Web.API.dll
```
