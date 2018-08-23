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
        public static string uriRound = "/api/product/round";

        public static string uriGetReservePage(string scheduledId) { return $"reserve/plan/schedule/{scheduledId}"; }
        public static string uriGetGrades(string scheduledId) { return $"api/V2/plan/schedules/{scheduledId}/grades"; }
        public static string uriGetBlocks(string scheduledId) { return $"api/V2/plan/schedules/{scheduledId}/blocks/grades"; }
        public static string uriGetPreoccupancy(string scheduledId) { return $"api/V2/plan/preoccupancy/schedules/{scheduledId}/"; }
        public static string uriGetSoldoutBlocks(string scheduledId) { return $"api/V2/plan/{scheduledId}/seat-soldout/block"; }
        public static string uriGetSoldoutAreas(string scheduledId) { return $"api/V2/plan/{scheduledId}/seat-soldout/area"; }
    }
}
