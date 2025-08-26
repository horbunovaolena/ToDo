resource "azurerm_resource_group" "to_do_resource_group" {
  location = "polandcentral"
  name     = "ToDoApp"
}
resource "azurerm_user_assigned_identity" "to_do_identity" {
  location            = "polandcentral"
  name                = "oidc-msi-9916"
  resource_group_name = "ToDoApp"
  depends_on = [
    azurerm_resource_group.to_do_resource_group
  ]
}
resource "azurerm_federated_identity_credential" "to_do_federated" {
  audience            = ["api://AzureADTokenExchange"]
  issuer              = "https://token.actions.githubusercontent.com"
  name                = "oidc-credential-9e2f"
  parent_id           = azurerm_user_assigned_identity.to_do_identity.id
  resource_group_name = "ToDoApp"
  subject             = "repo:horbunovaolena/ToDo:ref:refs/heads/main"
}

resource "azurerm_service_plan" "to_do_service_plan" {
  location            = "polandcentral"
  name                = "ToDoApi20250823144752Plan"
  os_type             = "Windows"
  resource_group_name = "ToDoApp"
  sku_name            = "F1"
  depends_on = [
    azurerm_resource_group.to_do_resource_group
  ]
}

resource "azurerm_windows_web_app" "to_do_web_app" {
  client_affinity_enabled                  = true
  ftp_publish_basic_authentication_enabled = false
  https_only                               = true
  location                                 = "polandcentral"
  name                                     = "helenahortodoapp"
  resource_group_name                      = "ToDoApp"
  service_plan_id                          = azurerm_service_plan.to_do_service_plan.id

  webdeploy_publish_basic_authentication_enabled = false
  identity {
    type = "SystemAssigned"
  }
  site_config {
    always_on                         = false
    ftps_state                        = "FtpsOnly"
    ip_restriction_default_action     = "Allow"
    scm_ip_restriction_default_action = "Allow"
  }
}
resource "azurerm_app_service_custom_hostname_binding" "to_do_identity1" {
  app_service_name    = "helenahortodoapp"
  hostname            = "helenahortodoapp.azurewebsites.net"
  resource_group_name = "ToDoApp"
  depends_on = [
    azurerm_windows_web_app.to_do_web_app
  ]
}
