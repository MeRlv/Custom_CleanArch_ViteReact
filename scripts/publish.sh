#!/usr/bin/env bash
set -e

cd ../YourProjectName
dotnet publish src/Web/Web.csproj /p:PublishProfile=Local