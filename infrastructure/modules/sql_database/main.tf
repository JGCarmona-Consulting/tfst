# infrastructure/modules/sql_database/main.tf

# SQL Server
resource "azurerm_mssql_server" "shared" {
  name                         = var.sql_server_name
  resource_group_name          = var.resource_group_name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.admin_username
  administrator_login_password = var.admin_password
}

# SQL Database
resource "azurerm_mssql_database" "shared" {
  name                        = var.database_name
  server_id                   = azurerm_mssql_server.shared.id
  collation                   = "SQL_Latin1_General_CP1_CI_AS" # Default collation
  sku_name                    = "GP_S_Gen5_1"                  # General Purpose SKU, Serverless starts with GP_S_...
  auto_pause_delay_in_minutes = 60                             # Auto-pause after 60 minutes of inactivity
  zone_redundant              = false                          # No redundancy for cost savings
  max_size_gb                 = var.max_size_gb                # Specify max size based on needs
  storage_account_type        = "Local"                        # Almacenamiento local para minimizar costos
  min_capacity                = 0.5

  tags = var.tags

  # Prevent accidental destruction of the database
  lifecycle {
    prevent_destroy = true
  }
}
