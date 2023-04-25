using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface ITemplateService
{
    string RootPath { get; }
    Task<string?> CompileRenderAsync(IRenderTemplate template, object model);
    Task<string?> CompileRenderPartialAsync(IRenderTemplatePartial template, object mode);

    public string GetFullyQualifiedPath(IRenderTemplate template, bool includeFile = false);
    public string GetFullyQualifiedPath(IRenderTemplateStyle template, bool includeFile = false);
    public string GetFullyQualifiedPath(IRenderTemplatePartial template, bool includeFile = false);
}