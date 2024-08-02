
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using P2PLoan.Constants;
using P2PLoan.Models;
using P2PLoan.Requirements;

namespace P2PLoan.Providers;
public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

    public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("Permission"))
        {
            var parts = policyName.Split(':');
            if (parts.Length == 4)
            {
                var module = Enum.Parse<Modules>(parts[1]);
                var action = Enum.Parse<PermissionAction>(parts[2]);
                var userType = Enum.Parse<UserType>(parts[3]);

                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(module, action, userType))
                    .Build();

                return Task.FromResult(policy);
            }
        }

        return BackupPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => BackupPolicyProvider.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => BackupPolicyProvider.GetFallbackPolicyAsync();
}
