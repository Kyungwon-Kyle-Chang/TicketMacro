using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Utils
{
    public static class Configs
    {
        public static string webAPI = "http://www.ticketlink.co.kr/";
        
        public static string uriGetGrades(string scheduledId) { return $"api/V2/plan/schedules/{scheduledId}/grades"; }
        public static string uriGetBlocks(string scheduledId) { return $"api/V2/plan/schedules/{scheduledId}/blocks/grades"; }
    }
}
