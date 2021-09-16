using System;
using System.Collections.Generic;

#nullable disable

namespace AzurePerfTest.DAL
{
    public partial class Comment
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int PostId { get; set; }
        public int? Score { get; set; }
        public string Text { get; set; }
        public int? UserId { get; set; }
    }
}
