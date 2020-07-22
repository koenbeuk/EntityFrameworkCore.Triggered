using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketManager
{

    public class SupportStaff
    {
        public int Id { get; set; }

        public string Email { get; set; }
    }


    public class SupportTicket
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string ReplyEmail { get; set; }
    }

    public class ApplicationContext
    {
        public DbSet MyProperty { get; set; }

    }
}
