# The Full Stack Team

# Issue: Changes in the `.env` File Not Reflected in the Application

### Problem Description
When changes are made to the `.env` file (e.g., user passwords for RabbitMQ or SQL Server) and containers are deleted to regenerate them, the updates are not reflected. This happens because the old data persists in the volumes configured in Docker, preventing changes to passwords and configurations in the `.env` file from being applied to the new containers.

---

## Solution: Steps to Properly Update the Data

### **Step 1: Remove Containers**
Delete the containers using Docker Desktop or run the following command to stop and remove the current containers:
```bash
docker-compose down
```

### **Step 2: Delete Persistent Volumes**
1. **Using Docker Desktop**:
   - Open Docker Desktop.
   - Navigate to the "Volumes" section.
   - Identify and delete the volumes related to the project.

2. **Using the terminal**:
   List all volumes:
   ```bash
   docker volume ls
   ```
   Then, delete the volumes associated with the project:
   ```bash
   docker volume rm <volume-name>
   ```

### **Step 3: Remove Files Generated in the File System**
1. Locate the folder where Docker stores persistent data. According to the `docker-compose.yml` file, each service has a volume configuration with paths similar to this:
   ```yaml
   volumes:
     - ./infrastructure/docker/<service-name>-data:/data/path
   ```
   For example:
   - SQL Server: `./infrastructure/docker/sqlserver-data`
   - RabbitMQ: `./infrastructure/docker/rabbitmq-data`
   - MongoDB: `./infrastructure/docker/mongo-data`

2. Manually delete the files and directories inside the `docker` folder from your file explorer.

### **Step 4: Rebuild the Containers**
Run the project again or rebuild the containers using the following Docker Compose command. This will regenerate the containers and volumes based on the `docker-compose.yml` configurations:
```bash
docker-compose up --build
```
---

## Important Notes
- During development, you can temporarily disable volumes by removing or commenting out the `volumes` section in the `docker-compose.yml` file. For example:
   ```yaml
   volumes:
     - ./infrastructure/docker/sqlserver-data:/var/opt/mssql/data
   ```

- Remember to re-enable them before moving to production to ensure data persistence.

- Docker will automatically generate new volumes and data in the folders specified in the `docker-compose.yml` file when you rebuild the containers.