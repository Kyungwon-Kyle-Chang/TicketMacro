using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketLinkMacro.Models
{
    public class SoldoutSeats
    {
        public object data;
        public CommunicationResult result;

        private Seat[] _seatStatus;
        public Seat[] SeatStatus
        {
            get
            {
                if (_seatStatus == null)
                {
                    string[] units = data.ToString().Split(new string[] {"{", "\r\n ", ",", "}"}, StringSplitOptions.RemoveEmptyEntries);
                    _seatStatus = new Seat[units.Length];

                    for(int i=0; i<units.Length; i++)
                    {
                        string[] values = units[i].Split(new string[] { "\"", ": ", " " }, StringSplitOptions.RemoveEmptyEntries);
                        Seat seat = new Seat();
                        seat.id = values[0];
                        seat.sold = bool.Parse(values[1]);
                        _seatStatus[i] = seat;
                    }

                    return _seatStatus;
                }
                else
                    return _seatStatus;
            }
        }

        public class Seat
        {
            public string id;
            public bool sold;
        }
    }
}
