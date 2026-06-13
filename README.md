# Cine.NET 🍿

Cine.NET is a comprehensive cinema management solution built with modern .NET technologies. This mono-repository contains the backend API, a Blazor WebAssembly frontend, shared libraries, and deployment configurations.

## 🚀 Project Overview

The solution groups backend, frontend, and shared code in one repository to:
- **Enable Shared Data Contracts:** Shared DTOs and logic between API and Frontend.
- **Simplify Development:** Single solution for the entire stack.
- **Keep Architecture Aligned:** Consistent patterns across the project.
- **Support Joint Deployment:** Integrated Docker configurations.

## 🏗 Architecture & Structure

This solution consists of:

- **[API](./API)** – ASP.NET Core 10 Web API using Entity Framework Core 9 and MySQL.
- **[WA](./WA)** – Blazor WebAssembly frontend with MudBlazor components.
- **SharedLibrary](./SharedLibrary)** – Shared models, DTOs, and utility logic used by both API and WA.
- **[UnitTest](./UnitTest)** – Comprehensive test suite for all project layers.

### Tech Stack

| Component | Technology |
| :--- | :--- |
| **Runtime** | .NET 10.0 |
| **Backend** | ASP.NET Core 10, Entity Framework Core 9 |
| **Frontend** | Blazor WebAssembly, MudBlazor 8.15 |
| **Database** | MySQL 8.4 |
| **Infrastructure** | Docker, Docker Compose, Nginx |
| **Testing** | xUnit, Moq |

## 🛠 Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop)
- IDE (Rider, Visual Studio 2022, or VS Code)

### Development Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-repo/Cine.NET.git
   cd Cine.NET
   ```

2. **Configure Environment:**
   - Copy `API/.env.example` to `API/.env` and fill in the required values (DB credentials, API keys).

3. **Run with Docker Compose:**
   ```bash
   docker compose up --build
   ```
   This starts the API, MySQL database, and phpMyAdmin.

4. **Run Frontend (WA):**
   ```bash
   cd WA
   dotnet run
   ```

## 🧪 Testing

The solution includes unit and integration tests located in the `UnitTest` directory.

To run all tests:
```bash
dotnet test
```

## 🚢 Deployment & Environments

### Docker Environments
- `docker-compose.yml`: Standard development environment.
- `docker-compose.acc.yml`: Acceptance/Staging configuration.
- `docker-compose.prod.yml`: Production configuration.

### Publishing Blazor WA
To publish with a specific environment profile:
```bash
dotnet publish WA/WA.csproj -p:PublishProfile=Production
```

## 📖 More Information

Each project contains its own detailed documentation:
- [API Documentation](./API/README.md) - Backend architecture, endpoints, and setup.
- [WebAssembly Documentation](./WA/README.md) - Frontend components and client-side logic.
- [SharedLibrary Documentation](./SharedLibrary/README.md) - Data models and DTOs.

---
*Developed by: Fauve, Richard, Giel, Ivar, Ruben, Yoran, Bart*
