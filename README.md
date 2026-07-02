# FreakyFashion - Cloud-Ready .NET 10 API (Clean Architecture)

Welcome to **FreakyFashion**, a production-ready, highly scalable e-commerce backend built with **.NET 10**. This project demonstrates enterprise-level system architecture, robust microservices design patterns, and automated CI/CD configurations for containerized cloud deployment.

## Architectural Overview & Highlights
This backend is built entirely from scratch with a focus on modern software engineering standards. To guarantee maximum code separation, high testability, and maintainability, the solution strictly follows **Clean Architecture** patterns split into isolated layers:
*   **DomainLayer**: Core enterprise entities (`Products`, `Categories`, `Orders`) and EF Core Database Migrations.
*   **ApplicationLayer**: Decoupled use-cases, DTO contracts, and core repository abstractions.
*   **InfrastructureLayer**: Concrete data-access logic, repository implementations, and storage orchestrations.
*   **Presentation (FreakyFashion)**: A decoupled REST API documented via Swagger.

### Key Technical Implementations
*   **Custom Thread-Safe Object Mapping**: Instead of using third-party libraries, I designed an optimized `MapperUnitOfWork` utilizing a `ConcurrentDictionary` cache. This ensures safe multi-threaded data mapping and blazing fast lookup speeds.
*   **Secure Cloud Storage & Streaming**: Image uploads are handled via the Azure Blob Storage SDK. To comply with modern security standards, the system generates time-restricted **SAS (Shared Access Signature) Tokens** allowing secure, direct image streaming to frontend applications.
*   **Production Telemetry**: Integrated with **Azure Application Insights** for proactive system monitoring, log tracking, and live diagnostics.

## Containerization & CI/CD Pipeline
The repository comes fully equipped with a production-grade infrastructure setup, making it 100% cloud-ready.

*   **Multi-Stage Dockerfile**: Engineered to minimize build times and security vulnerabilities. It leverages heavy SDK layers for caching NuGet restores and compilation, but ships on a stripped-down, secure non-root `aspnet:10.0` environment.
*   **Azure DevOps Pipeline (`azure-pipelines.yml`)**: A automated CI/CD pipeline built for Azure Container Registry (ACR). It securely parameterizes variables, automates immutable container tag versions via `Build.BuildId`, and handles atomic build-and-push steps.

---

## Local Development (100% Cost-Effective)
To eliminate development costs and guarantee environmental consistency, the application runs entirely locally using containerized emulators:
1.  **Database**: Managed via Local SQL Server via Entity Framework Migrations.
2.  **Cloud Storage Emulator**: Fully integrated with **Azurite** for local Blob Storage emulation via a persistent USB environment (`UseDevelopmentStorage=true`).

---

## Project Roadmap (In Progress)
To maintain a transparent MVP (Minimum Viable Product) workflow, the following segments are currently under active development:
*   [ ] **Automated Test Coverage**: Implementation of isolated In-Memory database unit testing for core API controllers.
*   [ ] **Frontend Consumer Application**: Completing a companion .NET MVC Client application designed to asynchronously consume this API and display dynamic views.
*   [ ] **Distributed Session Management**: Moving local HttpContext sessions into an **Azure Cache for Redis** cluster to allow horizontal scaling in full Canary/Blue-Green deployments.
