# Kubernetes Pods and Service Communication

## Pods in Kubernetes

A **Pod** is the smallest deployable unit in Kubernetes.

A pod can contain **one or more containers**.

However, in most microservice architectures, the common practice is:

```
One container per pod
```

For this project, each pod will run **a single container**.

---

## Communication Between Services

Kubernetes services enable communication between pods.

One commonly used service type is:

```
ClusterIP
```

### ClusterIP Service

A **ClusterIP service** allows internal communication between services inside the Kubernetes cluster.

Example:

```
Service A → ClusterIP → Service B
```

This is typically used for **container-to-container communication**.

---

## Ingress NGINX

The **Ingress NGINX Controller** is used to expose services externally.

Important point:

- Ingress NGINX **does not use ClusterIP for communication**
- Instead, it communicates **directly with the Kubernetes service**

Flow:

```
External Client
       │
       ▼
Ingress NGINX
       │
       ▼
Kubernetes Service
       │
       ▼
Pod
```

---

## Pod Communication

All pods inside the Kubernetes cluster can communicate with each other using **ClusterIP services**.

Example architecture:

```
Pod 1
   │
Pod 2
   │
Pod 3
   │
Pod 4
```

All four pods can communicate with each other through **ClusterIP services**.

---

## Key Takeaways

- A **pod can contain multiple containers**, but typically runs **one container per pod** in microservices.
- **ClusterIP services** enable internal communication between services.
- **Ingress NGINX** is used for external access to the cluster.
- Pods communicate with each other through **Kubernetes services**.