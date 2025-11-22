# Architecture Decision Record (ADR) -- Meter Readings Upload & Processing

**ADR-001: CSV-Based Meter Reading Upload Pipeline**

**Status:** Accepted\
**Date:** 2025-11-19

## 1. Problem

The system needs a structured, testable, and maintainable pipeline for
uploading and processing CSV-based meter readings.

## 2. Decision

A CSV-driven processing pipeline was implemented using: - DTO
(`MeterReadingDto`) - Domain entities (`Account`, `MeterReading`) -
Repository abstractions - Structured result types - Multipart file
upload endpoint

## 3. Rationale

-   Clear separation of concerns\
-   Improved testability\
-   Data integrity (duplicate + existence validation)\
-   Extensible for future workflows

## 4. Consequences

### Positive

-   Maintainable architecture\
-   Safe database writes\
-   Clean service boundaries

### Negative

-   Large files may require future batching/streaming

## 5. Alternatives Considered

-   JSON upload (rejected)
-   Direct DbContext usage (rejected)
