using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBookingSystem
{
    class DaybyDay
    {
        public string Day { get; set; }
    }
    class CourtNum
    {
        public string Courts { get; set; }
        public CourtNum() { }
        public CourtNum(string Courts)
        {
            this.Courts = Courts;

        }
    }
    class CourtDetail
    {
        public string Dayy { get; set; }
        public string Courtss { get; set; }
        public string Slot { get; set; }
        public string Email { get; set; }
        public string Time { get; set; }

        public CourtDetail() { }
        public CourtDetail(string date, string court, string slot, string email, string time)
        {
            this.Dayy = date;
            this.Courtss = court;
            this.Slot = slot;
            this.Email = email;
            this.Time = time;

        }
    }

}