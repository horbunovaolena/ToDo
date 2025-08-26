# Terraform Azure Blob Storage Backend Configuration

This directory contains Terraform configuration for the ToDo application infrastructure with Azure Blob Storage backend for state management.

## Files Overview

- `main.tf` - Main infrastructure resources including storage account for state
- `terraform.tf` - Terraform and provider configuration with Azure backend
- `variables.tf` - Input variables for the configuration  
- `outputs.tf` - Output values including storage account details
- `provider.tf` - Azure provider configuration
- `migrate-to-blob-storage.ps1` - PowerShell migration script
- `MIGRATION_GUIDE.md` - Detailed migration instructions

## Quick Start

### Option 1: Automated Migration (Recommended)

Run the PowerShell migration script from the `iac` directory:

```powershell
# Make sure you're in the iac directory
cd iac

# Run the migration script
.\migrate-to-blob-storage.ps1
```

The script will:
1. Create the storage account and container
2. Migrate your state from local to Azure Blob Storage
3. Verify the migration

### Option 2: Manual Migration

1. **Initialize with local backend and create storage resources:**
   ```bash
   cd iac
   terraform init
   terraform apply
   ```

2. **Get the storage account access key:**
   ```bash
   az storage account keys list --resource-group ToDoApp --account-name todoappterraformstate --query '[0].value' -o tsv
   ```

3. **Set the access key as environment variable:**
   ```bash
   # Windows PowerShell
   $env:ARM_ACCESS_KEY="<your-storage-key>"
   
   # Windows Command Prompt  
   set ARM_ACCESS_KEY=<your-storage-key>
   
   # Linux/Mac
   export ARM_ACCESS_KEY="<your-storage-key>"
   ```

4. **Migrate the state:**
   ```bash
   terraform init -migrate-state
   # Type 'yes' when prompted
   ```

5. **Verify migration:**
   ```bash
   terraform plan
   # Should show no changes
   ```

## Configuration Details

### Backend Configuration

The Terraform state is now stored in:
- **Resource Group:** ToDoApp
- **Storage Account:** todoappterraformstate  
- **Container:** tfstate
- **State File:** terraform.tfstate

### Key Features

1. **Storage Account with Versioning:** Blob versioning is enabled for state file protection
2. **Lifecycle Protection:** The storage account has `prevent_destroy` to avoid accidental deletion
3. **Variables:** Configurable values for easy customization
4. **Outputs:** Important values are exposed as outputs for reference

### Customization

You can customize the configuration by modifying variables in `variables.tf` or by passing variables during terraform commands:

```bash
terraform apply -var="storage_account_name=myuniquestorageaccount"
```

## Security Considerations

1. **Access Keys:** Store access keys securely, preferably as environment variables
2. **Azure AD Authentication:** Consider using Azure AD instead of access keys:
   ```bash
   az login
   terraform init  # Will use Azure AD authentication
   ```
3. **Storage Account Access:** The container is set to private access
4. **Resource Permissions:** Ensure proper RBAC permissions are configured

## Troubleshooting

### Common Issues

1. **Storage account name already exists:**
   - Change the `storage_account_name` variable to a unique value
   - Storage account names must be globally unique across Azure

2. **Permission denied:**
   - Ensure you have Contributor or Owner role on the subscription/resource group
   - Check that Azure CLI is properly authenticated (`az login`)

3. **State migration fails:**
   - Verify the storage account and container exist
   - Check that the access key is correct
   - Ensure the backend configuration matches the actual resources

### Validation Commands

```bash
# Validate Terraform configuration
terraform validate

# Format Terraform files
terraform fmt

# Check state location
terraform state list

# Show backend configuration  
terraform init -backend=false
```

## Best Practices

1. **Backup:** Always backup your local state file before migration
2. **Environment Variables:** Use environment variables for sensitive data
3. **Resource Locks:** Consider adding resource locks to prevent accidental deletion
4. **Monitoring:** Enable storage analytics and monitoring
5. **Team Access:** Use Azure AD groups for team access management

## CI/CD Integration

For GitHub Actions or other CI/CD systems, ensure:

1. Service Principal or Managed Identity has access to the storage account
2. Backend configuration is provided via environment variables or config files
3. State locking is handled properly in concurrent scenarios

Example GitHub Actions configuration:
```yaml
env:
  ARM_CLIENT_ID: ${{ secrets.ARM_CLIENT_ID }}
  ARM_CLIENT_SECRET: ${{ secrets.ARM_CLIENT_SECRET }}
  ARM_SUBSCRIPTION_ID: ${{ secrets.ARM_SUBSCRIPTION_ID }}
  ARM_TENANT_ID: ${{ secrets.ARM_TENANT_ID }}
  # Access key method:
  # ARM_ACCESS_KEY: ${{ secrets.ARM_ACCESS_KEY }}
```

## Rollback

If you need to revert to local backend:

1. Remove or comment out the backend block in `terraform.tf`
2. Run `terraform init -migrate-state`
3. Confirm migration back to local when prompted

---

For detailed step-by-step instructions, see `MIGRATION_GUIDE.md`.