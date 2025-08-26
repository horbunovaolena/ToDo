# PowerShell script to migrate Terraform state to Azure Blob Storage
# Run this script from the iac directory

param(
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroupName = "ToDoApp",
    
    [Parameter(Mandatory=$false)]
    [string]$StorageAccountName = "todoappterraformstate",
    
    [Parameter(Mandatory=$false)]
    [string]$ContainerName = "tfstate"
)

Write-Host "Starting Terraform state migration to Azure Blob Storage..." -ForegroundColor Green

# Step 1: Check if we're in the right directory
if (!(Test-Path "terraform.tf")) {
    Write-Error "terraform.tf not found. Please run this script from the iac directory."
    exit 1
}

# Step 2: Check if Azure CLI is installed and logged in
try {
    $azureAccount = az account show --query "name" -o tsv 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Azure CLI not found or not logged in. Please run 'az login' first."
        exit 1
    }
    Write-Host "Logged in to Azure account: $azureAccount" -ForegroundColor Yellow
} catch {
    Write-Error "Error checking Azure CLI status: $_"
    exit 1
}

# Step 3: Initialize Terraform with local backend and create resources
Write-Host "Initializing Terraform and creating Azure resources..." -ForegroundColor Yellow

terraform init
if ($LASTEXITCODE -ne 0) {
    Write-Error "Terraform init failed"
    exit 1
}

Write-Host "Applying Terraform configuration to create storage account..." -ForegroundColor Yellow
terraform apply -auto-approve
if ($LASTEXITCODE -ne 0) {
    Write-Error "Terraform apply failed"
    exit 1
}

# Step 4: Wait a moment for resources to be fully provisioned
Start-Sleep -Seconds 10

# Step 5: Get storage account access key
Write-Host "Retrieving storage account access key..." -ForegroundColor Yellow
try {
    $storageKey = az storage account keys list --resource-group $ResourceGroupName --account-name $StorageAccountName --query '[0].value' -o tsv
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrEmpty($storageKey)) {
        throw "Failed to retrieve storage account key"
    }
} catch {
    Write-Error "Error retrieving storage account key: $_"
    exit 1
}

# Step 6: Set environment variable for ARM_ACCESS_KEY
$env:ARM_ACCESS_KEY = $storageKey
Write-Host "Storage account access key set in environment variable" -ForegroundColor Yellow

# Step 7: Backup current state file
if (Test-Path "terraform.tfstate") {
    Copy-Item "terraform.tfstate" "terraform.tfstate.backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    Write-Host "Local state file backed up" -ForegroundColor Yellow
}

# Step 8: Reinitialize with Azure backend
Write-Host "Reinitializing Terraform with Azure Blob Storage backend..." -ForegroundColor Yellow
Write-Host "When prompted, type 'yes' to migrate the state" -ForegroundColor Cyan

terraform init -migrate-state
if ($LASTEXITCODE -ne 0) {
    Write-Error "Terraform backend migration failed"
    exit 1
}

# Step 9: Verify migration
Write-Host "Verifying migration with terraform plan..." -ForegroundColor Yellow
terraform plan
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Terraform plan showed issues. Please review the output above."
} else {
    Write-Host "Migration completed successfully!" -ForegroundColor Green
    Write-Host "Your Terraform state is now stored in Azure Blob Storage:" -ForegroundColor Green
    Write-Host "  Resource Group: $ResourceGroupName" -ForegroundColor White
    Write-Host "  Storage Account: $StorageAccountName" -ForegroundColor White
    Write-Host "  Container: $ContainerName" -ForegroundColor White
    Write-Host "  Key: terraform.tfstate" -ForegroundColor White
}

# Step 10: Show next steps
Write-Host "`nNext Steps:" -ForegroundColor Cyan
Write-Host "1. Review the terraform plan output above" -ForegroundColor White
Write-Host "2. Consider setting ARM_ACCESS_KEY as a permanent environment variable" -ForegroundColor White
Write-Host "3. Update your CI/CD pipelines to use the new backend configuration" -ForegroundColor White
Write-Host "4. Remove local terraform.tfstate files from version control" -ForegroundColor White