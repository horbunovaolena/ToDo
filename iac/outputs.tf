# Output the storage account details for reference
output "storage_account_name" {
  description = "Name of the storage account used for Terraform state"
  value       = azurerm_storage_account.terraform_state.name
}

output "storage_account_primary_access_key" {
  description = "Primary access key for the storage account (sensitive)"
  value       = azurerm_storage_account.terraform_state.primary_access_key
  sensitive   = true
}

output "storage_container_name" {
  description = "Name of the storage container for Terraform state"
  value       = azurerm_storage_container.terraform_state.name
}

output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.to_do_resource_group.name
}