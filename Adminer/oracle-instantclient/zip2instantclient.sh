cd "$(dirname "$0")"

# Chercher les fichiers
basiclite_file=$(ls -1 instantclient-basiclite*.zip 2>/dev/null | head -n 1)
sdk_file=$(ls -1 instantclient-sdk*.zip 2>/dev/null | head -n 1)

# Vérifier la présence des deux fichiers
if [[ -z "$basiclite_file" || -z "$sdk_file" ]]; then
  echo "Erreur : Les deux fichiers doivent être présents."
  echo "Fichier basiclite trouvé : $basiclite_file"
  echo "Fichier sdk trouvé : $sdk_file"
  exit 1
fi

# Extraire la version depuis le nom du fichier
basiclite_version=$(echo "$basiclite_file" | sed -n 's/.*-\([0-9_]*\)\.zip/\1/p')
sdk_version=$(echo "$sdk_file" | sed -n 's/.*-\([0-9_]*\)\.zip/\1/p')

if [[ "$basiclite_version" != "$sdk_version" ]]; then
  echo "Erreur : Les fichiers ne sont pas de la même version."
  exit 2
fi

# Décompresser les deux archives sans prompt (overwrite)
unzip -oq "$basiclite_file"
unzip -oq "$sdk_file"

# Vérifier qu'il n'y a qu'un seul dossier instantclient_*
dirs=(instantclient_*)
if [[ ${#dirs[@]} -ne 1 ]]; then
  echo "Erreur : Il doit y avoir exactement un dossier instantclient_*, trouvé : ${#dirs[@]}"
  exit 3
fi

# Renommer le dossier en instantclient
mv "${dirs[0]}" instantclient

echo "Décompression, vérification et renommage réussis."
