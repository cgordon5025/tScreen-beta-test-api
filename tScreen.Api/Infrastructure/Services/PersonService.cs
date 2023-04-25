using System;
using Application.Common.Interfaces;

namespace Infrastructure.Services
{
    public class PersonService : IPersonService
    {
        public Guid CurrentSignedInPersonId()
        {
            return Guid.Parse("34ed423e-f36b-1410-8a08-00f2e42120c8");
        }
    }
}