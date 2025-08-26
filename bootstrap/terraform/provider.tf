provider "azurerm" {
  features {
  }
  use_cli                         = true
  use_oidc                        = false
  resource_provider_registrations = "none"
  subscription_id                 = "43ca6b39-09a5-4bc0-ab3d-00b5644efd93"
  environment                     = "public"
  use_msi                         = false
}
