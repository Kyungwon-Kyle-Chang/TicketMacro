using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class Blocks
    {
        public Block[] data { get; set; }
        public CommunicationResult result { get; set; }
    }

    public class Block
    {
        public int blockId;
        public int gradeId;
        public int remainCnt;
    }
}
