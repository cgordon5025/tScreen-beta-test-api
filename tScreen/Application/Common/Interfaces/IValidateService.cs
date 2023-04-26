using System.Collections.Generic;
using Application.Common.Models;
using FluentValidation;
using HotChocolate;
using HotChocolate.Resolvers;

namespace Application.Common.Interfaces
{
    public interface IValidateService
    {
        /// <summary>
        /// Validates input model data
        /// </summary>
        /// <param name="input">Company data</param>
        /// <param name="validator">Fluent validator instance</param>
        /// <param name="context"></param>
        /// <typeparam name="TInput">Input data</typeparam>
        /// <exception cref="GraphQLException">Thrown when validate fails</exception>
        public IList<ErrorDetail> ValidateModel<TInput>(TInput input, AbstractValidator<TInput> validator);

        /// <summary>
        /// Validates input model data and maps to intended object type
        /// </summary>
        /// <param name="input">Company data</param>
        /// <param name="validator">Fluent validator instance</param>
        /// <typeparam name="TInput">Input data</typeparam>
        /// <typeparam name="TEntity">Entity the input data shall be mapped to after successful validation</typeparam>
        /// <exception cref="GraphQLException">Thrown when validate fails</exception>
        public TEntity ValidateModelAndMapData<TInput, TEntity>(TInput input, AbstractValidator<TInput> validator);
    }
}