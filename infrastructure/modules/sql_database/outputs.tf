# infrastructure/modules/sql_database/outputs.tf
output "connection_string" {
  value     = "Server=${azurerm_mssql_server.shared.fully_qualified_domain_name};Database=${azurerm_mssql_database.shared.name};User ID=${var.admin_username};Password=${var.admin_password};"
  sensitive = true
}
