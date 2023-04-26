using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using RazorLight;

namespace Infrastructure.Services;

public class TemplateService : ITemplateService
{
    private readonly RazorLightEngine _engine;
    public string RootPath { get; }

    public TemplateService()
    {
        RootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(RootPath)
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string?> CompileRenderAsync(IRenderTemplate template, object model)
    {
        var templatePath = Path.Combine(template.BasePath, template.Name);
        return await _engine.CompileRenderAsync(templatePath, model);
    }

    public async Task<string?> CompileRenderPartialAsync(IRenderTemplatePartial template, object model)
    {
        var templatePath = Path.Combine(template.BasePath, template.Name);
        return await _engine.CompileRenderAsync(templatePath, model);
    }

    /// <summary>
    /// Get template path with root included
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public string GetFullyQualifiedPath(IRenderTemplate template, bool includeFile = false)
    {
        return includeFile
            ? Path.Combine(RootPath, template.BasePath, template.Name)
            : Path.Combine(RootPath, template.BasePath);
    }

    /// <summary>
    /// Get template path with root included
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public string GetFullyQualifiedPath(IRenderTemplatePartial template, bool includeFile = false)
    {
        return includeFile
            ? Path.Combine(RootPath, template.BasePath, template.Name)
            : Path.Combine(RootPath, template.BasePath);
    }

    /// <summary>
    /// Get template path with root included
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public string GetFullyQualifiedPath(IRenderTemplateStyle template, bool includeFile = false)
    {
        return includeFile
            ? Path.Combine(RootPath, template.BasePath, template.Name)
            : Path.Combine(RootPath, template.BasePath);
    }
}