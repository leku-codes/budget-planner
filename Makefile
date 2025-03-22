# Makefile for BudgetPlanning C# Project

# Default target
.PHONY: all
all: start

# Start the application
.PHONY: start
start:
	dotnet run --project BudgetPlanning.csproj

# Build the application
.PHONY: build
build:
	dotnet build BudgetPlanning.csproj

# Clean the build artifacts
.PHONY: clean
clean:
	dotnet clean BudgetPlanning.csproj