#!/usr/bin/env bash
cd "$(dirname "$0")"
set -euo pipefail

COMPOSE_PROJECT="cleanarch9_reactvitejs_template"
SERVICE_NAME="oracle-db"
DB_USER="app"
DB_PASS="app"
DB_HOST="oracle-db"
DB_PORT="1521"
DB_SERVICE="FREEPDB1"

# === DESCRIPTION TCP COMPLET ===
DSN="(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=${DB_HOST})(PORT=${DB_PORT}))(CONNECT_DATA=(SERVICE_NAME=${DB_SERVICE})))"

# Create output directory if it doesn't exist
mkdir -p ./output

# Services to ignore (internes, health checks…)
IGNORE_SERVICES=(PLSExtProc XDB)

# Verifying environment
echo "Recherche du conteneur '$SERVICE_NAME' dans le projet '$COMPOSE_PROJECT'..."
CONTAINER_ID=$(docker compose -p "$COMPOSE_PROJECT" ps -q "$SERVICE_NAME")
if [[ -z "$CONTAINER_ID" ]]; then
  echo "Conteneur non trouvé."
  docker compose -p "$COMPOSE_PROJECT" ps
  exit 1
fi
echo "  → ID conteneur : $CONTAINER_ID"

# Test de connexion avant d'extraire le schéma
echo "Test de connexion à la base de données..."
OUTPUT=$(docker exec -i "$CONTAINER_ID" bash -c \
  "echo exit | sqlplus -s '${DB_USER}/${DB_PASS}@${DSN}'" 2>&1)
STATUS=$?

if [[ ${STATUS} -ne 0 ]]; then
  echo "❌ Connexion échouée pour ${DB_USER}@${DB_HOST}:${DB_PORT}/${DB_SERVICE}"
  echo "Erreur retournée :"
  echo "${OUTPUT}"
  exit 1
fi
echo "✅ Connexion réussie"


OUT_CTN="/tmp/schema_${DB_USER}.sql"
OUT_HOST="./output/schema_${DB_USER}_$(date +%Y%m%d_%H%M%S).sql"

# Get the database schema DDL (Oracle DB)
echo "Génération du DDL pour le schéma '$DB_USER'…"
docker exec --user oracle "$CONTAINER_ID" bash -lc "
  sqlplus -s '${DB_USER}/${DB_PASS}@${DSN}' <<-EOF
    SET PAGESIZE 0 FEEDBACK OFF VERIFY OFF HEADING OFF ECHO OFF
    SET LONG 1000000 LINESIZE 200
    SPOOL ${OUT_CTN}
    SELECT DBMS_METADATA.GET_DDL('TABLE', table_name, owner) || ';'
      FROM all_tables
     WHERE owner = UPPER('${DB_USER}');
    SPOOL OFF
    EXIT
EOF
"

echo "Rapatriement du fichier…"
docker cp "${CONTAINER_ID}:${OUT_CTN}" "./${OUT_HOST}"

echo "Schéma exporté : ./${OUT_HOST}"
