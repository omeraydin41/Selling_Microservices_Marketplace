# Technical Decisions

## Why Microservices Architecture?
This project uses a microservices architecture to achieve:
- Independent deployment of services
- Better scalability for marketplace workloads
- Clear separation of business domains

## Why Marketplace Model?
The marketplace model allows:
- Multiple sellers to offer products
- Independent management of sellers and buyers
- Flexible growth of the platform

## Why Asynchronous Messaging (RabbitMQ)?
RabbitMQ is used to:
- Decouple services from each other
- Handle high traffic scenarios
- Improve system resilience

## Why .NET?
.NET was chosen because:
- Strong support for microservices
- High performance and scalability
- Rich ecosystem and tooling
