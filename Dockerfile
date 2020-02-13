# Base
ARG REGISTRY=562955126301.dkr.ecr.eu-west-2.amazonaws.com

# Development
# FROM $REGISTRY/ffc-dotnet-parent/sdk:$BASE_VERSION AS 
FROM dotnet-sdk-parent AS development
RUN mkdir -p /app/FFCDemoPaymentService/
WORKDIR /app/FFCDemoPaymentService/
COPY --chown=www-data:www-data ./FFCDemoPaymentService/*.csproj ./
RUN dotnet restore
COPY --chown=www-data:www-data ./FFCDemoPaymentService/ ./
RUN dotnet publish -c Release -o /app/out
CMD ["--urls", "http://*:3007"]

# Test
FROM developent AS test
RUN mkdir -p /app/FFCDemoPaymentService.Tests/
WORKDIR /app/FFCDemoPaymentService.Tests/
COPY --chown=www-data:www-data ./FFCDemoPaymentService.Tests/*.csproj ./
RUN dotnet restore
COPY --chown=www-data:www-data ./FFCDemoPaymentService.Tests/ ./
ENTRYPOINT [ "dotnet", "test" ]

# Production
FROM dotnet-runtime-parent AS production
WORKDIR /app/
ENV ASPNETCORE_URLS=http://*:3007
EXPOSE 3007
COPY --from=build /app/out/ ./
CMD ["FFCDemoPaymentService.dll"]

