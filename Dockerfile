# Base
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=production

# Development
FROM base AS development
WORKDIR /FFCDemoPaymentService
RUN apt-get update \
  && apt-get install -y --no-install-recommends unzip \
  && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg
COPY ./FFCDemoPaymentService/*.csproj ./
RUN dotnet restore
COPY ./FFCDemoPaymentService ./
#ENTRYPOINT [ "dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000" ]

# Test
FROM development AS test 
WORKDIR /FFCDemoPaymentService.Tests
COPY ./FFCDemoPaymentService.Tests/*.csproj ./
RUN dotnet restore
COPY ./FFCDemoPaymentService.Tests ./
ENTRYPOINT [ "dotnet", "test" ]

# Production
FROM base AS production
COPY ./FFCDemoPaymentService/*.csproj ./
RUN dotnet restore
COPY ./FFCDemoPaymentService ./
RUN dotnet publish -c Release -o out

# Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=production /app/out ./
RUN chown -R www-data:www-data /app
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080
ENTRYPOINT [ "dotnet", "FFCDemoPaymentService.dll" ]
