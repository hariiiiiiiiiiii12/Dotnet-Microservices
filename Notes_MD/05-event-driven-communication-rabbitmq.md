# Event-Driven Communication Using RabbitMQ

## Problem Recap

Previously, the **Commands Service** returned `404` when trying to access platforms.

Reason:

```
Commands Service did not have any platform data
```

Platforms were created in **Platform Service**, but Commands Service had no knowledge of them.

---

# Possible Solution (HTTP Communication)

One approach would be:

```
Platform Service → HTTP POST → Commands Service
```

We had already tested this earlier using an **HTTP client** that sends a POST request from Platform Service to Commands Service.

However, this approach has limitations:

- Tight coupling between services
- Platform Service must know Commands Service endpoint
- Services depend on each other's availability

Because of these drawbacks, **this approach will not be used**.

---

# Event-Driven Architecture

Instead, we use a **Message Bus architecture**.

Concept:

```
Any service can publish messages to the message bus.
Other services can subscribe to those messages.
```

Key benefit:

```
Services only need to know about the message bus,
not about other services.
```

Example flow:

```
Platform Service
        │
        │ Publish Event
        ▼
Message Bus
        │
        ▼
Commands Service (Subscriber)
```

When a platform is created:

- Platform Service publishes an event
- Commands Service listens for the event
- Commands Service updates its local database

---

# RabbitMQ

RabbitMQ will act as the **message broker**.

Responsibilities of a message broker:

- Receive messages from publishers
- Store messages temporarily
- Deliver messages to consumers

RabbitMQ also acts as a **buffer**.

Example:

```
If services are overwhelmed,
messages remain in the queue
until consumers process them.
```

---

# RabbitMQ Concepts

## Exchange

An **Exchange** is responsible for routing messages.

Important characteristics:

- Producers publish messages to exchanges
- Exchanges route messages to queues
- Exchanges do not store messages

---

## Queue

Queues store messages until they are processed by consumers.

---

## Exchange Types

RabbitMQ supports multiple exchange types:

```
Direct
Fanout
Topic
Headers
```

For this project we use:

```
Fanout Exchange
```

---

# Fanout Exchange

Fanout exchanges broadcast messages to **all queues bound to the exchange**.

Characteristics:

- Routing key is ignored
- Every subscriber receives the message

Example:

```
Publisher
   │
   ▼
Fanout Exchange
   │
 ┌─┴─────────┐
 ▼           ▼
Queue A     Queue B
```

---

# RabbitMQ Deployment in Kubernetes

RabbitMQ will be deployed inside Kubernetes.

Components:

```
RabbitMQ Pod
ClusterIP Service
LoadBalancer Service
```

Ports:

- Service communication port
- Web UI port for RabbitMQ dashboard

This setup allows:

- Internal communication inside the cluster
- External access via LoadBalancer

---

# Publisher Implementation (Platform Service)

The Platform Service will publish events to RabbitMQ.

Steps:

1. Install RabbitMQ client packages
2. Configure RabbitMQ connection
3. Create DTO for publishing events
4. Implement Message Bus Client
5. Publish events when a platform is created

---

# Publishing DTO

A separate DTO is created for publishing events.

Example:

```
PlatformPublishedDto
```

Why a separate DTO?

Because the data sent through the message bus may differ from internal models.

---

# Message Bus Client

We create an interface:

```
IMessageBusClient
```

Implementation:

```
MessageBusClient
```

This client is responsible for publishing messages to RabbitMQ.

---

# RabbitMQ Connection

A **ConnectionFactory** is used to create connections.

Example responsibilities:

- Establish connection to RabbitMQ
- Create channels
- Declare exchanges

The application also subscribes to the **connection shutdown event** to handle failures.

---

# Sending Messages

The message is sent using:

```
BasicPublish()
```

Example steps:

1. Convert message string to byte array

```
Encoding.UTF8.GetBytes(message)
```

2. Publish message to exchange

```
_channel.BasicPublish(
    exchange: "trigger",
    routingKey: "",
    basicProperties: null,
    body: body
)
```

Explanation:

- Exchange: `trigger`
- Exchange type: `Fanout`
- Routing key ignored
- Message body sent as byte array

---

# Message Publishing Flow

```
Platform Service
        │
        ▼
RabbitMQ Exchange (Fanout)
        │
        ▼
Queues
        │
        ▼
Consumers
```

The publisher **does not know which services are consuming the message**.

---

# Testing the Publisher

After implementing the publisher:

1. Run Platform Service
2. Create a platform
3. Observe event published to RabbitMQ

Important note:

Commands Service does **not need to be running** for this to work.

Even if synchronous HTTP communication fails:

```
The asynchronous message is still published.
```

---

# Subscriber Implementation (Commands Service)

Next step is implementing the subscriber.

Tasks:

1. Subscribe to message bus
2. Determine event type
3. Process event

---

# Required Components

Commands Service requires:

```
DTOs
RabbitMQ configuration
Event Processor
Event Listener
```

---

# PlatformPublishedDto

This DTO exists in **both services**.

Important rule:

```
DTO structure must be identical in both services
```

The DTO contains the platform data sent through the message bus.

---

# AutoMapper Configuration

Mapping is required between:

```
PlatformPublishedDto → Platform
```

Special case:

```
Id → ExternalId
```

AutoMapper must explicitly map this relationship.

---

# Avoiding Duplicate Platforms

Before inserting a platform into the database:

```
Check if ExternalId already exists
```

This prevents duplicate platform entries.

---

# Event Processor

Responsibilities:

- Receive message
- Determine event type
- Perform appropriate action

Example:

```
If event == PlatformPublished
→ Insert platform into database
```

---

# Service Lifetime Issue

Event Processor must be registered as:

```
Singleton
```

Reason:

It will be injected into the **Message Bus Subscriber**.

However:

```
Repositories are Scoped services
```

Therefore they **cannot be injected directly into the constructor**.

---

# Using IServiceScopeFactory

To resolve scoped dependencies inside singleton services:

```
IServiceScopeFactory
```

This allows the Event Processor to create a scope and access repositories.

---

# Message Bus Subscriber

The subscriber is implemented as a **Background Service**.

Characteristics:

- Starts when the application starts
- Continuously listens for events
- Processes incoming messages

This is ideal for:

```
Long-running message listeners
```

---

# Subscriber Workflow

1. Connect to RabbitMQ
2. Listen for messages
3. Convert message from byte array to string
4. Send message to Event Processor
5. Process event

---

# Local Testing

Run:

```
Platform Service
Commands Service
RabbitMQ
```

Create a platform.

Result:

- Event published
- Commands Service receives event
- Platform stored in Commands database

---

# Kubernetes Deployment

Steps:

1. Push images to DockerHub
2. Redeploy services in Kubernetes
3. Restart deployments

Testing with Insomnia:

- Create platform
- Observe message in RabbitMQ
- Commands Service processes event

---

# Result

Commands Service now receives platform data through events.

Example:

```
Create Platform → Event Published → Commands Service Updates Database
```

However, only **new platforms** are synced through events.

Older platforms are not available yet.

---

# Next Step

To retrieve missing platforms:

```
Use gRPC communication between services
```