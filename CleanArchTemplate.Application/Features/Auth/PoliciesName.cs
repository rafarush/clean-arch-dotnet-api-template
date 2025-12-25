namespace CleanArchTemplate.Aplication.Features.Auth;

public static class PoliciesName
{
    public static class User
    {
        public const string View = "view-user";
        public const string ViewMe = "me-user";
        public const string Create = "create-user";
        public const string Update = "update-user";
        public const string Delete = "delete-user";
    }

    public static class Role
    {
        public const string View = "view-role";
        public const string Create = "create-role";
        public const string Update = "update-role";
        public const string Delete = "delete-role";
    }

    public static class Policy
    {
        public const string View = "view-policy";
        public const string Create = "create-policy";
        public const string Update = "update-policy";
        public const string Delete = "delete-policy";
    }
}