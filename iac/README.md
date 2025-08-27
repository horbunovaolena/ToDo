# Terraform Infrastructure

This directory contains Terraform code for the project.

## Remote State Backend (Azure Blob Storage)

The root configuration (../iac) uses an Azure Blob Storage backend. Because the backend resources must exist before Terraform can use them, we provide a separate bootstrap configuration that creates them using a local backend first.

### 1. Bootstrap the Backend Resources

```
terraform -chdir=iac/bootstrap init
terraform -chdir=iac/bootstrap apply -auto-approve
```

Outputs will include the names required for backend configuration.

### 2. Configure the Backend

Copy iac/backend.config.example to iac/backend.config and fill in the values from the bootstrap outputs:

```
resource_group_name  = "<rg name>"
storage_account_name = "<storage account name>"
container_name       = "<container name>"
key                  = "todo/terraform.tfstate"
```

### 3. Initialize Root Configuration with Remote State

```
terraform -chdir=iac init -backend-config=backend.config
```

### 4. Apply Further Infrastructure (if any)

Add additional .tf files in the iac directory for application infrastructure and run:

```
terraform -chdir=iac plan
terraform -chdir=iac apply
```

### Notes

- The storage account name must be globally unique and 3-24 lowercase alphanumeric characters.
- Adjust location and naming in iac/bootstrap/variables.tf as needed.
- State key can be changed to isolate environments (e.g., todo/dev/terraform.tfstate, todo/prod/terraform.tfstate).
- For multiple environments, parameterize names or use workspaces plus distinct keys.
