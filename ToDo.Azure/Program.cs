using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System.Collections.Generic;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Create an Azure Resource Group
    var resourceGroup = new ResourceGroup("ToDoHel", new ResourceGroupArgs
    {
        ResourceGroupName = "ToDoHel",
        Location = "East US"
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
                    Action = "Deny"
                }
            },
            ScmIpSecurityRestrictions = new[]
            {
                new IpSecurityRestrictionArgs
                {
                    IpAddress = "Any",
                    Action = "Deny"
                }
            },
            IpSecurityRestrictionsDefaultAction = "Deny",
            ScmIpSecurityRestrictionsDefaultAction = "Deny"
        },
        Identity = new ManagedServiceIdentityArgs
        {
            Type = Pulumi.AzureNative.Web.ManagedServiceIdentityType.SystemAssigned
        }
    });

    // Export the Web App URL
    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = resourceGroup.Name,
        ["webAppName"] = webApp.Name,
        ["webAppUrl"] = webApp.DefaultHostName.Apply(hostname => $"https://{hostname}")
    };
});