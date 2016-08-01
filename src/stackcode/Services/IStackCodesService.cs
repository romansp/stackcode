using System.Collections;
using System.Threading.Tasks;

namespace stackcode.Services
{
    public interface IStackCodesService
    {
        Task<string> GetStackCodesStringAsync();
    }
}