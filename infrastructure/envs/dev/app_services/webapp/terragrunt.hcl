# infrastructure/envs/dev/app_services/web/terragrunt.hcl
locals {
  env_vars = read_terragrunt_config(find_in_parent_folders("env.hcl")).locals
}

terraform {
  source = "../../../../modules/app_services/webapp"
}

include {
  path = find_in_parent_folders()
}

dependency "service_plan" {
  config_path = "../../service_plan"
}

dependency "api" {
  config_path  = "../api"
  skip_outputs = false
  mock_outputs = {
    api_url = "http://mock-api-url"
  }
}

inputs = {
  environment         = local.env_vars.environment_name
  location            = local.env_vars.location
  resource_group_name = local.env_vars.resource_group_name
  service_plan_id     = dependency.service_plan.outputs.service_plan_id
  tags                = local.env_vars.default_tags

  # Set `always_on` based on the `service_sku`
  always_on = local.env_vars.service_sku != "F1" && local.env_vars.service_sku != "D1"

  # Use the API service's URL as an environment variable
  api_url = dependency.api.outputs.default_hostname
}
