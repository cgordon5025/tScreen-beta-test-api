using System;
using System.Reflection;
using Application.Mappings;
using AutoMapper;
using GraphQl.GraphQl.Diagnostics;
using GraphQl.GraphQl.Features.Objects.Adventure;
using GraphQl.GraphQl.Features.Objects.Answer;
using GraphQl.GraphQl.Features.Objects.Avatar;
using GraphQl.GraphQl.Features.Objects.Company;
using GraphQl.GraphQl.Features.Objects.CustomField;
using GraphQl.GraphQl.Features.Objects.CustomField.Results;
using GraphQl.GraphQl.Features.Objects.Location;
using GraphQl.GraphQl.Features.Objects.Me;
using GraphQl.GraphQl.Features.Objects.Person;
using GraphQl.GraphQl.Features.Objects.Question;
using GraphQl.GraphQl.Features.Objects.QuestionContingent;
using GraphQl.GraphQl.Features.Objects.Scene;
using GraphQl.GraphQl.Features.Objects.Session;
using GraphQl.GraphQl.Features.Objects.Session.Results;
using GraphQl.GraphQl.Features.Objects.Student;
using GraphQl.GraphQl.Features.Objects.Student.Results;
using GraphQl.GraphQl.Features.Objects.StudentCustomField;
using GraphQl.GraphQl.Features.Objects.WorkList;
using GraphQl.GraphQl.Interceptors;
using GraphQl.GraphQl.Models;
using GraphQl.Mappings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CompanyType = GraphQl.GraphQl.Features.Objects.Company.CompanyType;
using LocationType = GraphQl.GraphQl.Features.Objects.Location.LocationType;

namespace GraphQl.GraphQl.Features
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddGraphQlServices(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddHttpRequestInterceptor<HttpRequestInterceptor>()
                .AddQueryType(e => e.Name("Query"))
                .AddTypeExtension<MeQuery>()
                .AddTypeExtension<CompanyQuery>()
                .AddTypeExtension<PersonQuery>()
                .AddTypeExtension<StudentQuery>()
                .AddTypeExtension<AvatarQuery>()
                .AddTypeExtension<CustomFieldQuery>()
                .AddTypeExtension<AdventureQuery>()
                .AddTypeExtension<QuestionQuery>()
                .AddTypeExtension<QuestionContingentQuery>()
                .AddTypeExtension<LocationQuery>()
                .AddTypeExtension<AnswerQuery>()
                .AddTypeExtension<SceneQuery>()
                .AddTypeExtension<SessionQuery>()
                .AddTypeExtension<WorkListQuery>()
                .AddMutationType(e => e.Name("Mutation"))
                .AddTypeExtension<MeMutation>()
                .AddTypeExtension<CompanyMutation>()
                .AddTypeExtension<StudentMutation>()
                .AddTypeExtension<AvatarMutation>()
                .AddTypeExtension<CustomFieldMutation>()
                .AddTypeExtension<WorkListMutation>()
                .AddTypeExtension<AnswerMutation>()
                .AddTypeExtension<SessionMutation>()
                .AddSorting()
                .AddProjections()
                .AddType<AuthenticateType>()
                .AddType<AdventureType>()
                .AddType<CompanyType>()
                .AddType<QuestionType>()
                .AddType<QuestionContingentType>()
                .AddType<PersonType>()
                .AddType<StudentType>()
                // .AddType<StudentCustomFieldType>()
                .AddType<SceneType>()
                .AddType<LocationType>()
                .AddType<SessionType>()
                .AddEnumType<SessionStatus>(descriptor =>
                {
                    descriptor.Value(SessionStatus.Abandoned).Name(SessionStatus.Abandoned.Name);
                    descriptor.Value(SessionStatus.Complete).Name(SessionStatus.Complete.Name);
                    descriptor.Value(SessionStatus.Incomplete).Name(SessionStatus.Incomplete.Name);
                    descriptor.Value(SessionStatus.Pending).Name(SessionStatus.Pending.Name);
                    descriptor.Value(SessionStatus.TestingSnakeCase).Name(SessionStatus.TestingSnakeCase.Name);
                })
                .AddType<AnswerType>()
                .AddType<WorkListType>()
                .AddEnumType<WorkListStatus>(descriptor =>
                {
                    descriptor.Value(WorkListStatus.Reviewed).Name(WorkListStatus.Reviewed.Name);
                    descriptor.Value(WorkListStatus.Unread).Name(WorkListStatus.Unread.Name);
                })
                .AddType<ValidationError>()
                .AddStudentResultTypes()
                .AddSessionResultTypes()
                .AddWorkListResultTypes()
                .AddCustomFieldResultTypes()
                .AddDiagnosticEventListener<ApplicationInsightsDiagnosticEventListener>();

            return services;
        }
    }
}