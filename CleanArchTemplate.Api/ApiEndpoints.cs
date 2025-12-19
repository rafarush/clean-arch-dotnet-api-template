namespace CleanArchTemplate.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";
    
    public static class Users
    {
        private const string BaseUrl = $"{ApiBase}/users";
        
        public const string Create = BaseUrl;
        public const string Get = $"{BaseUrl}/{{id:guid}}";
        public const string Update = $"{BaseUrl}/{{id:guid}}";
        public const string Delete = $"{BaseUrl}/{{id:guid}}";
        public const string GetAll = BaseUrl;
    }
    
    public static class Policies
    {
        private const string BaseUrl = $"{ApiBase}/policies";
        
        public const string Create = BaseUrl;
        public const string Get = $"{BaseUrl}/{{id:guid}}";
        public const string Update = $"{BaseUrl}/{{id:guid}}";
        public const string Delete = $"{BaseUrl}/{{id:guid}}";
        public const string GetAll = BaseUrl;
    }

    public static class Roles
    {
        private const string BaseUrl = $"{ApiBase}/roles";
        
        public const string Create = BaseUrl;
        public const string Get = $"{BaseUrl}/{{id:guid}}";
        public const string Update = $"{BaseUrl}/{{id:guid}}";
        public const string Delete = $"{BaseUrl}/{{id:guid}}";
        public const string GetAll = BaseUrl;
    }
}