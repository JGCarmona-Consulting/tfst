# dev/service_plan/terragrunt.hcl
locals {
  env_vars = read_terragrunt_config(find_in_parent_folders("env.hcl")).locals
}

terraform {
  source = "../../../modules/service_plan"
}

include {
  path = find_in_parent_folders()
}

inputs = {
  environment         = local.env_vars.environment_name
  location            = local.env_vars.location
  resource_group_name = local.env_vars.resource_group_name
  tags                = local.env_vars.default_tags
  # SKU NAME:
  #     - The F1 plan is free.
  #     - The D1 plan is the cheapest plan after the free plan.
  #     - For test and development environments, F1 and D1 are ideal.
  # Change this value to "D1" or "B1" or whatever you diced to select a different SKU
  sku_name = local.env_vars.service_sku
  # Change this value to "Windows" if you want to deploy a Windows Service Plan
  os_type = "Linux"
}
