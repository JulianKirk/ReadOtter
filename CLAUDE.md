# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Development Commands

```bash
# Build the shared library (core logic)
dotnet build ReadOtter.Shared/ReadOtter.Shared.csproj

# Build the MAUI desktop app
dotnet build ReadOtter/ReadOtter.csproj

# Build the web version
dotnet build ReadOtter.Web/ReadOtter.Web/ReadOtter.Web.csproj

# Run all tests
dotnet test ReadOtter.Tests/ReadOtter.Tests.csproj

# Run a specific test
dotnet test ReadOtter.Tests/ReadOtter.Tests.csproj --filter "FullyQualifiedName~TestMethodName"

# Format code with CSharpier
dotnet csharpier .
```

## Architecture Overview

This is a multi-project .NET 8 solution for reading EPUB files, supporting both native (MAUI) and web (Blazor) platforms.

### Project Structure

- **ReadOtter/** - MAUI desktop application (entry point for native app)
- **ReadOtter.Shared/** - Core library containing all business logic, services, data models, and shared Blazor components
- **ReadOtter.Web/** - ASP.NET Core server + Blazor WebAssembly client for web deployment
- **ReadOtter.Tests/** - NUnit test suite

### Data Flow Architecture

```
Blazor Components (UI)
        ↓
Business Services (EpubContentService, EpubMetadataService, BookCollectionService)
        ↓
IBookProvider (CachedBookProvider or DirectBookProvider)
        ↓
IVersOneAdaptor (EPUB parsing) + IUnitOfWork/Repository (database)
        ↓
SQLite Database + EPUB Files
```

### Key Patterns

1. **Provider Pattern**: `IBookProvider` abstracts book data access. `CachedBookProvider` maintains in-memory caches for books, metadata, content, and chapters.

2. **Repository + Unit of Work**: Database access via `IBookRepository` and `IUnitOfWork` for transaction management.

3. **Adapter Pattern**: `VersOneAdaptor` wraps the VersOne.Epub library for EPUB parsing.

### Service Registration

All services are registered in `ReadOtter/MauiProgram.cs`. Key services:
- `IBookProvider` → `CachedBookProvider` (with Dictionary-based caching)
- `IVersOneAdaptor` → `VersOneAdaptor`
- `IUnitOfWork` → `UnitOfWork`
- Business services: `EpubContentService`, `EpubMetadataService`, `BookCollectionService`, `InputService`

### Data Models

- **Book** (EF entity): Stores `Id`, `Title`, `FilePath`, `CurrentChapter` in SQLite
- **BookMetaData** (not mapped): Contains `Descriptions`, `Creators`, `Publishers`, `Contributors`
- **BookContent** (not mapped): Contains `Chapters` collection
- **ContentChapter**: Requires either `Title` or `Index` (enforced in constructor)

### Shared Blazor Components

Located in `ReadOtter.Shared/Components/`:
- `ReadView.razor` - Main reading interface with chapter navigation
- `BookCatalogView.razor` - Book library grid display
- `BookGridItemComponent.razor` - Reusable book tile component
- `BookMetadataView.razor` - Book details display

Desktop pages in `ReadOtter/Components/Pages/` delegate to these shared components.

### Database

SQLite database stored at `%LOCALAPPDATA%/ReadOtterLibrary.db`. Managed via EF Core with migrations.

## Preferences

- Do NOT run tests unless explicitly asked to by the user. They prefer running tests manually to save tokens.
