# Meal Planner Backend

This is the backend API for the Meal Planner project.

Setups:
1. Clone the repository
2. Install .NET 8.0 (https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.203-windows-x64-installer)
3. Open the solution MPWebApi. You can now run the solution using Visual Studio
4. Set Up Database

## Database Setup
The project utilizes MySQL as the core database for the whole project. For simplicity, Docker would be used to launch the 
the database. In order to run MySQL using Docker, please following the below steps:
#### 1. Install Docker
For Docker installation, please refer to the official [Docker Engine Installation](https://docs.docker.com/engine/install/)

#### 2. Install Docker-Compose
Docker-Compose library is would be readily installed when we use Docker Desktop. In case you could not successfully find
Docker-Compose in your machine. Please following the [Docker Compose Installation](https://docs.docker.com/compose/install/)

#### 3. Set Up MySQL for Local Development
After completing the above two steps, from the root directory (where the `docker-compose.yml` is located) of this 
repository, run the below commands to run MySQL. The database port would be exposed via port 3306. You can change database 
port into different one by editing the `docker-compose.yml` file.
```bash
docker compose up -d
```
or
```bash
docker-compose up -d
```
