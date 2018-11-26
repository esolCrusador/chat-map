using ChatMap.Entities;
using ChatMap.Extensions;
using ChatMap.Infrastructure;
using ChatMap.Models;
using ChatMap.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private static readonly Regex ChildrenRegex = new Regex(@"(^|,) *(?<name>\w+( \w+)*) *\(?(?<birthDate>\d{1,2}\.\d{1,2}\.\d{4})?\)? *", RegexOptions.Compiled);
        private const string PersonsTableName = "Form Responses 1";

        private readonly GoogleApiClient _googleApiService;
        private readonly DynamicMapper _dynamicMapper;
        private readonly GoogleSheetShemaProvider _schemaProvider;

        public ChatRepository(GoogleApiClient googleApiService, DynamicMapper dynamicMapper, GoogleSheetShemaProvider schemaProvider)
        {
            _googleApiService = googleApiService;
            _dynamicMapper = dynamicMapper;
            _schemaProvider = schemaProvider;
        }

        private async Task<IReadOnlyCollection<PersonEntity>> GetPersonEntities(string sheetId, CancellationToken cancellationToken)
        {
            IReadOnlyCollection<System.Data.DataTable> sheetTables = await _googleApiService.GetSheetTables(sheetId, cancellationToken);
            _schemaProvider[sheetId].UpdateColumnsSchema(sheetTables);

            List<PersonEntity> personEntities = _dynamicMapper.Map<PersonEntity>(sheetTables.First(t => t.TableName == PersonsTableName)).ToList();

            return personEntities;
        }

        public async Task<IReadOnlyCollection<PersonModel>> GetPersons(string sheetId, CancellationToken cancellationToken)
        {
            var personEntitites = await GetPersonEntities(sheetId, cancellationToken);

            List<PersonModel> persons = personEntitites.Select(personEntity =>
            {
                try
                {
                    PersonModel person = new PersonModel()
                    {
                        UserId = personEntity.UserId == null ? (Guid?)null : new Guid(personEntity.UserId),
                        FullName = personEntity.FullName,
                        Children = ChildrenRegex.Matches(personEntity.Children)
                        .Select(m =>
                            new ChildModel
                            {
                                ChildName = m.Groups["name"].Value,
                                BirthDate = m.Groups["birthDate"].Success ? DateTime.ParseExact(m.Groups["birthDate"].Value, "d.M.yyyy", CultureInfo.InvariantCulture) : (DateTime?)null
                            }).ToList(),
                        Address = personEntity.Location,
                        Instagram = personEntity.Instagram,
                        EmailAddress = personEntity.EmailAddress,
                        PhoneNumber = personEntity.TelephoneNumber,
                        ChangeDate = string.IsNullOrEmpty(personEntity.Timestamp) ? (DateTime?)null : DateTime.ParseExact(personEntity.Timestamp, "M/d/yyyy H:m:s", CultureInfo.InvariantCulture),
                        Id = personEntity.ExternalId,
                    };

                    return person;
                }
                catch
                {
                    return null;
                }

            }).ToList();

            return persons;
        }

        struct ChangeHandler<TEntity> where TEntity: class
        {
            private readonly int _columnsCount;
            private readonly DynamicMapper _dynamicMapper;
            private readonly GoogleSheetTableSchemaProvider _tableShemaProvider;

            public ChangeHandler(string sheetId, DynamicMapper dynamicMapper, GoogleSheetShemaProvider sheetShemaProvider)
            {
                _dynamicMapper = dynamicMapper;
                _tableShemaProvider = sheetShemaProvider[sheetId];

                _columnsCount = _tableShemaProvider.GetColumnsCount(PersonsTableName);

                Existing = null;
                Updated = null;
                ChangesMap = null;
            }

            public TEntity Existing { get; private set; }
            public TEntity Updated { get; private set; }
            public string[] ChangesMap { get; private set; }

            public void SetCurrent(TEntity existing, TEntity updated)
            {
                Existing = existing;
                Updated = updated;
                ChangesMap = new string[_columnsCount];
                for (int i = 0; i < ChangesMap.Length; i++)
                    ChangesMap[i] = GoogleApiClient.Unchanged;
            }

            private int GetIndex(Expression<Func<TEntity, string>> property)
            {
                return _tableShemaProvider.GetColumnIndex(PersonsTableName, _dynamicMapper.GetColumnName(property));
            }

            public void UpdateIfChanged(Expression<Func<TEntity, string>> property)
            {
                var getProperty = _dynamicMapper.GetGetPropertyDelegate(property);
                ChangesMap[GetIndex(property)] = getProperty(Existing) == getProperty(Updated) ? GoogleApiClient.Unchanged : getProperty(Updated);
            }
        }

        public async Task<bool> UpdatePersons(string sheetId, IReadOnlyCollection<PersonModel> persons, CancellationToken cancellationToken)
        {
            var personExistingEntites = await GetPersonEntities(sheetId, cancellationToken);

            var personNewEntities = persons.Select(person => new PersonEntity
            {
                UserId = person.UserId.HasValue ? person.UserId.Value.ToString() : null,
                FullName = person.FullName,
                Children = string.Join(", ", person.Children.Select(c => $"{c.ChildName} ({(c.BirthDate ?? DateTime.Today).ToString("dd.MM.yyyy")})")),
                Location = person.Address,
                Instagram = person.Instagram,
                EmailAddress = person.EmailAddress,
                TelephoneNumber = person.PhoneNumber,
                ExternalId = person.Id
            }).ToList();

            var changes = new Dictionary<int, string[]>();

            var entititesPairs = personExistingEntites.JoinTo(personNewEntities)
                .LeftKey(existing => existing.ExternalId)
                .RightKey(updated => updated.ExternalId)
                .Join((existing, updated) => new { Existing = existing, Updated = updated })
                .Where(pair => !pair.Existing.Equals(pair.Updated));

            
            var changesHandler = new ChangeHandler<PersonEntity>(sheetId, _dynamicMapper, _schemaProvider);

            foreach (var pair in entititesPairs)
            {
                var existing = pair.Existing;
                var updated = pair.Updated;
                updated.Timestamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                changesHandler.SetCurrent(existing, updated);

                changesHandler.UpdateIfChanged(p => p.UserId);
                changesHandler.UpdateIfChanged(p => p.FullName);
                changesHandler.UpdateIfChanged(p => p.Children);
                changesHandler.UpdateIfChanged(p => p.Location);
                changesHandler.UpdateIfChanged(p => p.Instagram);
                changesHandler.UpdateIfChanged(p => p.EmailAddress);
                changesHandler.UpdateIfChanged(p => p.TelephoneNumber);
                changesHandler.UpdateIfChanged(p => p.Timestamp);


                changes.Add(existing.ExternalId, changesHandler.ChangesMap);
            }

            if(changes.Count != 0)
            {
                await _googleApiService.UpdateSheetTable(sheetId, PersonsTableName, changes, cancellationToken);
                return true;
            }

            return false;
        }
    }
}
