
using Microsoft.Extensions.DependencyInjection;

namespace Passaword.Configuration
{
    public interface IPassawordBuilder
    {
        IServiceCollection Services { get; set; }
    }

    public class PassawordBuilder : IPassawordBuilder {
        public IServiceCollection Services { get; set; }
    }
}
