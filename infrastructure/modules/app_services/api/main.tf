# /infrastructure/modules/app_services/api/main.tf
resource "azurerm_linux_web_app" "api" {
  name                = "tfst-api-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = var.service_plan_id
  https_only          = true
  site_config {
    application_stack {
      dotnet_version = "8.0" # Use application_stack to specify the .NET version
    }
    always_on = var.always_on
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"               = var.aspnetcore_environment
    "ConnectionStrings__DefaultConnection" = var.mssql_connection_string
  }

  tags = var.tags
}
