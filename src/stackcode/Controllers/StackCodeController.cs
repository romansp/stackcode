﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<string> Index()
        {
            var stackCodes = await _stackCodesService.GetStackCodesStringAsync();
            return stackCodes;
        }
    }
}
