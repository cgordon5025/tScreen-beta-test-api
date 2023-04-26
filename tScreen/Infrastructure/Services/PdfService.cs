using System;
using System.IO;
using System.Reflection;
using Application.Common.Interfaces;
using Core;
using IronPdf;
using IronPdf.Engines.Chrome;
using IronPdf.Rendering;

namespace Infrastructure.Services;

public class PdfService : IPdfService<ChromePdfRenderer>
{
    // ReSharper disable once MemberCanBePrivate.Global
    public ChromePdfRenderer Renderer { get; }

    private readonly ChromePdfRenderOptions _renderOptions = new()
    {
        PaperSize = PdfPaperSize.A4,
        CssMediaType = PdfCssMediaType.Screen,
        FitToPaperMode = FitToPaperModes.Automatic,
        PaperOrientation = PdfPaperOrientation.Portrait,
        MarginTop = 1,
        MarginLeft = 1,
        MarginRight = 1,
        MarginBottom = 1,
        // ViewPortWidth = 1024,
        Timeout = 180,
        RenderDelay = 6000,
    };

    public PdfService(IApplicationEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            IronPdf.Logging.Logger.EnableDebugging = true;
            IronPdf.Logging.Logger.LogFilePath = "IronPdfLogs/Default.log";
            IronPdf.Logging.Logger.LoggingMode = IronPdf.Logging.Logger.LoggingModes.All;

            if (environment.IsContainerHosted() && OperatingSystem.IsLinux())
            {
                IronPdf.Installation.LinuxAndDockerDependenciesAutoConfig = false;
                IronPdf.Installation.ChromeGpuMode = IronPdf.Engines.Chrome.ChromeGpuModes.Disabled;
            }
        }

        Renderer = new IronPdf.ChromePdfRenderer()
        {
            RenderingOptions = _renderOptions
        };
    }

    public void SaveDocument(string name, string contents)
    {
        if (Renderer is null)
            throw new NullReferenceException("PDF Renderer is not initialized");

        var applicationEnvironment = new ApplicationEnvironment();
        if (applicationEnvironment.IsKnownUnprotectedEnvironment())
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var fileName = Path.GetFileName(name).Replace(".pdf", ".html");
            var filePath = Path.Combine(rootPath, "Templates", "Reports", "Debug", fileName);
            var findPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(findPath))
            {
                Directory.CreateDirectory(findPath!);
            }
            File.WriteAllText(filePath, contents);
        }

        using var pdfDocument = Renderer?.RenderHtmlAsPdf(contents);
        pdfDocument?.SaveAs(name);
    }
}