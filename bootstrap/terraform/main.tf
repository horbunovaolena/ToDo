# Bootstrap resources for Terraform remote state (Azure Blob Storage)

resource "azurerm_resource_group" "state" {
  name     = var.resource_group_name
  location = var.location
  tags     = var.tags
}

resource "azurerm_storage_account" "state" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.state.name
  location                 = azurerm_resource_group.state.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  min_tls_version          = "TLS1_2"
  tags                     = var.tags
}

resource "azurerm_storage_container" "state" {
  name                  = var.container_name
  storage_account_id    = azurerm_storage_account.state.id
  container_access_type = "private"
}

output "backend_resource_group_name" {
  value = azurerm_resource_group.state.name
}

output "backend_storage_account_id" {
  value = azurerm_storage_account.state.id
}

output "backend_container_name" {
  value = azurerm_storage_container.state.name
}

output "backend_key" {
  value = "todo/terraform.tfstate"
}
