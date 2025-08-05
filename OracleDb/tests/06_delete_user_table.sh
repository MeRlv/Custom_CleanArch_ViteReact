#!/usr/bin/env bash
set -euo pipefail

# Source common functions
source "$(dirname "$0")/common.sh"

log_message "=== ÉTAPE 6: CLEANUP - SUPPRESSION DE LA TABLE ==="

# Get container ID and test connection
get_container_id
test_connection

# Show final state before cleanup
FINAL_STATE_SQL="
SELECT 'État final avant nettoyage:' AS info FROM dual;

SELECT COUNT(*) AS final_record_count FROM $TABLE_NAME;

SELECT id, username, password FROM $TABLE_NAME ORDER BY id;
"

execute_sql "$FINAL_STATE_SQL" "État final des données"

# Drop table and cleanup
CLEANUP_SQL="
DROP TABLE $TABLE_NAME;
DROP SEQUENCE ${TABLE_NAME}_seq;
"

if execute_sql "$CLEANUP_SQL" "Suppression de la table et nettoyage"; then
    log_message "✅ Nettoyage réussi - Table $TABLE_NAME supprimée"
    echo "✅ Nettoyage réussi - Table $TABLE_NAME supprimée"
    
    # Final verification
    FINAL_CHECK_SQL="
    SELECT COUNT(*) AS table_exists 
    FROM user_tables 
    WHERE table_name = UPPER('$TABLE_NAME');
    
    SELECT 'Table supprimée avec succès' AS status FROM dual 
    WHERE NOT EXISTS (
        SELECT 1 FROM user_tables WHERE table_name = UPPER('$TABLE_NAME')
    );
    "

    if execute_sql "$FINAL_CHECK_SQL" "Vérification finale"; then
        log_message "✅ Vérification finale réussie"
        echo "✅ Vérification finale réussie"
        
        # Final summary
        log_message "=== RÉSUMÉ FINAL DU TEST CRUD ==="
        log_message "✅ CREATE TABLE: Table créée avec succès"
        log_message "✅ CREATE DATA: Données insérées avec succès"
        log_message "✅ READ: Données lues avec succès"
        log_message "✅ UPDATE: Données modifiées avec succès"
        log_message "✅ DELETE: Données supprimées avec succès"
        log_message "✅ CLEANUP: Table supprimée avec succès"
        log_message "=== TEST CRUD TERMINÉ AVEC SUCCÈS ==="
        
        echo ""
        echo "🎉 Test CRUD complet terminé avec succès!"
    fi
else
    log_message "❌ ERREUR: Échec du nettoyage"
    echo "❌ ERREUR: Échec du nettoyage"
    exit 1
fi

echo "Consultez le fichier de log pour plus de détails: $LOG_FILE"
