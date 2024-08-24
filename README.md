# Dapr pub/sub

In this quickstart, you'll create a publisher microservice and a subscriber microservice to demonstrate how Dapr enables a publish-subcribe pattern. The publisher will generate messages of a specific topic, while subscribers will listen for messages of specific topics. See [Why Pub-Sub](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-overview/) to understand when this pattern might be a good choice for your software architecture.

Visit [this](https://docs.dapr.io/developing-applications/building-blocks/pubsub/) link for more information about Dapr and Pub-Sub.

> **Note:** This example leverages the Dapr client SDK. If you are looking for the example using only HTTP [click here](../http).

This quickstart includes one publisher:

- Dotnet client message generator `FormulaAirline`

And one subscriber:

- Dotnet subscriber `ReceiverWebAPI`

## Run all apps with multi-app run template file:

This section shows how to run both applications at once using [multi-app run template files](https://docs.dapr.io/developing-applications/local-development/multi-app-dapr-run/multi-app-overview/) with `dapr run -f .`. This enables to you test the interactions between multiple applications.

1. Open a new terminal window and run the multi app run template:

```bash
dapr run -f .
```

2. Stop and clean up application processes

```bash
dapr stop -f .
```

## Run a single app at a time with Dapr (Optional)

An alternative to running all or multiple applications at once is to run single apps one-at-a-time using multiple `dapr run .. -- dotnet run` commands. This next section covers how to do this.

### Run Dotnet message subscriber with Dapr

1. Run the Dotnet subscriber app with Dapr:

```bash
cd ./ReceiverWebAPI
dapr run --app-id ReceiverWebAPI --resources-path ../Components/ --app-port 7005 -- dotnet run
```

### Run Dotnet message publisher with Dapr

1. Run the Dotnet publisher app with Dapr:

```bash
cd ./FormulaAirline
dapr run --app-id FormulaAirline --resources-path ../Components/ -- dotnet run
```

2. Stop and clean up application processes

```bash
dapr stop --app-id ReceiverWebAPI
dapr stop --app-id FormulaAirline
```
