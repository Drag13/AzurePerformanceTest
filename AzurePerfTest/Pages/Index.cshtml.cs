using AzurePerfTest.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace AzurePerfTest.Pages
{
    public class IndexModel : PageModel
    {
        private static readonly char[] _symbols = new char[] { 'a', 'b', 'c', 'e', 'f', 'j', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
        private static readonly int _size = _symbols.Length;
        private readonly ILogger<IndexModel> _logger;
        private readonly StackOverflow2010Context _ctx;
        private readonly Random _rnd;
        private readonly Stopwatch _sw = new Stopwatch();
        private User[] _users;
        private long _elapsed;
        public long Elapsed => _elapsed;
        public User[] Users => _users;


        public IndexModel(ILogger<IndexModel> logger, StackOverflow2010Context ctx)
        {
            _logger = logger;
            _ctx = ctx;
            _rnd = new Random(DateTime.Now.Millisecond);
        }

        public void OnGet()
        {
            _sw.Start();

            var userName = GetRandomName();
            var p1 = new SqlParameter("@DisplayName", $"%{userName}%");
            var query = _ctx.Users.FromSqlRaw($"GetUsersByDisplayName @DisplayName", p1);

            //var query = _ctx.Users.Where(x => x.DisplayName.Contains(userName)).OrderBy(x => x.DisplayName).Take(25);
            //Console.WriteLine(query.ToQueryString());
            _users = query.ToArray();
            _sw.Stop();
            var elapsed = _sw.ElapsedMilliseconds;
            _logger.LogInformation($"PERF:{nameof(IndexModel)}.${nameof(OnGet)}:{_sw.ElapsedMilliseconds}", _sw.ElapsedMilliseconds);
            _elapsed = elapsed;
        }

        private string GetRandomName()
        {
            var res = new char[] {
                _symbols[_rnd.Next(_size)],
                _symbols[_rnd.Next(_size)],
                _symbols[_rnd.Next(_size)]
            };
            return new string(res);
        }
    }
}
