# Variables for Terraform configuration
variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "polandcentral"
}

variable "resource_group_name" {
  description = "The name of the resource group"
  type        = string
  default     = "ToDoApp"
}

variable "storage_account_name" {
  description = "The name of the storage account for Terraform state"
  type        = string
  default     = "todoappterraformstate"
  
  validation {
    condition     = can(regex("^[a-z0-9]{3,24}$", var.storage_account_name))
    error_message = "Storage account name must be between 3 and 24 characters long and contain only lowercase letters and numbers."
  }
}

variable "app_service_name" {
  description = "The name of the App Service"
  type        = string
  default     = "helenahortodoapp"
}

variable "github_repo" {
  description = "GitHub repository in format owner/repo"
  type        = string
  default     = "horbunovaolena/ToDo"
}