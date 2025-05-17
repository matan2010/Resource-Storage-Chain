# 📦 ChainResource - Resource Chain System

## ✨ Overview

`ChainResource<T>` is a robust, thread-safe utility designed to provide access to a shared resource of type `T` through a **chain of storages**. Each storage unit in the chain may be either **read-only** or **read-write**, and can have an **expiration policy**. The system ensures optimal access performance and data freshness, while maintaining separation of concerns between multiple data sources.

This design is ideal for static-like resources accessed concurrently from many parts of the system, such as configuration files, cached data, or external service results.

---

## 🧠 Key Concepts

### 🔗 Chain of Responsibility

Each storage is queried in sequence. If data is **missing or expired**, the next storage in the chain is queried. When a value is found, it is **propagated upward** and cached in each **writable** storage along the way.

### ✅ Storage Behavior

* **Read/Write storages** can both cache and retrieve values.
* **Read-only storages** (e.g. Web APIs) serve as the source of truth but cannot be updated.
* Each storage can define its own **expiration interval** to ensure data freshness.

---

## 🏗️ Technologies & Patterns Used

* ✅ **Singleton**: Ensures a single instance of the resource provider exists throughout the application lifecycle.
* ✅ **Interface-driven design**: Allows for flexible implementation and testing of storage layers.
* ✅ **Semaphore**: Used to safely synchronize access across threads and avoid race conditions.
* ✅ **Asynchronous methods (`Task<T>`)**: For optimal performance in I/O-bound operations.

---

## 🔧 Example Implementation: `ExchangeRateList`

An implementation of `ChainResource<ExchangeRateList>` that retrieves currency exchange rates using a 3-layered storage chain:

### 🛠️ Storage Chain (outermost ➡️ innermost):

1. **MemoryStorage**

   * Type: `Read/Write`
   * Expiration: `1 hour`
   * Fastest access point

2. **FileSystemStorage**

   * Type: `Read/Write`
   * Expiration: `4 hours`
   * Stores JSON data on disk

3. **WebServiceStorage**

   * Type: `Read-Only`
   * No expiration needed
   * Fetches live data from a web API

---

## 🔒 Thread Safety

Access to `GetValue()` is guarded with a `SemaphoreSlim` to prevent simultaneous read/write conflicts and to ensure that data is fetched or updated atomically.

---

## 🧼 Design Principles Followed

* **Clean Architecture**: Logic is separated between storage, resource management, and data models.
* **Open/Closed Principle**: New storage types can be added without changing existing code.
* **Failover Support**: System gracefully falls back to deeper layers in case of failure or expiration.

---
