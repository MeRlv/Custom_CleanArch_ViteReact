# Script to build NSwag API client
cd "$(dirname "$0")"
set -e

PROJECT_PATH="../YourProjectName/src/Web"

echo "Building the NSwag API client..."
cd "$PROJECT_PATH"
nswag run config.nswag