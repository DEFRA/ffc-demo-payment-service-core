# Base
ARG REGISTRY=562955126301.dkr.ecr.eu-west-2.amazonaws.com

FROM $REGISTRY/ffc-dotnet-parent/sdk:$BASE_VERSION AS build
COPY ./FFCDemoPaymentService/*.csproj ./
RUN dotnet restore
COPY ./FFCDemoPaymentService ./
RUN dotnet publish -c Release -o out
EXPOSE 3007

# Development
FROM build AS development
WORKDIR ${WORKING_DIR}
RUN apt-get update \
  && apt-get install -y --no-install-recommends unzip \
  && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg
COPY ./$(WORKING_DIR)/*.csproj ./
RUN dotnet restore
COPY ./$(WORKING_DIR) ./
ENTRYPOINT [ "dotnet", "watch", "run", "--urls", "http://0.0.0.0:3007" ]

# Test
FROM development AS test
WORKDIR ${WORKING_DIR}
COPY ./$(WORKING_DIR)/*.csproj ./
RUN dotnet restore
COPY ./${WORKING_DIR} ./
ENTRYPOINT [ "dotnet", "test" ]

# Production
FROM build AS production
ARG WORKING_DIR
ENV DLL_FILE ${WORKING_DIR}.dll
COPY --from=production /app/out ./
CMD ["${DLL_FILE}"]


# Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=production /app/out ./
RUN chown -R www-data:www-data /app
ENV ASPNETCORE_URLS=http://*:3007
EXPOSE 3007
ENTRYPOINT [ "dotnet", "FFCDemoPaymentService.dll" ]
