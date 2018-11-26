using ChatMap.Infrastructure;
using ChatMap.Models;
using ChatMap.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public Task<IReadOnlyCollection<PersonModel>> GetPersons(string sheetId, CancellationToken cancellationToken)
        {
            return _chatRepository.GetPersons(sheetId, cancellationToken);
        }

        public async Task<IReadOnlyCollection<PersonModel>> AdjustPersons(string sheetId, CancellationToken cancellationToken)
        {
            var persons = await GetPersons(sheetId, cancellationToken);

            foreach(var person in persons)
            {
                if (!person.UserId.HasValue)
                {
                    person.UserId = Guid.NewGuid();
                }
            }

            var didUpdate = await _chatRepository.UpdatePersons(sheetId, persons, cancellationToken);

            if (didUpdate)
                return await GetPersons(sheetId, cancellationToken);

            return persons;
        }
    }
}
