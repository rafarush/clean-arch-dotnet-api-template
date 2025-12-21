using CleanArchTemplate.Domain.Security;

namespace CleanArchTemplate.Infrastructure.Persistence.EntityFramework.Seeders;

public static class PolicySeeder
{
    public static List<Policy> GetPolicyPolicies()
    {
        return
        [
            new Policy
            {
                Name = "view-policy"
            },

            new Policy
            {
                Name = "create-policy"
            },
            
            new Policy
            {
                Name = "update-policy"
            },
            
            new Policy
            {
                Name = "delete-policy"
            }

        ];
    }
    
    public static List<Policy> GetUserPolicies()
    {
        return
        [
            new Policy
            {
                Name = "view-user"
            },

            new Policy
            {
                Name = "create-user"
            },
            
            new Policy
            {
                Name = "update-user"
            },
            
            new Policy
            {
                Name = "delete-user"
            },
            
            new Policy
            {
                Name = "view-me"
            }

        ];
    }
    public static List<Policy> GetRolePolicies()
    {
        return
        [
            new Policy
            {
                Name = "view-role"
            },

            new Policy
            {
                Name = "create-role"
            },
            
            new Policy
            {
                Name = "update-role"
            },
            
            new Policy
            {
                Name = "delete-role"
            }

        ];
    }
}