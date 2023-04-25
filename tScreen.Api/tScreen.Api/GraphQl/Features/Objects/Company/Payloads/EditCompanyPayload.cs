using Application.Features.Admin.Models;

namespace GraphQl.GraphQl.Features.Objects.Company.Payloads
{
    public class EditCompanyPayload
    {
        public CompanyDTO Company { get; }

        public EditCompanyPayload(CompanyDTO company)
        {
            Company = company;
        }
    }
}