namespace TicketLinkMacro.Models
{
    public class Grades
    {
        public Grade[] data;
        public CommunicationResult result;
    }

    public class Grade
    {
        public string agreeContext { get; set; }
        public bool auto { get; set; }
        public string color { get; set; }
        public bool direct { get; set; }
        public int gradeId { get; set; }
        public int groupSeatCount { get; set; }
        public string name { get; set; }
        public string notice { get; set; }
        public bool preReserveSale { get; set; }
        public int price { get; set; }
        public int priority { get; set; }
        public int remainCnt { get; set; }
        public bool restriction { get; set; }
    }
}
