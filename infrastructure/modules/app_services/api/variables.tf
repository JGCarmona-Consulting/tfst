variable "location" {}
variable "resource_group_name" {}
variable "environment" {}
variable "service_plan_id" {}
variable "tags" {
  type    = map(string)
  default = {}
}

variable "always_on" {
  description = "Enable Always On setting for the Web App"
  type        = bool
  default     = true
}

variable "aspnetcore_environment" {
  description = "The ASP.NET Core environment (e.g., Development, Staging, Production)"
  type        = string
}

variable "mssql_connection_string" {
  description = "The database connection string for the API"
  type        = string
}