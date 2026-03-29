# gRPC Communication Between Services

## Problem

After implementing the **event-driven architecture**, the Commands Service only knows about platforms that were created **after the event system was introduced**.

Example:

```
Create Platform → Event Published → Commands Service receives it
```

However, platforms that already existed in the **Platform Service** are still unknown to the **Commands Service**.

So the Commands Service does not have a **complete list of platforms**.

---

# Solution

To solve this problem we use **gRPC communication**.

The idea is:

When the **Commands Service starts**, it will call the **Platform Service** using gRPC and fetch all existing platforms.

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

This allows the Commands Service to **synchronize missing platform data**.

---

# Service Roles

In this setup:

```
Commands Service → gRPC Client
Platform Service → gRPC Server
```

The Commands Service pulls all platforms from the Platform Service when it starts.

---

# Why gRPC

gRPC is commonly used for **internal service-to-service communication** because:

- It is faster than REST
- Uses **HTTP/2**
- Uses **Protocol Buffers (Protobuf)**
- Strongly typed contracts

---

# HTTPS Requirement in gRPC

Normally, gRPC communication runs over:

```
HTTPS
```

However, inside our **Kubernetes cluster**, HTTPS is not configured.

Because of this we must configure gRPC to run using:

```
HTTP/2 without TLS
```

---

# Required Configuration

Since TLS is not used inside the cluster, we must explicitly configure the server to accept:

```
HTTP/2 over plain HTTP
```

Otherwise the gRPC client will attempt TLS negotiation and the connection will fail.

---

# Kestrel Configuration

The configuration must be applied to the **Kestrel Web Server** in the **Platform Service**.

Important notes:

- This configuration is applied only on the **server**.
- Commands Service (client) does not need this configuration.

---

# gRPC Port

The Kubernetes **ClusterIP service** must expose a dedicated port for gRPC communication.

Example:

```
Port: 666
```

Flow:

```
Commands Service
        │
        │ gRPC call
        ▼
platforms-clusterip-service:666
        │
        ▼
Platform Service
```

---

# Deployment Changes

Since we added a new port for gRPC, the **deployment file must be updated**.

Because the deployment configuration changed, we must run:

```
kubectl apply
```

instead of redeploying.

---

# Environment Configuration

This configuration is required only when running inside **Kubernetes (production environment)**.

In the **local development environment**, HTTP is already supported and additional configuration is not required.

---

# Summary

The final architecture now uses **two communication models**:

### Event Driven (RabbitMQ)

```
Platform Service → Event → Commands Service
```

Used for:

- New platform creation events

---

### gRPC (Synchronous Pull)

```
Commands Service → gRPC → Platform Service
```

Used for:

- Retrieving all existing platforms
- Ensuring data consistency between services

---

# Final Outcome

Commands Service now receives platform data in two ways:

1. **Event Driven Updates**

```
Platform created → Event → Commands Service
```

2. **Startup Synchronization**

```
Commands Service start → gRPC call → Fetch all platforms
```

This ensures the Commands Service always has a **complete and consistent list of platforms**.