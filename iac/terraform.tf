terraform {
  backend "azurerm" {
    resource_group_name  = "ToDoApp"
    storage_account_name = "todoappterraformstate"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.33.0"
    }
  }
}
