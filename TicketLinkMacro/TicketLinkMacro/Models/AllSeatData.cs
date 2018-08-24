using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class AllSeatData
    {
        public Data data;
        public CommunicationResult result;

        public class Data
        {
            public Dictionary<string, Seat[]> able;
            public Dictionary<string, Seat[]> disable;

            public class Seat
            {
                public string allotmentCode;
                public VirtualVector2<int> area;
                public string[] attributes;
                public string blockId;
                public int colIdx;
                public Rectangle cornerPoints;
                public string gate;
                public string gradeId;
                public string groupId;
                public int linkedId;
                public string logicalSeatid;
                public string mapInfo;
                public string orderNum;
                public Vector2 position;
                public int rowIdx;
                public int seatCount;
                public string sortMapInfo;

                public class Rectangle
                {
                    public Vector2 ne;
                    public Vector2 nw;
                    public Vector2 se;
                    public Vector2 sw;
                }
            }
        }
    }
}
