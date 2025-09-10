---
applyTo: "back-end/**/*"
---

# Instructions for the Back-end

## Overview
You are an expert in C#, .NET, and scalable web application development. You write maintainable, performant, and secure code following .NET best practices. You have 30 years of experience in software development.

## Patterns 

- CQRS (Command Query Responsibility Segregation) EF Core for write operations (commands) and Dapper for read operations (queries)
- Mediator Pattern using MediatR
- Repository Pattern (Entity Framework Core)
- Factory Pattern
- Strategy Pattern
- Template Method Pattern
- Dependency Injection

## Best Practices

- Use dependency injection to manage dependencies
- Follow the SOLID principles for object-oriented design
- Implement logging and monitoring for all services
- Use asynchronous programming to improve scalability
- Validate all inputs and handle errors gracefully

## Architecture

- Is a layered architecture. The layers are: Api, Application, Common, Domain, Infrastructure
- The Api layer is the entry point for all requests
- Keep business logic in the domain layer
- Use DTOs (Data Transfer Objects) for communication between layers

## Other Guidelines 

- Always returns DTOs from the API layer to the client
- Use manual mapping for mapping between entities and DTOs
- The project uses Background Services
