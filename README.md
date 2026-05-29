# Cine.NET

Shared .NET solution containing multiple projects.

## Structure

This solution consists of:

- **API** – ASP.NET Core Web API  
- **WA** – Blazor WebAssembly frontend  
- **SharedLibrary** – Shared models and contracts used by both API and WA  

All projects live in a single solution and are versioned together.

## Purpose

The solution groups backend, frontend, and shared code in one repository to:

- enable shared data contracts  
- simplify development  
- keep architecture aligned  
- support joint deployment  

## More Information

Each project contains its own detailed README:

- See the `/API` folder for backend architecture and setup  
- See the `/WA` folder for frontend details  
- See the `/SharedLibrary` folder for shared model information  

### Publishing & Environments (WA)

To publish the Blazor WebAssembly project with a specific environment, use the `PublishProfile` property. This will automatically set the `WasmApplicationEnvironmentName` for the build.

**Command line:**
```bash
dotnet publish WA/WA.csproj -p:PublishProfile=Staging
# OR
dotnet publish WA/WA.csproj -p:WasmApplicationEnvironmentName=Staging
```

Available profiles in `WA.csproj`:
- `Development` (Default)
- `Staging`
- `Production`

Refer to those individual READMEs for configuration, Docker usage, and project-specific documentation.
