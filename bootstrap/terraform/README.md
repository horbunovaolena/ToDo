# Terraform State Bootstrap (Azure Blob Storage)

This folder contains the minimal Terraform configuration to provision the Azure resources required for a remote Terraform state backend (Azure Blob Storage).

Resources created:
- Resource Group
- Storage Account (Standard LRS)
- Private Blob Container

After applying, you can use the outputs to configure the AzureRM backend in your main Terraform configuration.

## Files
- `main.tf` – Declares resource group, storage account, and container.

(If you need a provider or terraform block, you can add a `provider.tf` similar to below, otherwise supply via CLI):
```hcl
terraform {
  required_version = ">= 1.5.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.33.0"
    }
  }
}

provider "azurerm" {
  features {}
  use_cli = true
}
```

## Usage
1. Authenticate to Azure (Azure CLI example):
   ```bash
   az login
   az account set --subscription <SUBSCRIPTION_ID>
   ```
2. Initialize and apply this bootstrap configuration:
   ```bash
   terraform -chdir=bootstrap/terraform init
   terraform -chdir=bootstrap/terraform apply -auto-approve
   ```
3. Note the outputs:
   - `backend_resource_group_name`
   - `backend_storage_account_id` (you will usually need the name; derive it or output it explicitly)
   - `backend_container_name`
   - `backend_key`

   If you also need the storage account *name*, add an output:
   ```hcl
   output "backend_storage_account_name" { value = azurerm_storage_account.state.name }
   ```

4. Create a backend configuration file (e.g. `backend.config`) for your main Terraform root:
   ```hcl
   resource_group_name  = "ToDoApp"
   storage_account_name = "sttfstatestoragetodoapp"
   container_name       = "tfstate"
   key                  = "todo/terraform.tfstate"
   ```

5. In your main Terraform directory (where you want remote state):
   ```bash
   terraform init -backend-config=backend.config
   ```

## Naming / Conventions
- Storage account name must be globally unique, 3-24 lowercase alphanumeric.
- Use different state keys or separate containers for multiple environments (e.g. `env/dev/terraform.tfstate`).

## Cleanup
To remove state resources (danger – destroys remote state container):
```bash
terraform -chdir=bootstrap/terraform destroy
```
Make sure you have migrated / backed up any state you still need.

## Security Notes
- Container access type is private.
- Consider enabling soft delete / versioning manually for protection.
- Lock down access using Azure RBAC (not configured here).

## Next Steps
After the backend is configured, move on to creating application infrastructure in your primary Terraform directory using the AzureRM backend for consistent shared state.
