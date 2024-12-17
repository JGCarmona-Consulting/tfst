# Start from Ubuntu as base image
FROM catthehacker/ubuntu:act-latest

# Set environment variables for non-interactive installations
ENV DEBIAN_FRONTEND=noninteractive

# Install prerequisites
RUN apt-get update && \
    apt-get install -y \
    curl \
    unzip \
    jq \
    python3-pip \
    software-properties-common \
    gnupg2 && \
    apt-get clean

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

# Install Terraform
RUN curl -fsSL https://apt.releases.hashicorp.com/gpg | gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(lsb_release -cs) main" | tee /etc/apt/sources.list.d/hashicorp.list && \
    apt-get update && \
    apt-get install -y terraform

# Install Terragrunt
RUN curl -sL https://github.com/gruntwork-io/terragrunt/releases/download/v0.69.0/terragrunt_linux_amd64 -o /usr/local/bin/terragrunt && \
    chmod +x /usr/local/bin/terragrunt

# Add Azure CLI and Terraform/Terragrunt to PATH
ENV PATH=$PATH:/usr/bin:/usr/local/bin

# Set the working directory
WORKDIR /workspace

# Default entrypoint for the container
ENTRYPOINT ["/bin/bash"]
