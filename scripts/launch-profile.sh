# Script to launch the project locally with a specified launch profile
cd "$(dirname "$0")"
set -e

# Path to the Web project
PROJECT_PATH="../YourProjectName/src/Web/Web.csproj"
PROFILE="YourProjectName.Web"

echo "Starting the project with the $PROFILE launch profile..."
dotnet run --project "$PROJECT_PATH" --launch-profile "$PROFILE"
