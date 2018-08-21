using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class RemainSeatData
    {
        public int groupId { get; set; }
        public int gradeId { get; set; }
        public int blockId { get; set; }
        public string blockName { get; set; }
        public int remainCnt { get; set; }
        public DateTime registerTime { get; set; }
    }
}
