using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Passaword.Storage
{
    public interface ISecretStore
    {
        Task<string> CreateAsync(Secret secret);
        Task<Secret> GetAsync(string id);

        Task<List<Secret>> FindAsync(Expression<Func<Secret, bool>> predicate);
        Task UpdateAsync(Secret secret);
        Task DeleteAsync(string id);
    }
}
