using ChatMap.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Services.Contracts
{
    public interface IChatRepository
    {
        Task<IReadOnlyCollection<PersonModel>> GetPersons(string sheetId, CancellationToken cancellationToken);
        Task<bool> UpdatePersons(string sheetId, IReadOnlyCollection<PersonModel> persons, CancellationToken cancellationToken);
    }
}
