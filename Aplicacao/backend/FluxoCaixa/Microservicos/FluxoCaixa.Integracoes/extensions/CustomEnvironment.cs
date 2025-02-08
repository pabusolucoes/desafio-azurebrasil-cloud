using Microsoft.Extensions.Hosting;

namespace FluxoCaixa.Integracoes.Extensions
{
    public interface ICustomEnvironment
    {
        bool IsDevelopment();
        bool IsProduction();
        bool IsStaging();
        bool IsLocal();
        bool IsEnvironment(string environmentName);
    }
    public class CustomEnvironment : ICustomEnvironment
    {
        private readonly IHostEnvironment _hostEnvironment;

        public CustomEnvironment(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public bool IsDevelopment() => _hostEnvironment.IsDevelopment();
        public bool IsProduction() => _hostEnvironment.IsProduction();
        public bool IsStaging() => _hostEnvironment.IsStaging();
        
        // ✅ Ambiente personalizado
        public bool IsLocal() => _hostEnvironment.IsEnvironment("Local");

        public bool IsEnvironment(string environmentName) => _hostEnvironment.IsEnvironment(environmentName);
    }
}
