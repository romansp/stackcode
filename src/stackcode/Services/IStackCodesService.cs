using System.Collections;
using System.Threading.Tasks;
using stackcode.Models.Twitter;

namespace stackcode.Services
{
    public interface IStackCodesService
    {
        Task<Status[]> GetStackCodesFromSearchAsync();
        Task<Status[]> GetStackCodesFromTimelineAsync(long? maxId = null);
    }
}