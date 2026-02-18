# AGENTS Guide

This file provides quick guidance for LLM/code agents working in this repository.

## Repository Summary

- .NET 10 Clean Architecture template.
- Main projects:
  - `src/BoricuaCoder.CleanTemplate.Api`
  - `src/BoricuaCoder.CleanTemplate.Application`
  - `src/BoricuaCoder.CleanTemplate.Domain`
  - `src/BoricuaCoder.CleanTemplate.Infrastructure`
  - `tests/BoricuaCoder.CleanTemplate.UnitTests`
- Dependency flow: `Api -> Application -> Domain <- Infrastructure`.

## Local Validation Commands

- Build: `dotnet build`
- Test: `dotnet test`
- Run API: `dotnet run --project src/BoricuaCoder.CleanTemplate.Api`

## Implementation Notes

- Keep changes minimal and consistent with the existing structure.
- Follow the current "without dogma" approach in `README.md`:
  - no MediatR,
  - explicit handlers resolved via DI,
  - Dapper + PostgreSQL for persistence.
- For new features, mirror the layer-by-layer pattern documented in `README.md` under **How to Add a New Feature**.
