# Makefile for BudgetPlanering C# Project

# Default target
.PHONY: all
all: start

# Start the application
.PHONY: start
start:
	dotnet run --project BudgetPlanering.csproj

# Build the application
.PHONY: build
build:
	dotnet build BudgetPlanering.csproj

# Clean the build artifacts
.PHONY: clean
clean:
	dotnet clean BudgetPlanering.csproj