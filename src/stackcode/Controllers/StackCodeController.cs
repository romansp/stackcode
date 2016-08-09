using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using stackcode.Models.Twitter;
using stackcode.Services;
using stackcode.Views.StackCode;

namespace stackcode.Controllers
{
    public class StackCodeController : Controller
    {
        private readonly IStackCodesService _stackCodesService;

        public StackCodeController(IStackCodesService stackCodesService)
        {
            _stackCodesService = stackCodesService;
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            var stackCodes = await _stackCodesService.GetStackCodesStringAsync();
            return View(stackCodes);
        }
    }
}
