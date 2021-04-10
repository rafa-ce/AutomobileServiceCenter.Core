using ASC.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private IOptions<ApplicationSettings> _settings;

        public DashboardController(ILogger<DashboardController>  logger, IOptions<ApplicationSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult TestException()
        {
            var i = 0;
            // Should through Divide by zero error
            var j = 1 / i;
            return View();
        }
    }
}
