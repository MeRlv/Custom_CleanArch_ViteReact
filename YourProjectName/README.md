# YourProjectName

*The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 9.0.12.*

## Application use cases

Voir plus : `dotnet new ca-usecase --help`

## Pré-requis
```powershell
cd ./src/Application
```

### Ajouter une commande
```powershell
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
```

### Ajouter une requête
```powershell
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

## Entity Framework

Pour gérer les migrations et la mise à jour de la base distante, vous pouvez utiliser les commandes **dotnet ef** en ciblant votre environnement de publication (Local, Dev, Prod).

---

### 1. Créer une migration de schéma

```powershell
dotnet ef migrations add <MIGRATION_NAME>
  --project src/Infrastructure/Infrastructure.csproj
  --startup-project src/Web/Web.csproj
```
> Remplacez `MIGRATION_NAME` par le nom de votre migration (ce paramètre est requis).

---

### 2. Mettre à jour une base distante

Pour appliquer les migrations sur la base distante configurée dans votre fichier `appsettings.{ENV}.json`, il faut définir l'environnement à utiliser pour connaître la base de donnée cible dans `DefaultConnection`.

```powershell
dotnet ef database update
  --environment <ENV_PROFILE>
  --project src/Infrastructure/Infrastructure.csproj
  --startup-project src/Web/Web.csproj
```
> Remplacez `ENV_PROFILE` par le nom de l'environnement cible (`Local`, `Dev`, `Prod`, etc.).

**Explications** :

* `--project` cible l’assembly contenant votre `DbContext`.
* `--startup-project` pointe vers le projet Web où se trouve `Program.CreateHostBuilder(...)`.
* L’environnement (`Local`, `Dev`, `Prod`) détermine quel `appsettings.{ENV}.json` est chargé, et donc la chaîne de connexion utilisée.

### Gestion des utilisateurs

Entity Framework Core propose déjà une gestion de l'authentification, implémenté par `IdentityDbContext<ApplicationUser>` et hérité par `ApplicationDbContext`.