# Base
ARG REGISTRY=562955126301.dkr.ecr.eu-west-2.amazonaws.com

# FROM $REGISTRY/ffc-dotnet-parent/sdk:$BASE_VERSION AS 
FROM dotnet-sdk-parent as build
RUN mkdir -p /app/FFCDemoPaymentService/
WORKDIR /app/FFCDemoPaymentService/
COPY --chown=www-data:www-data ./FFCDemoPaymentService/*.csproj ./
RUN ls -la
RUN dotnet restore
COPY --chown=www-data:www-data ./FFCDemoPaymentService/ ./
RUN dotnet publish -c Release -o /app/out

# Development
FROM build AS development
USER root
RUN apk update \
  && apk add unzip \
  && apk --no-cache add curl procps \
  && wget -qO- https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg
USER www-data
CMD ["--urls", "http://*:3007"]

# Test
FROM build AS test
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

