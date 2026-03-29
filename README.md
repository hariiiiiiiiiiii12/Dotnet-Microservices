# .NET Microservices Architecture

This repository contains a microservices-based backend system built using **ASP.NET Core**, following modern distributed system practices such as **service-to-service communication, event-driven architecture, and Kubernetes deployment**.

The project demonstrates how multiple services can communicate using **HTTP, gRPC, and asynchronous messaging with RabbitMQ**, while being deployed and orchestrated using **Docker and Kubernetes**.

---

# Architecture Overview

The system consists of two core microservices:

* **Platform Service**
* **Commands Service**

Each service has its own responsibility and database access layer, following the **Database per Service pattern**.

The services communicate using multiple approaches:

1. **HTTP (Synchronous communication)**
2. **gRPC (Service-to-service data synchronization)**
3. **RabbitMQ (Event-driven asynchronous messaging)**

Architecture flow:

```
Client
   │
   ▼
Platform Service
   │
   ├── HTTP Communication
   ├── gRPC Communication
   └── Event Publishing (RabbitMQ)
           │
           ▼
      Message Bus
           │
           ▼
      Commands Service
```

---

# Services

## Platform Service

The Platform Service is responsible for managing **platform resources**.

Responsibilities:

* Create and manage platforms
* Publish events when a new platform is created
* Provide platform data through **gRPC**

Key features:

* REST API endpoints
* gRPC server implementation
* RabbitMQ event publisher
* Kubernetes deployment

Example API:

```
POST /api/platforms
GET /api/platforms
```

---

## Commands Service

The Commands Service manages **commands associated with platforms**.

Important rule:

```
Commands cannot exist without a Platform.
```

Responsibilities:

* Store commands related to platforms
* Maintain a local copy of platform data
* Subscribe to events from RabbitMQ
* Retrieve missing platforms via gRPC

Communication mechanisms:

* HTTP client (synchronous testing)
* RabbitMQ subscriber (event-driven updates)
* gRPC client (data synchronization)

Example API:

```
GET /api/c/platforms
GET /api/c/platforms/{platformId}/commands
POST /api/c/platforms/{platformId}/commands
```

---

# Communication Patterns

## 1. HTTP Communication

Synchronous communication used mainly for **testing service connectivity**.

Example flow:

```
Platform Service
      │
      │ HTTP POST
      ▼
Commands Service
```

Limitations:

* Tight coupling
* Dependency on service availability

Because of these limitations, HTTP communication is not used for production data synchronization.

---

## 2. Event Driven Messaging (RabbitMQ)

The system primarily relies on **event-driven architecture**.

When a platform is created:

```
Platform Service
      │
      │ Publish Event
      ▼
RabbitMQ Exchange
      │
      ▼
Commands Service (Subscriber)
```

Benefits:

* Loose coupling
* High scalability
* Reliable message delivery
* Services do not need to know about each other

RabbitMQ acts as a **message broker and buffer**, storing messages until consumers process them.

---

## 3. gRPC Communication

gRPC is used to solve the **eventual consistency problem**.

Issue:

Commands Service initially only knows about platforms created **after the event system was introduced**.

Solution:

When the Commands Service starts, it performs a **gRPC request to Platform Service** to retrieve all platforms.

Flow:

```
Commands Service
       │
       │ gRPC Request
       ▼
Platform Service
       │
       ▼
Return all platforms
```

Advantages of gRPC:

* High performance
* HTTP/2 protocol
* Binary serialization using Protocol Buffers
* Strongly typed contracts

---

# Eventual Consistency

The system uses **eventual consistency** instead of strong consistency.

This means:

* Platform Service is the **source of truth**
* Commands Service maintains a **local copy of platform data**
* Data synchronization happens through:

  * Events (RabbitMQ)
  * Startup synchronization (gRPC)

---

# RabbitMQ Concepts Used

The project uses several RabbitMQ components.

### Exchange

An **Exchange** receives messages from producers and routes them to queues.

Exchange types include:

* Direct
* Topic
* Fanout
* Headers

This project uses:

```
Fanout Exchange
```

A fanout exchange **broadcasts messages to all queues** bound to it.

---

### Queue

Queues store messages until they are consumed by services.

```
Publisher → Exchange → Queue → Consumer
```

---

# Kubernetes Deployment

The project includes Kubernetes manifests inside the `K8s` directory.

Components deployed:

* Platform Service
* Commands Service
* RabbitMQ
* Ingress Controller

Key Kubernetes resources used:

* Deployments
* Pods
* ClusterIP Services
* LoadBalancer Services
* Ingress

Example communication inside cluster:

```
Commands Service
      │
      ▼
platforms-clusterip-service
      │
      ▼
Platform Service Pod
```

---

# Repository Structure

```
Dotnet-Microservices
│
├── CommandsService
│   └── ASP.NET Core Commands microservice
│
├── PlatformService
│   └── ASP.NET Core Platform microservice
│
├── K8s
│   └── Kubernetes deployment and service configuration
│
├── Notes_MD
│   └── Detailed learning notes on microservices architecture
│
└── README.md
```

---

# Technologies Used

* ASP.NET Core
* Entity Framework Core
* RabbitMQ
* gRPC
* Docker
* Kubernetes
* AutoMapper

---

# How to Run the Project Locally

1. Start RabbitMQ locally
2. Run Platform Service
3. Run Commands Service
4. Test APIs using Postman or Insomnia

Example platform creation request:

```
POST /api/platforms
```

This will:

* Save the platform
* Publish an event to RabbitMQ
* Commands Service will consume the event

---

# Key Learning Concepts

This project demonstrates several important backend engineering concepts:

* Microservices architecture
* Service-to-service communication
* Event-driven systems
* Message brokers
* Eventual consistency
* Background services in .NET
* Kubernetes deployment
* Distributed system design
