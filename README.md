# HotelBookingAPI
This project has been a fantastic journey into microservices architecture, and it showcases my skills in creating a robust, scalable solution for managing hotel services.
💡 Key Features of the Hotel Management API Project:
📐 Architecture & Design Patterns
✅ Microservices Architecture – Achieves modularity and independence of services, ideal for scalability
✅ Vertical Slice Architecture – Each feature is self-contained, keeping code well-organized and manageable
✅ CQRS (Command Query Responsibility Segregation) – Separates read and write operations for optimized performance
✅ Cache-Aware Pattern – Implements caching strategies to enhance performance
✅ N-Layers Architecture – Ensures separation of concerns for better code maintainability
🛢 Database & ORM
✅ PostgreSQL – The primary relational database, handling complex data operations
✅ Marten – Simplifies document storage in PostgreSQL, enabling efficient handling of JSON data
✅ SQLite – Used for lightweight, local data storage
✅ Entity Framework Core – ORM for database management, mapping between models and tables
💾 Caching
✅ Redis – Integrated with Microsoft.Extensions.Caching.StackExchangeRedis, caching frequently accessed data to improve response times
⚙️ Middleware & Tools
✅ ExceptionHandler – Global exception handling to ensure consistent error management
✅ HTTP Client – Manages efficient API calls for inter-service interactions
🛠 Orchestrator
✅ .NET Aspire – Manages orchestration between microservices for streamlined interactions
🏫 Libraries Used
✅ MediatR – Implements CQRS, handling commands and queries centrally
✅ Scrutor – Enhances dependency injection, making services easy to manage
✅ Carter – Organizes Minimal APIs for clear and efficient endpoint creation
✅ Mapster – Maps data between DTOs and models effortlessly
✅ FluentValidation – Ensures input validation, integrated with MediatR for CQRS
