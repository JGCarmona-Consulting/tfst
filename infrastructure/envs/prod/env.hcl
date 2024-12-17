# envs/prod/env.hcl
locals {
  environment_name       = "prod"
  aspnetcore_environment = "Production"
  location               = "spaincentral"

  resource_group_name = "rg-tfst-prod"

  # Select Service Plan SKU
  service_sku = "B1"

  # SQL Database Configuration
  sql_server_name = "tfst-sql-server-prod"
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