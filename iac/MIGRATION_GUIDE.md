# Migration Guide: Local to Azure Blob Storage Backend

This guide will help you migrate your Terraform state from local storage to Azure Blob Storage.

## Prerequisites

1. Azure CLI installed and authenticated
2. Terraform CLI installed
3. Proper permissions to create storage accounts in your Azure subscription

## Migration Steps

### Step 1: Initialize with Local Backend (Current State)
If you haven't already, make sure your current state is up to date:

```bash
# Navigate to the iac directory
cd iac

# Initialize with local backend (current configuration)
terraform init

# Apply to create the storage account and container
terraform apply
```

### Step 2: Get Storage Account Access Key
After the storage account is created, get the access key:

```bash
# Get the storage account access key
az storage account keys list --resource-group ToDoApp --account-name todoappterraformstate --query '[0].value' -o tsv
```

### Step 3: Configure Environment Variables (Optional but Recommended)
Set up environment variables for the backend configuration:

```bash
# Windows (Command Prompt)
set ARM_ACCESS_KEY=<storage_account_access_key>

# Windows (PowerShell)
$env:ARM_ACCESS_KEY="<storage_account_access_key>"

# Linux/Mac
export ARM_ACCESS_KEY="<storage_account_access_key>"
```

### Step 4: Reinitialize with Azure Backend
After updating terraform.tf with the azurerm backend:

```bash
# Reinitialize Terraform with the new backend
terraform init -migrate-state

# When prompted, type 'yes' to migrate the existing state
```

### Step 5: Verify the Migration
```bash
# Verify that the state has been migrated
terraform plan

# The plan should show no changes if migration was successful
```

## Alternative: Manual Backend Configuration

If you prefer not to hardcode backend configuration in terraform.tf, you can use a backend config file:

### Create backend.conf:
```hcl
resource_group_name  = "ToDoApp"
storage_account_name = "todoappterraformstate"
container_name       = "tfstate"
key                  = "terraform.tfstate"
```

### Initialize with config file:
```bash
terraform init -backend-config="backend.conf"
```

## Troubleshooting

### Issue: Storage account name already exists
If you get an error about the storage account name being taken, update the `storage_account_name` variable in variables.tf to use a unique name.

### Issue: Access denied
Make sure you have:
1. Proper Azure permissions (Contributor or Owner role)
2. Correct storage account access key
3. The storage account and container exist

### Issue: State file not found
If you get a "state file not found" error, it might mean:
1. The migration didn't complete successfully
2. The backend configuration doesn't match the actual storage location
3. The storage account access key is incorrect

## Best Practices

1. **Backup your local state file** before migration:
   ```bash
   cp terraform.tfstate terraform.tfstate.backup
   ```

2. **Use environment variables** for sensitive information like access keys

3. **Enable storage account features**:
   - Versioning (already enabled in the configuration)
   - Soft delete for additional protection
   - Access logging for audit trails

4. **Consider using Azure AD authentication** instead of access keys for better security:
   ```bash
   # Login with Azure CLI
   az login
   
   # Use Azure AD authentication (no need for ARM_ACCESS_KEY)
   terraform init
   ```

## Cleanup

If you need to revert to local backend:
1. Comment out or remove the backend block in terraform.tf
2. Run `terraform init -migrate-state`
3. When prompted, choose to migrate state back to local