using System;

namespace Application.Common.Interfaces
{
    public interface IPersonService
    { 
        Guid CurrentSignedInPersonId();
    }
}