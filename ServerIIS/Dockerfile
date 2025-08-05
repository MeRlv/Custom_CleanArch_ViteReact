# Utilise l'image de base ASP.NET Core runtime pour Linux (.NET 9.0)
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Crée un utilisateur non-root pour la sécurité
RUN adduser --disabled-password --gecos '' --shell /bin/bash --home /home/appuser appuser

# Définit le répertoire de travail
WORKDIR /app

# Copie les fichiers de l'application
COPY build/ ./

# Change le propriétaire des fichiers vers l'utilisateur appuser
RUN chown -R appuser:appuser /app

# Passe à l'utilisateur non-root
USER appuser

# Expose le port 80 pour HTTP
EXPOSE 80

# Point d'entrée pour démarrer l'application ASP.NET Core
ENTRYPOINT ["dotnet", "YourProjectName.Web.dll"]