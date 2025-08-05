#!/usr/bin/env bash
set -euo pipefail

# Source common functions
source "$(dirname "$0")/common.sh"

log_message "=== ÉTAPE 3: READ - LECTURE DES DONNÉES ==="

# Get container ID and test connection
get_container_id
test_connection

# Read all data
SELECT_SQL="
SELECT id, username, password, 
       TO_CHAR(created_date, 'DD/MM/YYYY HH24:MI:SS') AS created_date,
       TO_CHAR(updated_date, 'DD/MM/YYYY HH24:MI:SS') AS updated_date
FROM $TABLE_NAME 
ORDER BY id;
"

if execute_sql "$SELECT_SQL" "Lecture de toutes les données (READ)"; then
    log_message "✅ Lecture des données réussie"
    echo "✅ Lecture des données réussie"
    
    # Additional queries for testing different READ operations
    ADDITIONAL_READS="
    -- Compter le nombre total d'enregistrements
    SELECT COUNT(*) AS total_users FROM $TABLE_NAME;
    
    -- Rechercher un utilisateur spécifique
    SELECT id, username FROM $TABLE_NAME WHERE username = 'admin';
    
    -- Lister les utilisateurs par ordre alphabétique
    SELECT username, TO_CHAR(created_date, 'DD/MM/YYYY') AS date_creation 
    FROM $TABLE_NAME 
    ORDER BY username;
    "
    
    if execute_sql "$ADDITIONAL_READS" "Requêtes READ supplémentaires"; then
        log_message "✅ Requêtes supplémentaires réussies"
        echo "✅ Requêtes supplémentaires réussies"
    fi
else
    log_message "❌ ERREUR: Échec de la lecture des données"
    echo "❌ ERREUR: Échec de la lecture des données"
    exit 1
fi

echo "Consultez le fichier de log pour plus de détails: $LOG_FILE"
