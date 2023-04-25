using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core;

/// <summary>
/// Derived from https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
/// </summary>
public class Enumeration : IComparable
{
    // ReSharper disable once MemberCanBePrivate.Global
    public string Name { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public int Id { get; }

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        // ReSharper disable once CheckForReferenceEqualityInstead.1
        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }
    
    public override int GetHashCode() => Id.GetHashCode();

    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
}