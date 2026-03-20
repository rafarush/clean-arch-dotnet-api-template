namespace CleanArchTemplate.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";
    
    public static class Users
    {
        public const string BaseUrl = $"{ApiBase}/users";
        
        public const string Create = BaseUrl;
        public const string Get = $"{BaseUrl}/{{id:guid}}";
        public const string Update = $"{BaseUrl}/{{id:guid}}";
        public const string Delete = $"{BaseUrl}/{{id:guid}}";
        public const string GetAll = BaseUrl;
        public const string Search = BaseUrl;
        public const string AssignRolesToUser =  $"{BaseUrl}/{{id:guid}}/assign";
    }
    
    public static class Policies
    {
        public const string BaseUrl = $"{ApiBase}/policies";
        
        public const string Create = BaseUrl;
        public const string Get = $"{BaseUrl}/{{id:guid}}";
        public const string Update = $"{BaseUrl}/{{id:guid}}";
        public const string Delete = $"{BaseUrl}/{{id:guid}}";
        public const string GetAll = BaseUrl;
    }

    public static class Roles
    {
        public const string BaseUrl = $"{ApiBase}/roles";
        
        public const string Create = BaseUrl;
        public const string Get = $"{BaseUrl}/{{id:guid}}";
        public const string Update = $"{BaseUrl}/{{id:guid}}";
        public const string Delete = $"{BaseUrl}/{{id:guid}}";
        public const string GetAll = BaseUrl;
        public const string AssignPoliciesToRole =  $"{BaseUrl}/{{id:guid}}/assign";
    }
    
    public static class Auth
    {
        public const string BaseUrl = $"{ApiBase}/auth";

        public const string SignIn = $"{BaseUrl}/signin";
        public const string SignUp = $"{BaseUrl}/signup";
    }
}