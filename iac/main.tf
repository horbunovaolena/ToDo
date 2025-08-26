resource "azurerm_resource_group" "to_do_resource_group" {
  location = var.location
  name     = var.resource_group_name
}

# Storage Account for Terraform state
resource "azurerm_storage_account" "terraform_state" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.to_do_resource_group.name
  location                 = azurerm_resource_group.to_do_resource_group.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  
  # Enable versioning for state file protection
  blob_properties {
    versioning_enabled = true
  }

  # Prevent accidental deletion
  lifecycle {
    prevent_destroy = true
  }

  depends_on = [
    azurerm_resource_group.to_do_resource_group
  ]
}

# Container for Terraform state
resource "azurerm_storage_container" "terraform_state" {
  name                  = "tfstate"
  storage_account_name  = azurerm_storage_account.terraform_state.name
  container_access_type = "private"
}

resource "azurerm_user_assigned_identity" "to_do_identity" {
  location            = var.location
  name                = "oidc-msi-9916"
  resource_group_name = var.resource_group_name
  depends_on = [
    azurerm_resource_group.to_do_resource_group
  ]
}

resource "azurerm_federated_identity_credential" "to_do_federated" {
  audience            = ["api://AzureADTokenExchange"]
  issuer              = "https://token.actions.githubusercontent.com"
  name                = "oidc-credential-9e2f"
  parent_id           = azurerm_user_assigned_identity.to_do_identity.id
  resource_group_name = var.resource_group_name
  subject             = "repo:${var.github_repo}:ref:refs/heads/main"
}

resource "azurerm_service_plan" "to_do_service_plan" {
  location            = var.location
  name                = "ToDoApi20250823144752Plan"
  os_type             = "Windows"
  resource_group_name = var.resource_group_name
  sku_name            = "F1"
  depends_on = [
    azurerm_resource_group.to_do_resource_group
  ]
}

resource "azurerm_windows_web_app" "to_do_web_app" {
  client_affinity_enabled                  = true
  ftp_publish_basic_authentication_enabled = false
  https_only                               = true
  location                                 = var.location
  name                                     = var.app_service_name
  resource_group_name                      = var.resource_group_name
  service_plan_id                          = azurerm_service_plan.to_do_service_plan.id

  webdeploy_publish_basic_authentication_enabled = false
  identity {
    type = "SystemAssigned"
  }
  site_config {
    always_on                         = false
    ftps_state                        = "FtpsOnly"
    ip_restriction_default_action     = ""
    scm_ip_restriction_default_action = ""
  }
}

resource "azurerm_app_service_custom_hostname_binding" "to_do_identity1" {
  app_service_name    = var.app_service_name
  hostname            = "${var.app_service_name}.azurewebsites.net"
  resource_group_name = var.resource_group_name
  depends_on = [
    azurerm_windows_web_app.to_do_web_app
  ]
}
