using System.Collections.Generic;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace BadmintonCourtBookingSystem
{
    class Coach
    {
        public string CoachID { get; set; }

        public string CoachName { get; set; }

        public string CoachPhone { get; set; }

        public string CoachPrice { get; set; }

        public string CoachDescription { get; set; }

        public string CoachCourt { get; set; }


    }
}
