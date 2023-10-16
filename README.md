# FFC Demo Payment Service
Future Farming and Countryside programme payment service.

Digital service mock to claim public money in the event property subsides into mine shaft.  The payment service subscribes to a message queue for new claims and saves a monthly payment schedule in a PostgreSQL database.  It also subscribes to the queue for new calculations and updates the value to pay in the database.

This is a C# .Net Core copy of https://github.com/DEFRA/ffc-demo-payment-service.  The purpose of which is to confirm FFC Platform is capable of supporting .Net Core as well as Node.js.

## Prerequisites

- Access to an instance of an
[Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/)(ASB).
- Docker
- Docker Compose

Optional:
- Kubernetes
- Helm

### Azure Service Bus
This service depends on a valid Azure Service Bus connection string for asynchronous communication.
The following environment variables need to be set
in any non-production environment before the Docker
container is started. When deployed into an appropriately configured AKS
cluster (where configured) the micro-service will use AAD Workload Identity through the manifests.

| Name                             | Description                                                                                |
|----------------------------------|--------------------------------------------------------------------------------------------|
| MESSAGE_QUEUE_HOST               | Azure Service Bus hostname, eg `myservicebus.servicebus.windows.net`                       |
| MESSAGE_QUEUE_USER               | Azure Service Bus SAS policy name, eg `RootManageSharedAccessKey`                          |
| MESSAGE_QUEUE_PASSWORD           | Azure Service Bus SAS policy key                                                           |
| MESSAGE_QUEUE_SUFFIX             | Developer specific queue suffix to prevent collisions, only required for local development |

## Environment variables

The following environment variables are required by the application container. Values for development are set in the Docker Compose configuration. Default values for production-like deployments are set in the Helm chart and may be overridden by build and release pipelines.

| Name                                    | Description                        | Required | Default                        | Valid | Notes                                                                |
| ----                                    | -----------                        | -------- | -------                        | ----- | -----                                                                |
| ApplicationInsights__ConnectionString | App Insights key                   | no       |                                |       | will log to Azure Application Insights if set                        |
| ApplicationInsights__CloudRole          | Role used for filtering metrics    | no       | ffc-demo-payment-service-core  |       | Set to `ffc-demo-payment-service-core-local` in docker compose files |
| Messaging__ScheduleQueueName            | Schedule queue name                | no       | ffc-demo-schedule-             |       |                                                                      |
| Messaging__PaymentQueueName             | Payment queue name                 | no       | ffc-demo-payment-              |       |                                                                      |
| Messaging__MessageQueuePreFetch         | No of messages to pre fetch        | no       |                                |       |                                                                      |
| Postgres__PostgresUser                  | PostgresSQL role                   | yes      | postgres                       |       |                                                                      |
| Postgres__PostgresPassword              | PostgresSQL password               | yes      | ppp                            |       |                                                                      |
| Postgres__PostgresDb                    | PostgresSQL database               | yes      | ffc_demo_payments              |       |                                                                      |
| Postgres__PostgresHost                  | PostgresSQL host name              | yes      | ffc-demo-payment-core-postgres |       |                                                                      |
| Postgres__PostgresPort                  | PostgresSQL port                   | yes      | 5432                           |       |                                                                      |
| Schema__Default                         | Default schema used in PostgresSQL | no       | public                         |       |                                                                      |

## How to run tests
Tests should be run in a container.  Docker compose files are provided to aide with this.

### docker-compose.test.yaml
This file runs all tests and exits the container. If any tests fails the error will be output. Use the docker-compose `-p` flag to avoid conflicting with a running app instance:

`docker-compose -p ffc-demo-payment-service-core-test -f docker-compose.yaml -f docker-compose.test.yaml up`

### docker-compose.test.watch.yaml
This file is intended to be an override file for `docker-compose.test.yaml`.  The container will not exit following test run, instead it will watch for code changes in the application or tests and rerun on occurrence.

`docker-compose -p ffc-demo-payment-service-core-test -f docker-compose.yaml -f docker-compose.test.watch.yaml up`

## Running the application
The application is designed to run in containerised environments, using Docker Compose in development and Kubernetes in production.
- A Helm chart is provided for production deployments to Kubernetes.

The service uses [Liquibase](https://www.liquibase.org/) to manage database migrations. To ensure the appropriate migrations have been run the utility script `scripts/start` may be run to execute the migrations, then the application.

Alternatively the steps can be run manually:
* run migrations
  * `docker-compose -f docker-compose.migrate.yaml run --rm database-up`
* start
  * `docker-compose up`
* stop
  * `docker-compose down` or CTRL-C

### Build container image
Container images are built using Docker Compose, with the same images used to run the service with either Docker Compose or Kubernetes.

By default, the start script will build (or rebuild) images so there will rarely be a need to build images manually. However, this can be achieved through the Docker Compose [build](https://docs.docker.com/compose/reference/build/) command:

```
# Build container images
docker-compose build
```

### Start and stop the service
Use Docker Compose to run service locally.

```
# Start the service in development
docker-compose up
```

### docker-compose.override.yaml

The default `docker-compose.yaml` and `docker-compose.override.yaml` provide the following features to aid local development:

- map port 3007 from the host to the app container
- bind-mount application code into the app container
- run the application behind a file watcher, automatically reloading the app on change
- run a database and message queue alongside the application

Additional Docker Compose files are provided for scenarios such as linking to other running services and running automated tests.

### docker-compose.link.yaml
This will link to other FFC Demo services running locally.

`docker-compose -f docker-compose.yaml -f docker-compose.link.yaml up`

### Deploy to Kubernetes

For production deployments, a helm chart is included in the `.\helm` folder. This service connects to Azure Service Bus, using aadWorkloadIdentity defined in [values.yaml](./helm/ffc-demo-payment-service-core/values.yaml), which must be made available prior to deployment.

#### Accessing the pod
By default, the service is not exposed via an endpoint within Kubernetes.

Access may be granted by forwarding a local port to the deployed pod:

```
# Forward local port to the Kubernetes deployment
kubectl port-forward --namespace=ffc-demo deployment/ffc-demo-payment-service-core 3007:3007
```

Once the port is forwarded, the service can be accessed and tested in the same way as described in the "Test the service" section above.

#### Probes
The service has both an Http readiness probe and an Http liveness probe configured to receive at the below end points.

Readiness: `/healthy`
Liveness: `/healthz`

## Licence
THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

<http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3>

The following attribution statement MUST be cited in your products and applications when using this information.

> Contains public sector information licensed under the Open Government license v3

### About the licence
The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
