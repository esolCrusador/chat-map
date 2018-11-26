using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatMap.Models;

namespace ChatMap.Services.Contracts
{
    public interface IChatService
    {
        Task<IReadOnlyCollection<PersonModel>> GetPersons(string sheetId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<PersonModel>> AdjustPersons(string sheetId, CancellationToken cancellationToken);
    }
}
