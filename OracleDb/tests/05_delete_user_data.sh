#!/usr/bin/env bash
set -euo pipefail

# Source common functions
source "$(dirname "$0")/common.sh"

log_message "=== ÉTAPE 5: DELETE - SUPPRESSION DE DONNÉES ==="

# Get container ID and test connection
get_container_id
test_connection

# Show data before deletion
BEFORE_DELETE_SQL="
SELECT 'Données avant suppression:' AS info FROM dual;

SELECT id, username, password FROM $TABLE_NAME ORDER BY id;

SELECT COUNT(*) AS total_before_delete FROM $TABLE_NAME;
"

execute_sql "$BEFORE_DELETE_SQL" "État avant suppression"

# Delete specific record
DELETE_SQL="
DELETE FROM $TABLE_NAME WHERE username = 'user2';
COMMIT;
"

if execute_sql "$DELETE_SQL" "Suppression d'un enregistrement (DELETE)"; then
    log_message "✅ Enregistrement supprimé avec succès"
    echo "✅ Enregistrement supprimé avec succès"
    
    # Verify deletion
    VERIFY_DELETE_SQL="
    SELECT 'Données après suppression:' AS info FROM dual;
    
    SELECT COUNT(*) AS remaining_records FROM $TABLE_NAME;

    SELECT id, username, password FROM $TABLE_NAME ORDER BY id;
    
    -- Vérifier que user2 n'existe plus
    SELECT 'Vérification suppression user2' AS verification, COUNT(*) AS should_be_zero
    FROM $TABLE_NAME 
    WHERE username = 'user2';
    "

    if execute_sql "$VERIFY_DELETE_SQL" "Vérification après suppression"; then
        log_message "✅ Vérification après suppression réussie"
        echo "✅ Vérification après suppression réussie"
    fi
else
    log_message "❌ ERREUR: Échec de la suppression de l'enregistrement"
    echo "❌ ERREUR: Échec de la suppression de l'enregistrement"
    exit 1
fi

echo "Consultez le fichier de log pour plus de détails: $LOG_FILE"
