# Portfolio.Api: A Modern, Scalable RESTful API Solution

## Introduction

Portfolio.Api is a **production-ready RESTful API** built using .NET, Docker, and a suite of modern technologies including Redis, Keycloak, PostgreSQL, Azure Blob Storage and Zoho mail. Designed with robust architectural patterns such as **CQRS** and **Clean Architecture**, this API adheres to modern design principles to ensure high performance, scalability, and maintainability.

Emphasizing both functionality and reliability, Portfolio.Api incorporates advanced API features that include:

- **Content Negotiation & Content Type Versioning:** Allowing clients to request data in preferred formats and seamlessly manage API versioning.
- **HATEOAS:** Enhancing API discoverability by providing hypermedia links in responses.
- **Idempotency:** Ensuring that repeated requests produce the same result, thereby preventing unintended side effects.
- **Rate Limiting:** Protecting the system from abuse while guaranteeing fair usage for all clients.
- **And More**

These features make Portfolio.Api not only robust and secure for production environments but also flexible enough to meet the evolving needs of modern applications.

## How to Run

### Running with Docker

For a quick start in any environment, Portfolio.Api can be executed using Docker. Ensure you have Docker installed, then run the following command from the project root:

```bash
docker compose up -d
```