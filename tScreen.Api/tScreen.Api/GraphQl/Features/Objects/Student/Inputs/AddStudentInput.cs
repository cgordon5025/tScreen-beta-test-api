using System;
using System.Collections.Generic;

namespace GraphQl.GraphQl.Features.Objects.Student.Inputs
{
    public record AddStudentInput(
        Guid LocationId, 
        string FirstName,
        string MiddleName, 
        string LastName, 
        DateTime Dob, 
        string Email, 
        int GradeLevel,
        IEnumerable<CustomFieldInput> CustomFields);

    public record CustomFieldInput(Guid Id, string Value);
}