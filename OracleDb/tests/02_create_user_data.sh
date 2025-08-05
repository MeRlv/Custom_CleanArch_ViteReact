#!/usr/bin/env bash
set -euo pipefail

# Source common functions
source "$(dirname "$0")/common.sh"

log_message "=== ÉTAPE 2: CREATE - INSERTION DE DONNÉES ==="

# Get container ID and test connection
get_container_id
test_connection

# Insert test data
INSERT_SQL="
INSERT INTO $TABLE_NAME (username, password) VALUES ('user1', 'password123');
INSERT INTO $TABLE_NAME (username, password) VALUES ('user2', 'secret456');
INSERT INTO $TABLE_NAME (username, password) VALUES ('admin', 'admin789');
COMMIT;
"

if execute_sql "$INSERT_SQL" "Insertion de données (CREATE)"; then
    log_message "✅ Données insérées avec succès"
    echo "✅ Données insérées avec succès"
    
    # Verify insertion
    VERIFY_SQL="
    SELECT COUNT(*) AS total_records FROM $TABLE_NAME;
    
    SELECT id, username, password, 
           TO_CHAR(created_date, 'DD/MM/YYYY HH24:MI:SS') AS created_date
    FROM $TABLE_NAME 
    ORDER BY id;
    "
    
    if execute_sql "$VERIFY_SQL" "Vérification des données insérées"; then
        log_message "✅ Vérification réussie"
        echo "✅ Vérification réussie"
    fi
else
    log_message "❌ ERREUR: Échec de l'insertion des données"
    echo "❌ ERREUR: Échec de l'insertion des données"
    exit 1
fi

echo "Consultez le fichier de log pour plus de détails: $LOG_FILE"
