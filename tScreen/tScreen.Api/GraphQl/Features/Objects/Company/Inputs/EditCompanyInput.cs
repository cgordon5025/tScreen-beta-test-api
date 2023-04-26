using System;

namespace GraphQl.GraphQl.Features.Objects.Company.Inputs
{
    /// <summary>
    /// Edit/update company record
    /// </summary>
    /// <param name="Id">Company ID</param>
    /// <param name="Type">E.g., School, Hospital, PrivatePractice</param>
    /// <param name="Name">Company name</param>
    /// <param name="Description">Company description [optional]</param>
    // ReSharper disable once ClassNeverInstantiated.Global
    public record EditCompanyInput(Guid Id, CompanyType Type, string Name, string Description);
}