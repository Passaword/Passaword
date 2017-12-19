using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Passaword.KeyGen;

namespace Passaword.Storage
{
    public class InMemorySecretStore : ISecretStore
    {
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogger<InMemorySecretStore> _logger;
        private readonly ConcurrentDictionary<string, Secret> _store = new ConcurrentDictionary<string, Secret>();

        public InMemorySecretStore(IKeyGenerator keyGenerator, ILogger<InMemorySecretStore> logger)
        {
            _keyGenerator = keyGenerator;
            _logger = logger;
        }

        public Task<string> CreateAsync(Secret secret)
        {
            var id = _keyGenerator.GenerateKey();
            _logger.LogDebug($"Creating a secret - id {id} given");
            secret.Id = id;
            _store.TryAdd(id, secret);
            return Task.FromResult(id);
        }

        public Task<List<Secret>> FindAsync(Expression<Func<Secret, bool>> predicate)
        {
            _logger.LogDebug($"Finding secrets with predicate {predicate}");
            var list = _store.Values.Where(predicate.Compile()).ToList();
            return Task.FromResult(list);
        }

        public Task<Secret> GetAsync(string id)
        {
            if (_store.ContainsKey(id))
            {
                _logger.LogDebug($"Found secret with id {id}");
                return Task.FromResult(_store[id]);
            }
            _logger.LogDebug($"Could not find secret with id {id}");
            return null;
        }

        public Task UpdateAsync(Secret secret)
        {
            if (_store.ContainsKey(secret.Id))
            {
                _logger.LogDebug($"Updating secret with id {secret.Id}");
                _store[secret.Id] = secret;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            if (_store.ContainsKey(id))
            {
                _logger.LogDebug($"Deleting secret with id {id}");
                Secret secret;
                _store.TryRemove(id, out secret);
            }
            return Task.CompletedTask;
        }
    }
}