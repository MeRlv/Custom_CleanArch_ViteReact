# SDK .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Working directory
WORKDIR /workspace

# Install SSH client for Git over SSH
RUN apt-get update && apt-get install -y openssh-client

# Install Clean Architecture Template
RUN dotnet new install Clean.Architecture.Solution.Template::9.0.12

# Install Node.js & npm (LTS)
RUN apt-get update && apt-get install -y curl gnupg \
    && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs

# Verify installations
RUN dotnet --version && \
    node --version && \
    npm --version