using Firebase.Database;
using Firebase.Database.Query;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace BadmintonCourtBookingSystem
{
    class FirebaseHelper
    { 
        FirebaseClient firebase = new FirebaseClient(GlobalData.firebaseDatabase);
        private List<CourtDetail> all_detail = new List<CourtDetail>();
        private List<CourtDetail> history_detail = new List<CourtDetail>();

        public async Task DeleteHistory(string date)
        {

            var toDeletePerson = (await firebase
              .Child("BookingInfo")
              .OnceAsync<DaybyDay>()).Where(r => r.Key.Contains(date)).FirstOrDefault();
            await firebase.Child("BookingInfo").Child(toDeletePerson.Key).DeleteAsync();

        }

        public async Task<List<CourtDetail>> BookedDetail()
        {
            List<CourtDetail> get_detail = new List<CourtDetail>();
            List<CourtNum> get_court = new List<CourtNum>();
            List<CourtNum> half_detail = new List<CourtNum>();
            List<DaybyDay> get_day = new List<DaybyDay>();

            get_day = (await firebase.Child("BookingInfo").OnceAsync<DaybyDay>()).Select(item => new DaybyDay
            {
                Day = item.Key.ToString()

            }).ToList();


            get_court = (await firebase
                  .Child("BookingInfo").Child(get_day[0].Day)
                  .OnceAsync<CourtNum>()) //deserialization
                  .Select(item => new CourtNum
                  {
                      Courts = item.Key.ToString(),

                  }).ToList();

            foreach (var item in get_court)
            {
                half_detail.Add(new CourtNum(item.Courts));

            }

            for (int i = 0; i < get_day.Count; i++)
            {
                for (int j = 0; j < half_detail.Count; j++)
                {

                    get_detail = (await firebase
                      .Child("BookingInfo/" + get_day[i].Day + "/" + half_detail[j].Courts)
                      .OnceAsync<CourtDetail>()) //deserialization
                      .Select(item => new CourtDetail
                      {
                          Slot = item.Key.ToString(),
                          Email = item.Object.Email,
                          Time = item.Object.Time,

                      }).ToList();

                    foreach (var item in get_detail)
                    {
                        /*  int result = DateTime.Compare(DateTime.Now, DateTime.Parse(get_day[i].Day));
                          if (result < 0)*/

                        all_detail.Add(new CourtDetail(DateTime.Parse(get_day[i].Day).ToString("yyyy-MM-dd"), half_detail[j].Courts, item.Slot, item.Email, item.Time));

                    }


                }
            }
            return all_detail;

        }


        //add slot time
        public async Task AddSlot(string day, string time, string court, string email)
        {

            await firebase
              .Child("BookingInfo/" + day).Child(court)
              .PostAsync(new CourtDetail()
              {
                  Time = time,
                  Email = email

              });

        }

        // delete a booking by delete email
        public async Task DeleteBooking(string day, string time, string key, string court)
        {
            var toUpdateData = (await firebase
                .Child("BookingInfo/" + day).Child(court)
                .OnceAsync<CourtDetail>()).Where(a => a.Key == key).FirstOrDefault();

            toUpdateData.Object.Email = "";
            await firebase
          .Child("BookingInfo/" + day).Child(court).Child(key)
          .PutAsync(JsonConvert.SerializeObject(toUpdateData.Object));
        }

        //update booked time or email
        public async Task UpdateData(string day, string time, string key, string court, string email)
        {

            var toUpdateData = (await firebase
              .Child("BookingInfo/" + day).Child(court)
              .OnceAsync<CourtDetail>()).Where(a => a.Key == key).FirstOrDefault();

            toUpdateData.Object.Time = time;
            toUpdateData.Object.Email = email;

            await firebase
            .Child("BookingInfo/" + day).Child(court).Child(key)
            .PutAsync(JsonConvert.SerializeObject(toUpdateData.Object));
        }



    }
}
