using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using stackcode.Models;

namespace stackcode.Services
{
    public class TwitterStackCodesService : IStackCodesService
    {
        private readonly IMemoryCache _cache;
        private readonly AppSettings _appSettings;

        public TwitterStackCodesService(IOptions<AppSettings> appSettings, IMemoryCache cache)
        {
            _cache = cache;
            _appSettings = appSettings.Value;
        }

        public Task<IEnumerable<StackCode>> GetStackCodesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}