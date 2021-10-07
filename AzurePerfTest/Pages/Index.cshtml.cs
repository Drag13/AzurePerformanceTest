using AzurePerfTest.DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

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
            _users = GetUsersSQL(userName);
            _sw.Stop();
            var elapsed = _sw.ElapsedMilliseconds;
            _logger.LogInformation($"PERF:{nameof(IndexModel)}.${nameof(OnGet)}:{_sw.ElapsedMilliseconds}", _sw.ElapsedMilliseconds);
            _elapsed = elapsed;
        }

        private User[] GetUsersEF(string key)
        {
            return _ctx.Users
                .Where(x => x.DisplayName.StartsWith(key))
                .OrderBy(x => x.DisplayName)
                .Take(25)
                .ToArray();
        }

        private Task<User[]> GetUsersEFAsync(string key)
        {
            return _ctx.Users
                .Where(x => x.DisplayName.Contains(key))
                .OrderBy(x => x.DisplayName)
                .Take(25)
                .ToArrayAsync();
        }

        private User[] GetUsersSQL(string key)
        {
            var p1 = new SqlParameter("@DisplayName", $"{key}%");
            var query = _ctx.Users.FromSqlRaw($"SELECT TOP 25 * FROM USERS WITH (NOLOCK) WHERE DisplayName LIKE @DisplayName ORDER BY DisplayName", p1);
            return query.ToArray();
        }

        private User[] GetUsersSP(string key)
        {
            var p1 = new SqlParameter("@DisplayName", $"%{key}%");
            var query = _ctx.Users.FromSqlRaw($"GetUsersByDisplayName @DisplayName", p1);
            return query.ToArray();
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
