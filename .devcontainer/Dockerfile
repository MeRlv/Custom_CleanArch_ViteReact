# .devcontainer/Dockerfile adapted for EF Core CLI

# Use .NET 9 SDK base image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base

# Set working directory to VS Code workspace
WORKDIR /workspace

# Ensure dotnet global tools path is in PATH and define DOTNET_ROOT
ENV DOTNET_ROOT=/usr/share/dotnet \
    PATH="$PATH:/root/.dotnet/tools"

# Install SSH client for Git over SSH
RUN apt-get update \
 && apt-get install -y --no-install-recommends openssh-client \
 && rm -rf /var/lib/apt/lists/*

# Install Clean Architecture Template
RUN dotnet new install Clean.Architecture.Solution.Template::9.0.12

# Install EF Core CLI (dotnet-ef)
RUN dotnet tool install --global dotnet-ef --version 9.0.0
RUN dotnet tool install --global NSwag.ConsoleCore --version 14.2.0

# Install Node.js & npm (LTS)
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl gnupg \
 && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
 && apt-get install -y --no-install-recommends nodejs \
 && rm -rf /var/lib/apt/lists/*

# Verify installations
RUN dotnet --version \
 && dotnet ef --version \
 && nswag version \
 && node --version \
 && npm --version


EXPOSE 5000 3000