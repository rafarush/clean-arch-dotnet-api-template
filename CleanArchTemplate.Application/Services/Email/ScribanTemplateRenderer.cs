using CleanArchTemplate.Application.Services.Email.Abstractions;
using CleanArchTemplate.Application.Services.Email.Options;
using Microsoft.Extensions.Options;
using Scriban;
using Scriban.Runtime;

namespace CleanArchTemplate.Application.Services.Email;

public class ScribanTemplateRenderer(
    IOptions<ScribanOptions> scribanOptions
    ): ITemplateRenderer
{
    public async Task<string> RenderAsync<TModel>(string templateKey, TModel model)
    {
        var path = Path.Combine(scribanOptions.Value.TemplatesPath, $"{templateKey}.sbnhtml");
        var source = await File.ReadAllTextAsync(path);

        var template = Template.Parse(source);

        if (template.HasErrors)
            throw new InvalidOperationException(
                $"Template '{templateKey}' tiene errores: " +
                string.Join(", ", template.Messages.Select(m => m.Message)));

        var scriptObject = new ScriptObject();
        scriptObject.Import(model, renamer: member => member.Name.ToLowerInvariant());

        var context = new TemplateContext { StrictVariables = true };
        context.PushGlobal(scriptObject);

        return await template.RenderAsync(context);
    }
}