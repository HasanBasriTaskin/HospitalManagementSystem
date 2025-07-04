# 🏥 Hospital Management API

This project is an ASP.NET Core Web API for a simplified Hospital Management Information System (HMIS). It provides backend services for managing patients, doctors, departments, and appointments.

---

## 🏗️ Project Architecture

The project follows the principles of **N-Tier Architecture** to ensure a separation of concerns and a maintainable codebase.

-   **`Entity`**: Contains the core domain models, Data Transfer Objects (DTOs), and entity configurations. It is referenced by all other layers.
-   **`DataAccessLayer`**: Handles all database operations. It abstracts data access using the **Repository** and **Unit of Work** patterns and is implemented with Entity Framework Core. It also includes database seeding logic.
-   **`BusinessLogicLayer`**: Implements the application's business rules and logic. It uses services to orchestrate data from the `DataAccessLayer` and presents it to the `Api` layer. It also contains AutoMapper profiles for object mapping.
-   **`Api`**: The entry point of the application. It exposes the business logic to the outside world through a RESTful API. It's responsible for handling HTTP requests, authentication, and responses.

---

## 🚀 Technologies & Frameworks

-   **Backend**: ASP.NET Core Web API (.NET 8)
-   **ORM**: Entity Framework Core
-   **Database**: SQLite
-   **Authentication**: JWT (JSON Web Token) with Refresh Tokens using ASP.NET Core Identity
-   **API Documentation**: Swagger (OpenAPI)
-   **Logging**: Serilog, configured to write to the console and a Seq container.
-   **Containerization**: Docker & Docker Compose
-   **Mapping**: AutoMapper
-   **Design Patterns**: Repository, Unit of Work, Service Layer

---

## ✨ Features

-   User authentication (Login/Register) using JWT.
-   CRUD operations for Patients, Doctors, and Departments.
-   Appointment management with conflict detection logic.
-   Management of doctor schedules.
-   Querying for available appointment slots for a specific doctor.
-   Centralized exception handling via custom middleware.
-   Automatic database migration and data seeding on startup.

---

## 🗄️ API Endpoints

The API provides several resources, accessible through the following base routes:

-   `/api/auth` - User Authentication
-   `/api/patients` - Patient Management
-   `/api/doctors` - Doctor Management
-   `/api/departments` - Department Management
-   `/api/appointments` - Appointment Scheduling
-   `/api/doctor-schedules` - Doctor Schedule Management

---

## 🐳 How to Run with Docker

This project is configured to run in Docker containers. The `docker-compose.yml` file sets up the API service and a Seq instance for log management.

### Prerequisites
-   Docker
-   Docker Compose

### Steps
1.  Open a terminal in the project's root directory.
2.  Run the following command to build and start the services in the background:
    ```bash
    docker-compose up -d
    ```
3.  You can check the status of the running containers with `docker ps`.

### Access URLs
-   **API (Swagger UI)**: [`http://localhost:8080/swagger`](http://localhost:8080/swagger)
-   **Seq (Log Management)**: [`http://localhost:8081`](http://localhost:8081)

To stop the services, run `docker-compose down` in the same directory.

---
This project was prepared for learning purposes. It does not contain real patient data.
