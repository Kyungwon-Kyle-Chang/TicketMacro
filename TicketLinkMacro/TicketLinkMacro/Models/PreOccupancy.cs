using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class PreOccupancy
    {
        public string auto;
        public string code;
        public int memberNo;
        public uint scheduleId;
        public Seat[] seats;
        public int totalCnt;
        public string[] zones;

        public class Seat
        {
            public string allotmentCode;
            public VirtualVector2<int> area;
            public int blockId;
            public uint logicalSeatId;
            public int orderNum;
            public int productGradeId;
            public string productGradeName;
            public string seatAttribute;
            public string sortSeatAttribute;
        }
    }
}
