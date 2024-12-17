locals {
  env_vars = read_terragrunt_config(find_in_parent_folders("env.hcl")).locals
}

terraform {
  source = "../../../../modules/app_services/api"
}

include {
  path = find_in_parent_folders()
}

dependency "service_plan" {
  config_path = "../../service_plan"
}

dependency "sql_database" {
  config_path  = "../../sql_database"
  skip_outputs = false
  mock_outputs = {
    connection_string = "Server=tfst-sqlserver,1433;Database=TheFullStackTeam;User Id=sa;Password=YourStrong@Passw0rd;Encrypt=False;TrustServerCertificate=True;"
  }
}

inputs = {
  environment            = local.env_vars.environment_name
  location               = local.env_vars.location
  resource_group_name    = local.env_vars.resource_group_name
  service_plan_id        = dependency.service_plan.outputs.service_plan_id
  tags                   = local.env_vars.default_tags
  aspnetcore_environment = local.env_vars.aspnetcore_environment

  # Set `always_on` based on the `service_sku`
  always_on = local.env_vars.service_sku != "F1" && local.env_vars.service_sku != "D1"

  # Pass the SQL connection string as an environment variable
  mssql_connection_string = dependency.sql_database.outputs.connection_string
}
