#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using HotChocolate;
using HotChocolate.Resolvers;

namespace Infrastructure.Services
{
    public class ValidateService : IValidateService
    {
        private readonly IMapper _mapper;

        public ValidateService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IList<ErrorDetail> ValidateModel<TInput>(TInput input, AbstractValidator<TInput> validator)
        {
            return _mapGraphQLErrorsAndRaise(validator.Validate(input));
        }

        public void ValidateModelAndThrow<TInput>(TInput input, AbstractValidator<TInput> validator)
        {
            _mapGraphQLErrorsAndThrow(validator.Validate(input));
        }

        public TEntity ValidateModelAndMapData<TInput, TEntity>(TInput input, AbstractValidator<TInput> validator)
        {
            _mapGraphQLErrorsAndThrow(validator.Validate(input));
            return _mapper.Map<TEntity>(input);
        }
        
        private List<ErrorDetail> _mapGraphQLErrorsAndRaise(ValidationResult? validationResult)
        {
            if (validationResult == null || validationResult.IsValid)
                return new List<ErrorDetail>();
            
            // Flatten error collection if a field has more than one error associated with it.
            var errorCollection = new Dictionary<string, List<ValidationFailure>>();
            
            foreach (var error in validationResult.Errors)
            {
                var normalizedPropertyName = error.PropertyName.EndsWith("]")
                    ? Regex.Replace(error.PropertyName, @"\[[\d]+\]", "")
                    : error.PropertyName;

                var newPropertyName = JsonNamingPolicy.CamelCase.ConvertName(normalizedPropertyName);
                
                if (!errorCollection.ContainsKey(newPropertyName))
                {
                    errorCollection.Add(newPropertyName, new List<ValidationFailure>{ error });
                    continue;
                }
                
                errorCollection[newPropertyName].Add(error);
            }

            var errorDetails = new List<ErrorDetail>();
            foreach (var (key, results) in errorCollection)
            {
                errorDetails.Add(new ErrorDetail
                {
                    FieldName = key,
                    Messages = results.Select(e => e.ErrorMessage)
                });
            }

            return errorDetails;
        }

        private static void _mapGraphQLErrorsAndThrow(ValidationResult? validationResult)
        {
            if (validationResult == null || validationResult.IsValid)
                return;
            
            // Flatten error collection if a field has more than one error associated with it.
            var errorCollection = new Dictionary<string, List<ValidationFailure>>();
            
            foreach (var error in validationResult.Errors)
            {
                var normalizedField = error.PropertyName.EndsWith("]")
                    ? Regex.Replace(error.PropertyName, @"\[[\d]+\]", "")
                    : error.PropertyName;

                if (!errorCollection.ContainsKey(normalizedField))
                {
                    errorCollection.Add(normalizedField, new List<ValidationFailure>{ error });
                    continue;
                }
                
                errorCollection[normalizedField].Add(error);
            }

            var graphQlErrorsCollections = new List<IError>();
            
            foreach (var (key, value) in errorCollection)
            {
                var firstOfGroup = value.First();
                
                var errorBuilder = ErrorBuilder.New()
                    .SetMessage(firstOfGroup.ErrorMessage)
                    .SetCode(firstOfGroup.ErrorCode);

                if (value.Any())
                {
                    errorBuilder.SetExtension("fields", value
                        .Select(x => x.PropertyName).ToArray());
                    
                    errorBuilder.SetExtension("values", value
                        .Select(x => x.AttemptedValue).ToArray());   
                }
                else
                {
                    errorBuilder
                        .SetExtension("field", JsonNamingPolicy.CamelCase.ConvertName(key))
                        .SetExtension("value", firstOfGroup.AttemptedValue);
                }
                
                graphQlErrorsCollections.Add(errorBuilder.Build());

            }
            
            // Add top level error message to control status code handling, routing etc...    
            graphQlErrorsCollections.Insert(0, ErrorBuilder.New()
                .SetMessage("Input data provided by the user is invalid")
                .SetCode("User.Data.Invalid")
                .Build());
                
            throw new GraphQLException(graphQlErrorsCollections);
        }
    }
}