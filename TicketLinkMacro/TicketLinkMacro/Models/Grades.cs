using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class Grades
    {
        public Grade[] data;
        public CommunicationResult result;
    }

    public class Grade
    {
        public string agreeContext;
        public bool auto;
        public string color;
        public bool direct;
        public int gradeId;
        public int groupSeatCount;
        public string name;
        public string notice;
        public bool preReserveSale;
        public int price;
        public int priority;
        public int remainCnt;
        public bool restriction;
    }
}
