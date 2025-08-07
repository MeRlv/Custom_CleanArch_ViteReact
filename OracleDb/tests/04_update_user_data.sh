#!/usr/bin/env bash
cd "$(dirname "$0")"
set -euo pipefail

# Source common functions
source common.sh

log_message "=== ÉTAPE 4: UPDATE - MODIFICATION DES DONNÉES ==="

# Get container ID and test connection
get_container_id
test_connection

# Update data
UPDATE_SQL="
UPDATE $TABLE_NAME 
SET password = 'newpassword123', updated_date = SYSDATE 
WHERE username = 'user1';

UPDATE $TABLE_NAME 
SET username = 'superadmin', password = 'supersecret', updated_date = SYSDATE 
WHERE username = 'admin';

COMMIT;
"

if execute_sql "$UPDATE_SQL" "Modification des données (UPDATE)"; then
    log_message "✅ Données modifiées avec succès"
    echo "✅ Données modifiées avec succès"
    
    # Verify updates
    VERIFY_UPDATE_SQL="
    SELECT id, username, password, 
           TO_CHAR(created_date, 'DD/MM/YYYY HH24:MI:SS') AS created_date,
           TO_CHAR(updated_date, 'DD/MM/YYYY HH24:MI:SS') AS updated_date
    FROM $TABLE_NAME 
    ORDER BY id;
    
    -- Vérifier que les modifications ont bien eu lieu
    SELECT 'Mise à jour user1' AS verification, COUNT(*) AS count
    FROM $TABLE_NAME 
    WHERE username = 'user1' AND password = 'newpassword123'
    UNION ALL
    SELECT 'Mise à jour admin->superadmin' AS verification, COUNT(*) AS count
    FROM $TABLE_NAME 
    WHERE username = 'superadmin' AND password = 'supersecret';
    "
    
    if execute_sql "$VERIFY_UPDATE_SQL" "Vérification des modifications"; then
        log_message "✅ Vérification des modifications réussie"
        echo "✅ Vérification des modifications réussie"
    fi
else
    log_message "❌ ERREUR: Échec de la modification des données"
    echo "❌ ERREUR: Échec de la modification des données"
    exit 1
fi

echo "Consultez le fichier de log pour plus de détails: $LOG_FILE"
