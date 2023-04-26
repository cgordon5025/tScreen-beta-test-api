using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICompanyService
    {
        public Task AddCompanyAsync();
        public Task EditCompanyAsync();
        public Task RemoveCompanyAsync();
        public Task ArchiveCompanyAsync();
    }
}