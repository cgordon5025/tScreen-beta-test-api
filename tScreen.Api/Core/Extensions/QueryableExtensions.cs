#nullable enable
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Extensions;

public static class QueryableExtensions
{
    private static readonly ApplicationEnvironment _applicationEnivronment = new ApplicationEnvironment();
    /// <summary>
    /// Tag query call site safely if environment is "Development," or "Testing."
    /// If environment is a protected environment type or the <see cref="use"/> parameter
    /// is "false" then do not tag query with callsite.
    /// </summary>
    /// <param name="source">The source query.</param>
    /// <param name="use">If query should be tagged with callsite.</param>
    /// <param name="filePath">The file name where the method was called.</param>
    /// <param name="lineNumber">The file line number where the method was called.</param>
    /// <typeparam name="T">The type of entity being queried.</typeparam>
    /// <returns>A new query annotated with the callsite</returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source" />
    /// </exception>
    public static IQueryable<T> TagWithCallSiteSafely<T>(
        this IQueryable<T> source, 
        bool use = true,
        [NotParameterized] [CallerFilePath] string? filePath = null,
        [NotParameterized] [CallerLineNumber] int lineNumber = 0)
    {
        if (filePath is null) throw new ArgumentNullException(nameof(filePath));
        
        return !(!use && !_applicationEnivronment.IsDevelopment() && !_applicationEnivronment.IsTesting())
            ? source.TagWithCallSite(
                // ReSharper disable once ExplicitCallerInfoArgument
                filePath, 
                // ReSharper disable once ExplicitCallerInfoArgument
                lineNumber)
            : source;
    }
}