using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi.AzureNative.Authorization;
using System.Collections.Generic;
using WebManagedServiceIdentityArgs = Pulumi.AzureNative.Web.Inputs.ManagedServiceIdentityArgs;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new ResourceGroup("ToDoHel", new ResourceGroupArgs
    {
        ResourceGroupName = "ToDoHel",
        Location = "Poland Central"
    });

    // Create an App Service Plan for Windows
    var appServicePlan = new AppServicePlan("ToDoAppServicePlan", new AppServicePlanArgs
    {
        Name = "ToDoAppServicePlan",
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        Kind = "Windows",
        Sku = new SkuDescriptionArgs
        {
            Name = "F1",
            Tier = "Free"
        }
    });

    // Create a Windows Web App
    var webApp = new WebApp("helenatodoapp", new WebAppArgs
    {
        Name = "helenatodoapp",
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        ServerFarmId = appServicePlan.Id,
        HttpsOnly = true,
        SiteConfig = new SiteConfigArgs
        {
            AlwaysOn = false,
            NetFrameworkVersion = "v9.0",
            IpSecurityRestrictions = new[]
            {
                new IpSecurityRestrictionArgs
                {
                    IpAddress = "Any",
                    Action = "Allow"
                }
            },
            ScmIpSecurityRestrictions = new[]
            {
                new IpSecurityRestrictionArgs
                {
                    IpAddress = "Any",
                    Action = "Allow"
                }
            },
            IpSecurityRestrictionsDefaultAction = "Allow",
            ScmIpSecurityRestrictionsDefaultAction = "Allow"
        },
        Identity = new WebManagedServiceIdentityArgs
        {
            Type = Pulumi.AzureNative.Web.ManagedServiceIdentityType.SystemAssigned
        }
    });

    // Create role assignment for user with Contributor role
    var roleAssignment = new RoleAssignment("contributorRoleAssignment", new RoleAssignmentArgs
    {
        Scope = resourceGroup.Id,
        RoleDefinitionId = "/subscriptions/{subscription-id}/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c", // Contributor role
        PrincipalId = "076a7472-bbea-4281-9b36-5e27b186a7cc\r\n",
        PrincipalType = PrincipalType.User
    });

    // Export the resource information
    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = resourceGroup.Name,
        ["webAppName"] = webApp.Name,
        ["webAppUrl"] = webApp.DefaultHostName.Apply(hostname => $"https://{hostname}"),
        ["roleAssignmentId"] = roleAssignment.Id
    };
});