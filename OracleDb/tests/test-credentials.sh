#!/usr/bin/env bash
cd "$(dirname "$0")"

# === CONSTANTES ===
DB_USER="app"
DB_PASS="app"
DB_HOST="oracle-db"
DB_PORT="1521"
DB_SERVICE="FREEPDB1"

CONTAINER_NAME="oracle-db"

# === DESCRIPTION TCP COMPLET ===
DSN="(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=${DB_HOST})(PORT=${DB_PORT}))(CONNECT_DATA=(SERVICE_NAME=${DB_SERVICE})))"

echo "Testing connection for ${DB_USER}@${DSN} inside container ${CONTAINER_NAME}..."

# On exécute sqlplus dans le conteneur et on capture stdout+stderr
OUTPUT=$(docker exec -i "${CONTAINER_NAME}" bash -c \
  "echo exit | sqlplus -s \"${DB_USER}/${DB_PASS}@${DSN}\"" 2>&1)
STATUS=$?

if [[ ${STATUS} -eq 0 ]]; then
  echo "✅ Connexion réussie pour ${DB_USER}@${DB_HOST}:${DB_PORT}/${DB_SERVICE}"
  exit 0
else
  echo "❌ Connexion échouée pour ${DB_USER}@${DB_HOST}:${DB_PORT}/${DB_SERVICE}"
  echo "Erreur retournée :"
  echo "${OUTPUT}"
  exit 1
fi
