using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class ProductRound
    {
        public string productId;
        public string productDate;
        public string startDatetime;
        public string productRound;
        public string scheduleId;
    }

    public class ProductRounds
    {
        public ProductRound[] data;
        public CommunicationResult result;
    }
}
