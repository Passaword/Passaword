using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Passaword.KeyGen;

namespace Passaword.Storage.Sql
{
    public class SqlSecretStore : ISqlSecretStore
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogger<SqlSecretStore> _logger;

        public SqlSecretStore(IServiceProvider serviceProvider, IKeyGenerator keyGenerator, ILogger<SqlSecretStore> logger)
        {
            _serviceProvider = serviceProvider;
            _keyGenerator = keyGenerator;
            _logger = logger;
        }

        private PassawordDbContext GetContext()
        {
            return (PassawordDbContext) _serviceProvider.GetService(typeof(PassawordDbContext));
        }

        public async Task<string> CreateAsync(Secret secret)
        {
            var db = GetContext();
            var id = _keyGenerator.GenerateKey();
            _logger.LogDebug($"Creating a secret - id {id} given");

            secret.Id = id;

            db.Secrets.Add(secret);
            await db.SaveChangesAsync();

            return secret.Id;
        }

        public async Task<Secret> GetAsync(string id)
        {
            var db = GetContext();
            var secret = await db.Secrets
                .Include(q => q.SecretProperties)
                .Include(q => q.SecretValidationRules)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (secret == null)
            {
                _logger.LogDebug($"Could not find secret with id {id}");
                return null;
            }
            else
            {
                _logger.LogDebug($"Found secret with id {id}");
                return secret;
            }
        }

        public async Task<List<Secret>> FindAsync(Expression<Func<Secret, bool>> predicate)
        {
            _logger.LogDebug($"Finding secrets with predicate {predicate}");
            var db = GetContext();
            return await 
                db.Secrets
                    .Include(q=>q.SecretProperties)
                    .Include(q=>q.SecretValidationRules)
                    .Where(predicate)
                    .ToListAsync();
        }

        public async Task UpdateAsync(Secret secret)
        {
            var db = GetContext();
            _logger.LogDebug($"Updating secret with id {secret.Id}");

            db.Attach(secret);
            db.Update(secret);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var db = GetContext();
            var secret = await db.Secrets.FirstOrDefaultAsync(q => q.Id == id);
            if (secret != null)
            {
                _logger.LogDebug($"Deleting secret with id {id}");
                db.Secrets.Remove(secret);
                await db.SaveChangesAsync();
            }
        }
    }
}
