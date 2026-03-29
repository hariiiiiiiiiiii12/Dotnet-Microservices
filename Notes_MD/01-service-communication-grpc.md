# Service Communication using gRPC

In this architecture, the **Commands Service** communicates with the **Platform Service** using **gRPC**.

The Commands Service sends **gRPC requests** to a gRPC server hosted inside the Platform Service.

---

## Architecture Flow

```
Commands Service
       │
       │ gRPC Request
       ▼
Platform Service (gRPC Server)
```

---

## Explanation

- The **Commands Service** acts as the **gRPC client**.
- The **Platform Service** exposes a **gRPC endpoint**.
- Communication happens using **Protocol Buffers (Protobuf)**.
- This allows **fast, efficient service-to-service communication**.

---

## Why Use gRPC?

gRPC is commonly used in microservices because:

- It is **faster than REST** due to binary serialization.
- It uses **HTTP/2**, which supports multiplexing.
- It provides **strongly typed contracts** through `.proto` files.
- It is ideal for **internal service communication**.

---

## Typical Flow in .NET Microservices

1. Platform Service exposes a **gRPC server**.
2. A `.proto` contract defines the service and messages.
3. Commands Service generates a **gRPC client** from the `.proto` file.
4. Commands Service calls the Platform Service through gRPC.

---

## Example High-Level Flow

```
Client Request
     │
     ▼
Commands Service
     │
     │ gRPC call
     ▼
Platform Service
     │
     ▼
Database / Business Logic
```

---

## Key Takeaway

In this setup:

- **Commands Service → gRPC Client**
- **Platform Service → gRPC Server**

The Commands Service uses gRPC to communicate efficiently with the Platform Service in a microservices environment.