namespace CleanArchTemplate.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";
    
    public static class Users
    {
        public const string BaseUrl = $"{ApiBase}/users";
        
        public const string Create = BaseUrl;
        public const string Get = $"{BaseUrl}/{{id:guid}}";
        public const string GetRouteName = "GetUser";
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
        public const string GetRouteName = "GetPolicy";
        public const string Update = $"{BaseUrl}/{{id:guid}}";
        public const string Delete = $"{BaseUrl}/{{id:guid}}";
        public const string GetAll = BaseUrl;
    }

    public static class Roles
    {
        public const string BaseUrl = $"{ApiBase}/roles";
        
        public const string Create = BaseUrl;
        public const string Get = $"{BaseUrl}/{{id:guid}}";
        public const string GetRouteName = "GetRole";
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
        public const string Refresh = $"{BaseUrl}/refresh/{{token}}";
        public const string VerifyEmail = $"{BaseUrl}/verify-email/{{link}}";
        public const string ForgotPassword = $"{BaseUrl}/forgot-password";
        public const string ResetPassword = $"{BaseUrl}/reset-password";
        public const string OAuthGoogle = $"{BaseUrl}/oauth/google";
        public const string OAuthGitHub = $"{BaseUrl}/oauth/github";
        public const string OAuthGoogleCallback = $"{BaseUrl}/oauth/google/callback";
        public const string OAuthGitHubCallback = $"{BaseUrl}/oauth/github/callback";
        public const string OAuthLinkAccount = $"{BaseUrl}/oauth/link-account";
    }
}