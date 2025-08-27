terraform {
  # Remote state backend: Azure Blob Storage (Terraform azurerm backend)
  # Provide actual values via a backend config file (e.g., backend.config) and run:
  #   terraform init -backend-config=backend.config
  # Or pass -backend-config arguments directly on the CLI.
  # The storage account, resource group and container must exist before 'terraform init'.
  backend "azurerm" {
    # resource_group_name  = ""
    # storage_account_name = ""
    # container_name       = ""
    # key                  = "terraform.tfstate"  # Optional: defaults to terraform.tfstate
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.33.0"
    }
  }
}
