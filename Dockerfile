FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app 
EXPOSE 443

COPY *.sln .
COPY Job.Microservice/*.csproj ./Job.Microservice/
COPY Job.Services.Contracts/*.csproj ./Job.Services.Contracts/
COPY Job.Services.Business/*.csproj ./Job.Services.Business/
COPY Job.Services.Quartz/*.csproj ./Job.Services.Quartz/
COPY Job.Data.Access/*.csproj ./Job.Data.Access/
COPY Job.Data.Contracts/*.csproj ./Job.Data.Contracts/
COPY Job.Data.Object/*.csproj ./Job.Data.Object/

RUN dotnet restore ./Job.Microservice/Job.Microservice.csproj

COPY Job.Microservice/. ./Job.Microservice/
COPY Job.Services.Contracts/. ./Job.Services.Contracts/
COPY Job.Services.Business/. ./Job.Services.Business/
COPY Job.Services.Quartz/. ./Job.Services.Quartz/
COPY Job.Data.Access/. ./Job.Data.Access/
COPY Job.Data.Contracts/. ./Job.Data.Contracts/
COPY Job.Data.Object/. ./Job.Data.Object/

COPY localhost.pfx /certificate/

WORKDIR /app/Job.Microservice
RUN dotnet build ./Job.Microservice.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /app/Job.Microservice
RUN dotnet publish ./Job.Microservice.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app 

COPY --from=publish /certificate/localhost.pfx /app/certificate/
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Job.Microservice.dll"]