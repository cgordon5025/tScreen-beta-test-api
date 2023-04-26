using System;
using System.Reflection;
using Application.Common.Interfaces;
using Application.Mappings;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            var autoMapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile<AdminMapProfile>();
            });

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine("Assembly: " + assembly);
                foreach (var aType in assembly.GetTypes())
                {
                    if (aType.IsClass && !aType.IsAbstract && aType.IsSubclassOf(typeof(Profile)))
                        Console.WriteLine("   + Assembly Type: " + aType);
                }
            }

            services.AddSingleton(autoMapperConfig.CreateMapper());
            services.AddAutoMapper(executingAssembly);
            services.AddMediatR(executingAssembly);

            return services;
        }
    }
}