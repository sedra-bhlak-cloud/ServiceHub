# ServiceHub

ServiceHub is an internal ticketing system built with ASP.NET Core MVC. It allows departments to track, manage, and resolve employee service requests and maintain an internal knowledge base.

---

## Project Structure

This project is structured using Clean Architecture to separate data, business logic, and presentation:

* **`ServiceHub.Domain`**: Contains the core database entities and enums (e.g., ServiceRequest, RequestStatus).
* **`ServiceHub.Infrastructure`**: Handles data access and database migrations using Entity Framework Core and SQLite.
* **`ServiceHub.Web`**: The main ASP.NET Core application containing the MVC views, controllers, and REST API endpoints.
* **`ServiceHub.Tests`**: The automated testing project using xUnit, Moq, and FluentAssertions to verify business logic and API behavior.

---

## Features

* **Role-Based Access Control**: Includes specific access levels and views for Admin, Support Agent, and Employee roles.
* **Data Privacy**: Enforces rules so that standard employees can only view and edit their own submitted service requests.
* **Automated Data Seeding**: Populates the database with initial categories, departments, and an admin user on startup.
* **Test Coverage**: Business services and API controllers are validated using unit and integration tests.

---

## How to Run the Project

### Prerequisites
* .NET 10.0 SDK installed on your machine.

### Setup Steps

1. Clone the repository to your local machine:
```bash
   git clone [https://github.com/sedra-bhlak-cloud/ServiceHub.git](https://github.com/sedra-bhlak-cloud/ServiceHub.git)
   cd ServiceHub