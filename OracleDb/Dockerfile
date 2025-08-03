# OracleDb/Dockerfile
FROM gvenzl/oracle-xe:21.3.0-slim

# Declare volume to persist Oracle data
VOLUME ["/opt/oracle/oradata"]

# Default environment variables
ENV ORACLE_PASSWORD=oracle \
  APP_USER=app \
  APP_USER_PASSWORD=app

# Expose the Oracle port
EXPOSE 1521

# Copy the user creation script
COPY create-app-user.sql /container-entrypoint-initdb.d/