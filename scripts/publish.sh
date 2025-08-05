#!/usr/bin/env bash
cd "$(dirname "$0")"
set -e

# Demander à l'utilisateur de choisir un profil
echo "Choisissez un profil de publication :"
echo "1) Docker"
echo "2) Development"
echo "3) Production"
read -p "Entrez le numéro correspondant : " choice

# Déterminer le profil en fonction du choix
case $choice in
  1)
    PROFILE="Docker"
    ;;
  2)
    PROFILE="Development"
    ;;
  3)
    PROFILE="Production"
    ;;
  *)
    echo "Choix invalide. Veuillez réessayer."
    exit 1
    ;;
esac

# Publier avec le profil sélectionné
echo "Publication avec le profil $PROFILE..."
cd ../YourProjectName
dotnet publish src/Web/Web.csproj /p:PublishProfile=$PROFILE