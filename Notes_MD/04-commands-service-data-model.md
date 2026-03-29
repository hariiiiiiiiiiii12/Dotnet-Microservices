# Commands Service Data Model

## Context

The **Commands Service** needs to work with two types of data:

- Commands data
- Platform data (coming from Platform Service)

In this architecture:

```
Platform → Parent Resource
Command → Child Resource
```

Commands belong to a platform.

However, the Commands Service **does not create platforms**. Platforms are created in the **Platform Service**.

---

# Eventual Consistency

To allow the Commands Service to work with platform data, we use the concept of:

```
Eventual Consistency
```

This means:

Platform data created in the **Platform Service** will eventually be **replicated or transmitted** to the Commands Service.

This allows the Commands Service to be aware of platforms without being responsible for creating them.

---

# Handling Multiple Models

The Commands Service needs to handle:

- Platform model
- Command model

Even though the Platform Service owns platform creation, the Commands Service will maintain a **local copy of platform data**.

This allows it to:

- Return platforms it is aware of
- Associate commands with platforms

The existing **Platforms Controller** inside the Commands Service will be used for this.

---

# Important Rule

```
Commands cannot be created without a platform
```

Every command must belong to a valid platform.

---

# Platform Model Relationships

Key identifiers used:

### ExternalId

- Primary key of the platform model in **Platform Service**

### PlatformId

- Foreign key reference inside **Commands Service**
- Links a command to its parent platform

### Navigation Property

Used to define the relationship between:

```
Command → Platform
```

---

# PlatformId Source

The **PlatformId** is not sent inside the request body.

Instead, it is passed through the **URI**.

Example:

```
POST /api/platforms/{platformId}/commands
```

Because of this:

```
PlatformId does not need to be included in the request body
```

---

# Create Command Request

Example request:

```
POST /api/platforms/{platformId}/commands
```

Request body only contains:

- command data

PlatformId comes from the **route parameter**.

---

# Current Problem

At the moment, the Commands Service **does not contain any platform data**.

Because of this:

```
All action results currently return 404
```

Why?

The Commands Service cannot find the platform referenced by the request.

---

# Solution

We must establish communication between the two services so that:

```
Platform Service → Commands Service
```

platform data gets replicated or shared.

This will allow the Commands Service to:

- know about existing platforms
- create commands under those platforms
```