
# Dockerfile for `act` with Azure CLI Support

This container is designed to run GitHub Actions workflows locally using `act`, including support for Azure CLI (`az`) and additional tools like `terragrunt`.

## Instructions

### 1. Build the Image
Run the following command from the root of the repository:
```bash
docker build -t my-act-image -f .devcontainer/Dockerfile .
```

### 2. Use the Image with `act`
When executing `act`, specify the custom image for the platforms used in your workflows:
```bash
act workflow_dispatch -W .github/workflows/api-dev-cd.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

## Configuration of the `.secrets` file

To run the workflows locally, you need a `.secrets` file in the root of the project. This file contains the credentials required to authenticate with Azure and manage infrastructure resources.

### Example of `.secrets`

Create a `.secrets` file in the root of the project and add the necessary keys in the following format:

```plaintext
AZURE_CREDENTIALS_DEV=<JSON with Azure credentials for the DEV environment>
ARM_SUBSCRIPTION_ID=<Azure Subscription ID>
ARM_CLIENT_ID=<Azure Client ID>
ARM_CLIENT_SECRET=<Azure Client Secret>
ARM_TENANT_ID=<Azure Tenant ID>
```

note: AZURE_CREDENTIALS_DEV is a json, just search for that format.

## Tools Included
- Azure CLI
- Terragrunt
- jq
- curl

## Purpose
This container ensures a consistent environment for testing workflows locally, particularly those involving Azure resources.

# Commands to run pipelines locally with `act`

## Infrastructure Validation

### Validation for `dev`
```bash
act workflow_dispatch -W .github/workflows/infra-dev-validation.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

### Validation for `prod`
```bash
act workflow_dispatch -W .github/workflows/infra-prod-validation.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

## Infrastructure Deployment

### Deployment for `dev`
```bash
act workflow_dispatch -W .github/workflows/infra-dev-deploy.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

### Deployment for `prod`
```bash
act workflow_dispatch -W .github/workflows/infra-prod-deploy.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

## CI for Backend API

### CI for `dev`
```bash
act workflow_dispatch -W .github/workflows/api-dev-ci.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

### CI for `prod`
```bash
act workflow_dispatch -W .github/workflows/api-prod-ci.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

##Backend API Deployment

### Deployment for `dev`
```bash
act workflow_dispatch -W .github/workflows/api-dev-cd.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

### Deployment for `prod`
```bash
act workflow_dispatch -W .github/workflows/api-prod-cd.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

## CI for Frontend Angular

### CI for `dev`
```bash
act workflow_dispatch -W .github/workflows/webapp-dev-ci.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

### CI for `prod`
```bash
act workflow_dispatch -W .github/workflows/webapp-prod-ci.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

## Angular Frontend Deployment

### Deployment for `dev`
```bash
act workflow_dispatch -W .github/workflows/webapp-dev-deploy.yml -P ubuntu-latest=my-act-image:latest --pull=false
```

### Deployment for `prod`
```bash
act workflow_dispatch -W .github/workflows/webapp-prod-deploy.yml -P ubuntu-latest=my-act-image:latest --pull=false
```