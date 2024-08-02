
using System;
using System.Collections.Generic;
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

            var module = Enum.Parse<Modules>(parts[1]);
            var action = Enum.Parse<PermissionAction>(parts[2]);

            var userTypes = new List<UserType>();

            for (int i = 3; i < parts.Length; i++)
            {
                userTypes.Add(Enum.Parse<UserType>(parts[i]));
            }

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(module, action, userTypes))
                .Build();

            return Task.FromResult(policy);
        }

        return BackupPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => BackupPolicyProvider.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => BackupPolicyProvider.GetFallbackPolicyAsync();
}
