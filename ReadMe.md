# OddestOdds

## Overview

`OddestOdds` is a Proof of Concept (POC) constructed to demonstrate a robust architecture pattern within the domain of betting, using a suite of .NET technologies. This solution serves as an exemplar for showcasing technical prowess, best practices, and architectural considerations.

### Projects Structure

- **OddestOdds.Business**: Contains business logic and domain services.
- **OddestOdds.Caching**: Handles caching operations, mainly with Redis.
- **OddestOdds.Common**: Contains shared utilities and common configurations.
- **OddestOdds.Data**: Manages data access and Entity Framework Core operations.
- **OddestOdds.HandlerApp**: The primary API application for handling odds.
- **OddestOdds.Messaging**: Handles message publishing and consumption via RabbitMQ.
- **OddestOdds.PunterApp**: A client application for punters to view and bet on odds.
- **OddestOdds.RealTime**: Manages real-time updates using SignalR.
- **OddestOdds.RabbitMqTopologyCreator**: Utility to set up RabbitMQ topology.
- **OddestOdds.IntegrationTest**: Integration tests using BDD and SpecFlow.
- **OddestOdds.UnitTest**: Unit tests leveraging xUnit and Moq.


## Technical Stack

- **Entity Framework Core (EFCore)**: Utilized for ORM capabilities, enabling smooth database operations and migrations.
- **RabbitMQ**: Implemented as the primary messaging broker, facilitating decoupled and asynchronous communication across microservices.
- **Redis**: Adopted as an in-memory data structure store, primarily used for caching to boost read operations' performance.
- **SignalR**: Employs WebSockets to enable the `PunterApp` to receive real-time updates, ensuring data synchronicity across platforms.

## Execution Guide

1. **Environment Configuration**: Before running the solution, ensure to configure the environment settings in the `appsettings.json` file. Sample configuration is provided below:

```json
{
  "Rabbit": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "Port": 5672
  },
  "Database": {
    "DatabaseName": "oddsDb",
    "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=OddsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Redis": {
    "Host": "localhost:6379,ssl=false,abortConnect=false,defaultDatabase=0",
    "TTL": 120
  },
  "AllowedHosts": "*"
}
```

2. **RabbitMQ Topology**: Use the `OddestOdds.RabbitMqTopologyCreator` to set up the necessary queues and exchanges, either through command-line arguments or manually via RabbitMQ's interface.
3. **Handler Initialization**: Start the `OddestOdds.HandlerApp`. This service acts as the primary message producer, managing and broadcasting various betting event scenarios.
4. **Punter App Activation**: After initializing the handler, run the `OddestOdds.PunterApp`. Upon startup, it will connect to the SignalR hub, awaiting real-time updates. Note: All real-time messages will be reflected directly in the Punter console.
5. **Background Service**: A dedicated service runs asynchronously, consuming messages from the Handler app, ensuring consistent and efficient processing.

## Testing Paradigm

- **Environment Setup for Testing**: Ensure to set up the `appsettings.Test.json` configuration before executing any tests.

- **Unit Testing**: Leveraging `xUnit` and `Moq` for atomic-level testing, ensuring the robustness of isolated components.

- **Integration Testing**: Employing a combination of `SpecFlow` for BDD and automated scenarios.

For a deeper understanding of the foundational assumptions and architectural considerations, please refer to [ASSUMPTIONS.md](ASSUMPTIONS.md).
