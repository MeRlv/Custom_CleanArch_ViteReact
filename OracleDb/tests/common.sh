#!/usr/bin/env bash
# Common functions and variables for CRUD tests

COMPOSE_PROJECT="cleanarch9_reactvitejs_template"
SERVICE_NAME="oracle-db"
DB_USER="app"
DB_PASS="app"
HOST_PORT="1521"
DB_SERVICE="FREEPDB1"
TABLE_NAME="Users"

# Create output directory if it doesn't exist
mkdir -p ./output

# Log file (shared across all tests)
LOG_FILE="./output/crud_test_$(date +%Y%m%d_%H%M%S).log"

# Function to log messages
log_message() {
    local message="$1"
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $message" | tee -a "$LOG_FILE"
}

# Function to execute SQL and log results
execute_sql() {
    local sql_command="$1"
    local step_name="$2"
    
    log_message "=== $step_name ==="
    log_message "Exécution SQL: $sql_command"
    
    # Execute SQL command
    local result
    result=$(docker exec --user oracle "$CONTAINER_ID" bash -lc "
        sqlplus -s ${DB_USER}/${DB_PASS}@//127.0.0.1:${HOST_PORT}/${DB_SERVICE} <<-EOF
            SET PAGESIZE 0 FEEDBACK ON VERIFY OFF HEADING ON ECHO OFF
            SET SERVEROUTPUT ON
            $sql_command
            EXIT
EOF
    " 2>&1)
    
    # Log the result
    echo "$result" | tee -a "$LOG_FILE"
    
    # Check for errors
    if echo "$result" | grep -i "ORA-" > /dev/null; then
        log_message "ERREUR détectée dans l'étape: $step_name"
        return 1
    else
        log_message "Étape réussie: $step_name"
        return 0
    fi
}

# Function to get container ID
get_container_id() {
    log_message "Recherche du conteneur '$SERVICE_NAME' dans le projet '$COMPOSE_PROJECT'..."
    CONTAINER_ID=$(docker compose -p "$COMPOSE_PROJECT" ps -q "$SERVICE_NAME")
    if [[ -z "$CONTAINER_ID" ]]; then
        log_message "ERREUR: Conteneur non trouvé."
        docker compose -p "$COMPOSE_PROJECT" ps | tee -a "$LOG_FILE"
        exit 1
    fi
    log_message "ID conteneur trouvé: $CONTAINER_ID"
}

# Function to test connection
test_connection() {
    log_message "Test de connexion à la base de données..."
    if ! execute_sql "SELECT 'Connexion réussie' AS status FROM dual;" "Test de connexion"; then
        log_message "ERREUR: Impossible de se connecter à la base de données"
        exit 1
    fi
}
