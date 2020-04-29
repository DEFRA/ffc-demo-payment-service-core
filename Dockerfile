ARG PARENT_VERSION=1.0.0-dotnet3.1

# Development
FROM defradigital/dotnetcore-development:${PARENT_VERSION} AS development
ARG PARENT_VERSION
LABEL uk.gov.defra.ffc.parent-image=defradigital/dotnetcore-development:${PARENT_VERSION}

RUN mkdir -p /home/dotnet/FFCDemoPaymentService/ /home/dotnet/FFCDemoPaymentService.Tests/
COPY --chown=dotnet:dotnet ./FFCDemoPaymentService.Tests/*.csproj ./FFCDemoPaymentService.Tests/
RUN dotnet restore ./FFCDemoPaymentService.Tests/FFCDemoPaymentService.Tests.csproj
COPY --chown=dotnet:dotnet ./FFCDemoPaymentService/*.csproj ./FFCDemoPaymentService/
RUN dotnet restore ./FFCDemoPaymentService/FFCDemoPaymentService.csproj
COPY --chown=dotnet:dotnet ./FFCDemoPaymentService.Tests/ ./FFCDemoPaymentService.Tests/
COPY --chown=dotnet:dotnet ./FFCDemoPaymentService/ ./FFCDemoPaymentService/
RUN dotnet publish ./FFCDemoPaymentService/ -c Release -o /home/dotnet/out

ARG PORT=3007
ENV PORT ${PORT}
EXPOSE ${PORT}
# Override entrypoint using shell form so that environment variables are picked up
ENTRYPOINT dotnet watch --project ./FFCDemoPaymentService run --urls "http://*:${PORT}"

# Production
FROM defradigital/dotnetcore:${PARENT_VERSION} AS production
ARG PARENT_VERSION
LABEL uk.gov.defra.ffc.parent-image=defradigital/dotnetcore:${PARENT_VERSION}
COPY --from=development /home/dotnet/out/ ./
ARG PORT=3007
ENV ASPNETCORE_URLS http://*:${PORT}
EXPOSE ${PORT}
# Override entrypoint using shell form so that environment variables are picked up
ENTRYPOINT dotnet FFCDemoPaymentService.dll
