# infrastructure/envs/terragrunt.hcl
locals {
  # Load the environment variables from the env.hcl file
  environment_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  # Extract the environment name and location from the env.hcl file
  environment_name        = local.environment_vars.locals.environment_name
  location                = local.environment_vars.locals.location
  tfstate_rg_name         = try(local.environment_vars.locals.tfstate_rg_name, "tfst-tfstate-${local.environment_name}")
  tfstate_storage_account = try(local.environment_vars.locals.tfstate_storage_account, "tfsttfstate${local.environment_name}")

  # Get environment variables with default fallback
  admin_username = get_env("SQL_ADMIN_USERNAME", "")
  admin_password = get_env("SQL_ADMIN_PASSWORD", "")

  # Validate missing variables
  missing_variables = [for key, value in { "SQL_ADMIN_USERNAME" = local.admin_username, "SQL_ADMIN_PASSWORD" = local.admin_password } : key if value == ""]

  # Throw an error if variables are missing
  validation_error = length(local.missing_variables) > 0 ? error("Missing required environment variables: ${join(", ", local.missing_variables)}") : null
}


# Generate an Azure provider block
generate "provider" {
  path      = "provider.tf"
  if_exists = "overwrite_terragrunt"
  contents  = <<EOF
    provider "azurerm" {
      features {}
    }
EOF
}

# Terraform version and provider requirements
generate "versions" {
  path      = "versions_override.tf"
  if_exists = "overwrite_terragrunt"
  contents  = <<EOF
    terraform {
      required_version = ">=1.3.0"
      required_providers {
        azurerm = {
          source  = "hashicorp/azurerm"
          version = "~> 4.11.0"
        }
      }
    }
EOF
}

# Configure Terragrunt to automatically store tfstate files in Azure storage container
remote_state {
  backend = "azurerm"
  config = {
    resource_group_name  = local.tfstate_rg_name
    storage_account_name = local.tfstate_storage_account
    container_name       = "tfstate-container"
    key                  = "${path_relative_to_include()}/terraform.tfstate"
    subscription_id      = get_env("ARM_SUBSCRIPTION_ID", "")
    tenant_id            = get_env("ARM_TENANT_ID", "")
  }
  generate = {
    path      = "backend.tf"
    if_exists = "overwrite_terragrunt"
  }
}
