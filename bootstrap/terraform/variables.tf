variable "location" {
  type        = string
  description = "Azure region for state resources"
  default     = "polandcentral"
}

variable "resource_group_name" {
  type        = string
  description = "Resource Group name for Terraform state"
  default     = "ToDoApp"
}

variable "storage_account_name" {
  type        = string
  description = "Globally unique Storage Account name for Terraform state"
  default     = "sttfstatestoragetodoapp"
}

variable "container_name" {
  type        = string
  description = "Blob container name for Terraform state"
  default     = "tfstate"
}

variable "tags" {
  type        = map(string)
  description = "Common tags"
  default = {
    environment = "dev"
    workload    = "todo-app"
    component   = "terraform-state"
  }
}
