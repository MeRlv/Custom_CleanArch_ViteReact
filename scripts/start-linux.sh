#!/bin/bash

# Script de démarrage pour Linux
# Ce script démarre l'application ASP.NET Core avec Docker Compose

echo "🚀 Démarrage de l'application sur Linux..."

# Vérifier que Docker est installé et en cours d'exécution
if ! command -v docker &> /dev/null; then
    echo "❌ Docker n'est pas installé. Veuillez installer Docker d'abord."
    exit 1
fi

if ! docker info &> /dev/null; then
    echo "❌ Docker n'est pas en cours d'exécution. Veuillez démarrer Docker d'abord."
    exit 1
fi

# Vérifier que Docker Compose est disponible
if ! docker compose version &> /dev/null; then
    echo "❌ Docker Compose n'est pas disponible. Veuillez installer Docker Compose d'abord."
    exit 1
fi

# Se placer dans le répertoire racine du projet
cd "$(dirname "$0")/.."

# Arrêter les conteneurs existants s'ils existent
echo "🛑 Arrêt des conteneurs existants..."
docker compose down

# Construire et démarrer les services
echo "🔨 Construction et démarrage des services..."
docker compose up --build

echo "✅ Application démarrée !"
echo "🌐 L'application web est accessible sur : http://localhost:8080"
echo "📊 Base de données Oracle accessible sur : localhost:1521"
echo ""
echo "Pour arrêter l'application, utilisez Ctrl+C ou exécutez :"
echo "docker compose down"
