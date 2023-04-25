using Application.Features.Admin.Models;

namespace GraphQl.GraphQl.Features.Objects.Company.Payloads
{
    public class AddCompanyPayload
    {
        public CompanyDTO Company { get; }

        public AddCompanyPayload(CompanyDTO company)
        {
            Company = company;
        }
    }
}