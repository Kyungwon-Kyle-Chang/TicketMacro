using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketLinkMacro.Utils;

namespace TicketLinkMacro.Models
{
    public class RemainSeatData
    {
        public int gradeId { get; set; }
        public int blockId { get; set; }
        public string gradeName { get; set; }
        public string blockName { get; set; }
        public int remainCnt { get; set; }
        public DateTime registerTime { get; set; }
        public string remainSeatsID { get; set; }

        public CommandBase TicketBuyButton { get; private set; }

        public RemainSeatData(Action<object> ticketBuyAction)
        {
            TicketBuyButton = new CommandBase(ticketBuyAction);
        }
    }
}
