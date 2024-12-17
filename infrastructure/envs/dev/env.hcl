# envs/dev/env.hcl
locals {
  environment_name       = "dev"
  aspnetcore_environment = "Development"
  location               = "spaincentral"

  resource_group_name = "rg-tfst-dev"

  # Select Service Plan SKU
  service_sku = "F1"

  # SQL Database Configuration
  sql_server_name = "tfst-sql-server-dev"
  database_name   = "shared-sql-db"
  admin_username  = ""
  admin_password  = ""
  sku_name        = "GP_S_Gen5_2" # Free tier
  max_size_gb     = 32

  default_tags = {
    project = "TheFullStackTeam"
    owner   = "Juan G Carmona"
  }
}