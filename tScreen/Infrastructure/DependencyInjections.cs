using System.IO;
using Application.Common.Interfaces;
using Core;
using Core.Settings;
using Infrastructure.Services;
using IronPdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureValidateSettings<EmailSettings>(configuration.GetSection("Email"), out var emailSettings);

            var templateRoot = Path.Combine(new[]
            {
                Directory.GetCurrentDirectory(),
                "Templates",
                "Emails"
            });


            services
                .AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName)
                .AddSmtpSender(emailSettings.Smtp.Hostname, emailSettings.Smtp.Port)
                .AddRazorRenderer(templateRoot);

            services.AddSingleton<ITemplateService, TemplateService>();
            services.AddSingleton<IApplicationEnvironment, ApplicationEnvironment>();
            services.AddSingleton<IDateTime, DateTimeService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IPdfService<ChromePdfRenderer>, PdfService>();
            services.AddSingleton<IQueueService, QueueService>();
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IBlobStorage, BlobStorageService>();
            services.AddTransient<IValidateService, ValidateService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<StudentService>();

            return services;
        }
    }
}