ARG PARENT_VERSION=latest
ARG PARENT_REGISTRY=562955126301.dkr.ecr.eu-west-2.amazonaws.com

# Development
FROM $PARENT_REGISTRY/ffc-dotnet-sdk:$PARENT_VERSION AS development
ARG PARENT_VERSION
ARG PARENT_REGISTRY
LABEL uk.gov.defra.ffc.parent-image=${PARENT_REGISTRY}/ffc-dotnet-sdk:${PARENT_VERSION}
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
FROM $PARENT_REGISTRY/ffc-dotnet-runtime:$PARENT_VERSION AS production
ARG PARENT_VERSION
ARG PARENT_REGISTRY
LABEL uk.gov.defra.ffc.parent-image=${PARENT_REGISTRY}/ffc-dotnet-sdk:${PARENT_VERSION}
COPY --from=development /home/dotnet/out/ ./
ARG PORT=3007
ENV ASPNETCORE_URLS http://*:${PORT}
EXPOSE ${PORT}
# Override entrypoint using shell form so that environment variables are picked up
ENTRYPOINT dotnet FFCDemoPaymentService.dll
