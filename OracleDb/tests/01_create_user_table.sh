#!/usr/bin/env bash
set -euo pipefail

# Source common functions
source "$(dirname "$0")/common.sh"

# Initialize log if it doesn't exist
if [[ ! -f "$LOG_FILE" ]]; then
    echo "=== Test CRUD Oracle DB - $(date) ===" > "$LOG_FILE"
fi

log_message "=== ÉTAPE 1: CRÉATION DE LA TABLE $TABLE_NAME ==="

# Get container ID and test connection
get_container_id
test_connection

# Create table with sequence and trigger
CREATE_TABLE_SQL="
CREATE TABLE $TABLE_NAME (
    id NUMBER PRIMARY KEY,
    username VARCHAR2(50) NOT NULL UNIQUE,
    password VARCHAR2(100) NOT NULL,
    created_date DATE DEFAULT SYSDATE,
    updated_date DATE DEFAULT SYSDATE
);

CREATE SEQUENCE ${TABLE_NAME}_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER ${TABLE_NAME}_trigger
BEFORE INSERT ON $TABLE_NAME
FOR EACH ROW
BEGIN
    IF :new.id IS NULL THEN
        :new.id := ${TABLE_NAME}_seq.NEXTVAL;
    END IF;
END;
/"

if execute_sql "$CREATE_TABLE_SQL" "Création de la table $TABLE_NAME"; then
    log_message "✅ Table $TABLE_NAME créée avec succès"
    echo "✅ Table $TABLE_NAME créée avec succès"
else
    log_message "❌ ERREUR: Échec de la création de la table"
    echo "❌ ERREUR: Échec de la création de la table"
    exit 1
fi

echo "Consultez le fichier de log pour plus de détails: $LOG_FILE"
