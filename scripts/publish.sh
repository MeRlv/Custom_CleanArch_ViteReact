#!/usr/bin/env bash
set -e

# 1) Publish du projet Web dans ServerIIS/build
cd ../YourProjectName
dotnet publish src/Web/Web.csproj \
  --configuration Debug \
  --output ../ServerIIS/build \
  /p:PublishProfile=Dev