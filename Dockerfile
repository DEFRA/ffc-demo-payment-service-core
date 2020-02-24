ARG PARENT_VERSION=latest
ARG PARENT_REGISTRY=562955126301.dkr.ecr.eu-west-2.amazonaws.com

# Development
FROM $PARENT_REGISTRY/ffc-dotnet-sdk:$PARENT_VERSION AS development
ARG PARENT_VERSION
ARG PARENT_REGISTRY
ARG PORT=3007
LABEL uk.gov.defra.ffc.parent-image=${PARENT_REGISTRY}/ffc-node-development:${PARENT_VERSION}
RUN mkdir -p /home/dotnet/FFCDemoPaymentService/ /home/dotnet/FFCDemoPaymentService.Tests/
COPY --chown=dotnet:dotnet ./FFCDemoPaymentService.Tests/*.csproj ./FFCDemoPaymentService.Tests/
RUN dotnet restore ./FFCDemoPaymentService.Tests/FFCDemoPaymentService.Tests.csproj
COPY --chown=dotnet:dotnet ./FFCDemoPaymentService/*.csproj ./FFCDemoPaymentService/
RUN dotnet restore ./FFCDemoPaymentService/FFCDemoPaymentService.csproj
COPY --chown=dotnet:dotnet ./FFCDemoPaymentService.Tests/ ./FFCDemoPaymentService.Tests/
COPY --chown=dotnet:dotnet ./FFCDemoPaymentService/ ./FFCDemoPaymentService/
RUN dotnet publish ./FFCDemoPaymentService/ -c Release -o /home/dotnet/out
CMD [ "watch", "--project", "./FFCDemoPaymentService", "run", "--urls", "http://*:${PORT}" ]

# Production
FROM $PARENT_REGISTRY/ffc-dotnet-runtime:$PARENT_VERSION AS production
ARG PARENT_VERSION
ARG PARENT_REGISTRY
ARG PORT=3007
LABEL uk.gov.defra.ffc.parent-image=${PARENT_REGISTRY}/ffc-node-development:${PARENT_VERSION}
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}
COPY --from=development /home/dotnet/out/ ./
CMD [ "FFCDemoPaymentService.dll" ]
