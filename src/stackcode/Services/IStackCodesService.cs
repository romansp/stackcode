using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using stackcode.Models;

namespace stackcode.Services
{
    public interface IStackCodesService
    {
        Task<IEnumerable<StackCode>> GetStackCodesAsync();
    }
}