using Application.Features.Admin.Models;

namespace GraphQl.GraphQl.Features.Objects.CustomField.Payloads
{
    public class AddCustomElementPayload
    {
        public CustomFieldDTO CustomField { get; set; }

        public AddCustomElementPayload(CustomFieldDTO customField)
        {
            CustomField = customField;
        }
    }
}