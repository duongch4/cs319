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

## Testing With coverlet.msbuild + ReportGenerator
```[cmd]
dotnet test src\svc-dotnetcore3\Tests\Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="./TestResults/Result.opencover.xml" /p:Threshold=100

dotnet C:\\Users\bangc\.nuget\packages\reportgenerator\4.5.0\tools\netcoreapp3.0\ReportGenerator.dll "-reports:src\svc-dotnetcore3\Tests\TestResults\Result.opencover.xml" "-targetdir:src\svc-dotnetcore3\Tests\TestResults\CoverageReport"
```

## Testing With Native Microsoft Runsettings
```[cmd]
dotnet test src\svc-dotnetcore3\Tests\Tests.csproj --settings:src\svc-dotnetcore3\Tests\test.runsettings.xml
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
