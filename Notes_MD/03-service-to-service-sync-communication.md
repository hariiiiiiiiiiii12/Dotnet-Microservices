# Service-to-Service Communication (Synchronous)

## Controllers in Commands Service

The **Commands Service** will contain two controllers:

- Commands Controller
- Platforms Controller

The Platforms controller exists inside the Commands Service because it needs to work with **two models**:

- Platform
- Command

Here:

```
Platform → Parent Resource
Command → Child Resource
```

A command always belongs to a platform.

---

# External vs Internal Service Communication

In microservices architecture, communication patterns differ based on the caller.

### External Communication

Requests coming from:

- Web applications
- Mobile applications
- API testing tools (Insomnia / Postman)

are typically **synchronous**.

Example:

```
Client → API Service
```

---

### Internal Service Communication

Service-to-service communication inside a microservices architecture can be:

- Synchronous
- Asynchronous

However, **asynchronous communication is usually preferred**.

Reasons:

- Better scalability
- Loose coupling
- Higher reliability

---

# Synchronous Communication Methods

Two common synchronous communication mechanisms:

```
HTTP
gRPC
```

---

# HTTP Communication Between Services

We will implement a **synchronous HTTP client** in the **Platform Service** that communicates with the **Commands Service**.

The client will send an **HTTP POST request**.

Purpose of this test:

When a **new platform is created** in the Platform Service, we want the Commands Service to be aware of it.

Example flow:

```
Platform Service
      │
      │ HTTP POST
      ▼
Commands Service
```

The payload containing the platform data will be sent to the Commands Service.

---

# Future Improvement (Event-Based Communication)

In a production architecture, this communication should ideally be handled through an **event-driven architecture**.

Example:

```
Platform Created Event
        │
        ▼
Event Bus
        │
        ▼
Commands Service (Subscriber)
```

This is usually implemented using a **Publisher–Subscriber pattern** with a **message bus**.

However, for now we are only verifying that **communication between services works correctly**.

---

# HTTP Client Implementation

We will use:

```
HttpClient
HttpClientFactory
```

### Why Use HttpClientFactory?

When making multiple HTTP requests, using HttpClient directly can cause **connection exhaustion**.

HttpClientFactory helps by:

- Managing connection pooling
- Improving performance
- Preventing socket exhaustion
- Managing lifetime of HttpClient instances

---

# Command Data Client

We define an interface:

```
ICommandDataClient
```

This interface is implemented by:

```
HttpCommandDataClient
```

This implementation will be responsible for sending HTTP requests to the Commands Service.

---

# Is PostAsync Still Synchronous?

Even though we use:

```
PostAsync()
```

this communication is still considered **synchronous service communication**.

Reason:

The calling service **waits for the response before continuing execution**.

---

# Where This Client Is Used

The synchronous HTTP call is triggered when a new platform is created.

Example flow:

```
Create Platform Request
        │
        ▼
Platforms Controller (Platform Service)
        │
        ▼
HTTP POST
        │
        ▼
Commands Service
```

This allows the Commands Service to know that a **new platform has been created**.

---

# What Happens If Commands Service Is Down

If the Commands Service is not running and we attempt to send a request using:

```
PostAsync()
```

we will receive a **connection error**.

This highlights one of the major drawbacks of synchronous communication:

```
Tight coupling between services
```

If the dependent service is unavailable, the request fails.

---

# Deploying Commands Service to Kubernetes

Next step:

Deploy the **Commands Service** to Kubernetes.

Once deployed, we establish communication between the services using:

```
ClusterIP Services
```

---

# Configuration Changes in Platform Service

When running locally, services communicate using **localhost**.

However, in Kubernetes the environment changes.

Since we are now running inside a cluster:

```
Platform Service → Commands Service
```

communication must happen through the **ClusterIP Service**.

Therefore, the Platform Service configuration must point to:

```
commands-clusterip-service
```

This is the service name exposed inside the Kubernetes cluster.

---

# Important Note About Synchronous Communication

One of the problems with synchronous communication is:

```
Services must know the exact endpoint of other services
```

This increases coupling between services and makes deployments more complex.

---

# Next Step

After deploying both services to Kubernetes:

1. Configure ClusterIP services
2. Ensure both services can communicate internally
3. Configure Ingress for external access

---

# Kubernetes Ingress

Ingress is used to expose services outside the cluster.

Important detail:

Ingress is usually deployed inside a **separate namespace**.

Example flow:

```
Client
   │
   ▼
Ingress Controller
   │
   ▼
Kubernetes Service
   │
   ▼
Pod
```

This allows external clients to reach services running inside the cluster.