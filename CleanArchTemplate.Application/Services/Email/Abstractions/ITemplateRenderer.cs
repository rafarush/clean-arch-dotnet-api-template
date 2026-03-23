namespace CleanArchTemplate.Application.Services.Email.Abstractions;

public interface ITemplateRenderer
{
    Task<string> RenderAsync<TModel>(string templateKey, TModel model);
}