using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class ProductRound
    {
        public string productId { get; set; }
        public string productDate { get; set; }
        public string startDatetime { get; set; }
        public string startTime { get; set; }
        public string productRound { get; set; }
        public string scheduleId { get; set; }
    }

    public class ProductRounds
    {
        public ProductRound[] data;
        public CommunicationResult result;
    }
}
