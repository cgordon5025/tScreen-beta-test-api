using System.IO;
using Application.Common.Interfaces;
using Core;

namespace Application.Common;

public abstract class TemplateEnumeration : Enumeration
{
    public string BasePath { get; }

    protected TemplateEnumeration(int id, string name, string[] paths) : base(id, name)
    {
        BasePath = Path.Combine(paths);
    }
}

public class ReportTemplate : TemplateEnumeration, IRenderTemplate
{
    public static readonly ReportTemplate MainTemplate = new ReportTemplate(1, "ReportTemplate.cshtml");
    public static readonly ReportTemplate Page2 = new ReportTemplate(2, "ReportPage2.cshtml");
    public static readonly ReportTemplate Page3 = new ReportTemplate(2, "ReportPage2.cshtml");

    private ReportTemplate(int id, string name) : base(id, name,
        new[]
        {
            "Templates",
            "Reports",
            "SessionReport"
        }
    )
    { }

    public class Partial : TemplateEnumeration, IRenderTemplatePartial
    {
        public static readonly Partial Header = new Partial(1, "_header.cshtml");
        public static readonly Partial Footer = new Partial(2, "_footer.cshtml");

        private Partial(int id, string name) : base(id, name,
            new[]
            {
                "Templates",
                "Reports",
                "Partials"
            }
        )
        { }
    }

    public class Styles : TemplateEnumeration, IRenderTemplateStyle
    {
        public static readonly Styles ReportTemplate = new Styles(1, "ReportTemplate.css");

        private Styles(int id, string name) : base(id, name, new[]
        {
            "Templates",
            "Reports",
            "Styles"
        })
        { }
    }
}