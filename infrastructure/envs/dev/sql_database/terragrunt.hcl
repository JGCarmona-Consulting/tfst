# infrastructure/envs/dev/storage/terragrunt.hcl
locals {
  env_vars = read_terragrunt_config(find_in_parent_folders("env.hcl")).locals
}

terraform {
  source = "../../../modules/sql_database"
}

include {
  path = find_in_parent_folders()
}


inputs = {
  resource_group_name = local.env_vars.resource_group_name
  sql_server_name     = local.env_vars.sql_server_name
  database_name       = local.env_vars.database_name
  location            = local.env_vars.location
  admin_username      = local.env_vars.admin_username
  admin_password      = local.env_vars.admin_password
  sku_name            = local.env_vars.sku_name
  max_size_gb         = local.env_vars.max_size_gb
}