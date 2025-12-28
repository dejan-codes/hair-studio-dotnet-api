# HairStudio - .NET 6 Web API Application

#### **Overview**

This repository contains the backend for the **HairStudio** application,
built using **.NET 6 Web API**.\
It provides APIs for authentication, appointment booking, product
management, order handling, and more.\
The system supports three user roles: **Administrator**, **Employee**,
and **User**.

------------------------------------------------------------------------

#### **Project Structure**

The solution follows a modular architecture with separate layers for
API, services, repositories, and models:

------------------------------------------------------------------------

### **HairStudio.API**

Main application layer.

-   `Controllers/` --- Contains Web API controllers defining HTTP
    endpoints.
-   `PostmanCollections/` --- Postman collections for endpoint testing.
-   `appsettings.json` --- Application configuration (database, email,
    JWT).
-   `Program.cs` --- Application startup and middleware pipeline
    configuration.

------------------------------------------------------------------------

### **HairStudio.Model**

Domain and shared layer.

-   `DatabaseCreationScripts/` --- SQL scripts required for DB setup.
-   `Enums/` --- Shared enumerations.
-   `Models/` --- Entity classes mapped to the database.

------------------------------------------------------------------------

### **HairStudio.Repository**

Persistence layer implemented using Entity Framework Core.

-   `Extensions/` --- Helper extension logic related to data access.
-   `Implementations/` --- Repository logic implementations.
-   `Interfaces/` --- Data access contract definitions.

------------------------------------------------------------------------

### **HairStudio.Services**

Business logic layer.

-   `Audit/` --- System auditing and activity tracking.
-   `Common/` --- Shared utilities, constants, helpers.
-   `DTOs/` --- Data Transfer Objects used between layers.
-   `Errors/` --- Centralized error definitions for consistent
    messaging.
-   `GlobalExceptionHandler/` --- Central exception handling
    configuration.
-   `Infrastructure/` --- Provides access to current authenticated user
    context.
-   `Implementations/` --- Service implementation classes.
-   `Interfaces/` --- Business service contracts defining core logic.

------------------------------------------------------------------------

#### **Database Setup**

To initialize the database, execute the scripts located in:

    HairStudio.Model/DatabaseCreationScripts/

These scripts generate required tables.

------------------------------------------------------------------------

#### **Postman Collections**

To test the API, import the predefined Postman collection found in:

    HairStudio.API/PostmanCollections/

------------------------------------------------------------------------

#### **Key Features**

-   **Authentication**
    -   User registration and login.
    -   Email verification with confirmation code.
    -   Password reset functionality via email.
-   **Appointment Booking**
    -   Users can book appointments based on employee work schedules.
-   **Web Shop**
    -   Product browsing and purchase support.
-   **Admin Panel**
    -   Manage services, product categories, employees, product brands,
        and system users.
-   **Working Hours**
    -   Administrators define employee schedules; bookings are validated
        against available slots.
-   **Order Management**
    -   Employees can update order statuses and access related
        information.
-   **Activity Tracking**
    -   Admins can view system change history via an audit log.

------------------------------------------------------------------------

#### **User Roles**

-   **Administrator**
    -   Full system access.
    -   Manages employees, products, services, brands, users, and
        working schedules.
    -   Views activity logs.
-   **Employee**
    -   Monitors appointments and manages order statuses.
-   **User**
    -   Registers, logs in, books appointments, and purchases products.

------------------------------------------------------------------------

#### **Run Locally**

1.  **Clone the repository**

    ``` bash
    git clone https://github.com/dejan-codes/hair-studio-dotnet-api.git
    ```

2.  **Install Requirements**

    -   .NET 6 SDK
    -   SQL Server instance

3.  **Database Setup**

    -   Execute scripts from:

            HairStudio.Model/DatabaseCreationScripts/

4.  **Configure the Application**

    -   Set your database connection string in `appsettings.json`

5.  **Run the API**

    ``` bash
    dotnet run --project HairStudio.API
    ```

    Once started, the API will be available on the configured host.

------------------------------------------------------------------------
