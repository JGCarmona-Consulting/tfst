variable "sql_server_name" {
  description = "Name of the SQL server"
  type        = string
}

variable "database_name" {
  description = "Name of the SQL database"
  type        = string
}

variable "resource_group_name" {
  description = "Resource group for the SQL server"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
}

variable "admin_username" {
  description = "Administrator username for the SQL server"
  type        = string
}

variable "admin_password" {
  description = "Administrator password for the SQL server"
  type        = string
}

variable "sku_name" {
  description = "SKU for the SQL database (e.g., 'GP_S_Gen5_2')"
  type        = string
}

variable "max_size_gb" {
  description = "Maximum size of the SQL database in GB"
  type        = number
  default     = 32
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default     = {}
}
