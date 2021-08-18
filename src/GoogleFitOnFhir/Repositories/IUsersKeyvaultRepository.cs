using System.Threading.Tasks;
using Microsoft.Azure.KeyVault.Models;

namespace GoogleFitOnFhir.Repositories
{
    public interface IUsersKeyvaultRepository
    {
        void Upsert(string secretName, string value);

        Task<string> GetByName(string secretName);
    }
}