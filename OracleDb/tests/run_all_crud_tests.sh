#!/usr/bin/env bash
cd "$(dirname "$0")"
set -euo pipefail

echo "🚀 Lancement de la suite complète de tests CRUD Oracle DB"
echo "=================================================="

# Get the directory of this script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Array of test scripts in order
TESTS=(
    "01_create_user_table.sh"
    "02_create_user_data.sh"
    "03_read_user_data.sh"
    "04_update_user_data.sh"
    "05_delete_user_data.sh"
    "06_delete_user_table.sh"
)

# Execute each test
for i in "${!TESTS[@]}"; do
    test_file="${TESTS[$i]}"
    test_number=$((i + 1))
    
    echo ""
    echo "📋 Exécution du test $test_number/6: $test_file"
    echo "----------------------------------------"
    
    if [[ -x "$SCRIPT_DIR/$test_file" ]]; then
        if "$SCRIPT_DIR/$test_file"; then
            echo "✅ Test $test_number réussi: $test_file"
        else
            echo "❌ Échec du test $test_number: $test_file"
            echo "🛑 Arrêt de la suite de tests"
            exit 1
        fi
    else
        echo "❌ Fichier de test non trouvé ou non exécutable: $test_file"
        exit 1
    fi
    
    # Small pause between tests
    sleep 1
done

echo ""
echo "🎉 Suite complète de tests CRUD terminée avec succès!"
echo "=================================================="
echo "Tous les 6 tests ont été exécutés avec succès:"
echo "  1. ✅ Création de la table"
echo "  2. ✅ Insertion des données (CREATE)"
echo "  3. ✅ Lecture des données (READ)"
echo "  4. ✅ Modification des données (UPDATE)"
echo "  5. ✅ Suppression des données (DELETE)"
echo "  6. ✅ Suppression de la table (CLEANUP)"
echo ""
echo "📄 Consultez les logs dans le répertoire ./output/ pour plus de détails"
