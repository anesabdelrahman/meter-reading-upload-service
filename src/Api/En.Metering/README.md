# En Metering Service

## Overview

This backend service processes CSV-based meter reading uploads. It
validates accounts, prevents duplicates, persists valid readings, and
returns upload results.

## Features

-   CSV meter reading upload\
-   Validation (account existence, duplicates)\
-   EF Core persistence\
-   Repository pattern\
-   Serilog logging\
-   Structured API responses

## Technology Stack

-   ASP.NET Core (.NET 8)\
-   EF Core\
-   SQLite\
-   Serilog\
-   React SPA client

## Project Structure

-   Domain models: `Account`, `MeterReading`
-   DTO: `MeterReadingDto`
-   Request: `UploadMeterReadingsRequest`
-   Response: `MeterReadingProcessResult`, `MeterReadingUploadResponse`
-   Repositories: `IAccountRepository`, `IMeterReadingRepository`
-   DbContext: `MeteringDbContext`

## API Usage

**POST** `/api/meter-readings-upload`\
Content-Type: `multipart/form-data`\
Field: `file`

Response example:

``` json
{
  "success": 120,
  "failed": 12,
  "errors": null
}
```

## Running the Project

1.  Install .NET 8 SDK\
2.  Run the API with `dotnet run`\
3.  Upload CSV via the React client or Postman or included .http file

## Running the Client Project (React)

1.  Navigate to En.Candidate.Project\src\ClientApp
2.  Run the following commands: 
	a. npm install
    b. npm run dev	     
3.  If succeeded to run the web server, then use the following URL: http://localhost:5173/upload
4.  Sample test filea are included in this location: En.Candidate.Project\data

## Enhancements (Future)

-   Row-level error reporting\
-   File schema validation\
-   Performance improvements
-   Polish React client to include viewing of data
