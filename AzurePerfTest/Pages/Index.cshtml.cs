using AzurePerfTest.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Linq;

namespace AzurePerfTest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly StackOverflow2010Context _ctx;
        private readonly Stopwatch _sw = new Stopwatch();
        private User[] _users;
        private long _elapsed;
        public long Elapsed => _elapsed;
        public User[] Users => _users;


        public IndexModel(ILogger<IndexModel> logger, StackOverflow2010Context ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public void OnGet()
        {
            _sw.Start();

            HttpContext.Request.Query.TryGetValue("name", out StringValues names);
            var userName = names.FirstOrDefault();

            _users = _ctx.Users.Where(x => string.IsNullOrEmpty(userName) || x.DisplayName.Contains(userName)).Take(25).ToArray();
            _sw.Stop();
            var elapsed = _sw.ElapsedMilliseconds;
            _logger.LogInformation($"PERF:{nameof(IndexModel)}.${nameof(OnGet)}:{_sw.ElapsedMilliseconds}", _sw.ElapsedMilliseconds);
            _elapsed = elapsed;
        }
    }
}
