namespace GraphQl.GraphQl.Features.Objects.Company.Inputs
{
    /// <summary>
    /// Create/add company
    /// </summary>
    /// <param name="Type">E.g., School, Hospital, PrivatePractice</param>
    /// <param name="Name">Company name</param>
    /// <param name="Description">Company description [optional]</param>
    // ReSharper disable once ClassNeverInstantiated.Global
    public record AddCompanyInput(
        CompanyType Type, string Name, string Description);

    public enum CompanyType
    {
        School,
        PrivateClinic,
        Hospital
    }
}