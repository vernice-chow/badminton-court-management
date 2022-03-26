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


        //Zi Yan part
        //Coach Session
        public async Task<List<Coach>> GetAllCoach()
        {
            List<Coach> coaches = null;
            try
            {
                coaches = (await firebase
                     .Child("Coach") //Coach is my table name
                     .OnceAsync<Coach>())
                     .Select(item => new Coach
                     {
                         CoachID = item.Key.ToString(),
                         CoachName = item.Object.CoachName,
                         CoachPhone = item.Object.CoachPhone,
                         CoachDescription = item.Object.CoachDescription,
                         CoachPrice = item.Object.CoachPrice,
                         CoachCourt = item.Object.CoachCourt,

                     }).ToList();
            }
            catch (FirebaseException firebaseException)
            {
                Debug.WriteLine(firebaseException.InnerException.Message);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e, "Error from parsing Firebase JSON");
            }
            return coaches;
        }

        public async Task AddCoach(string CoachName, string CoachPhone, string CoachPrice, string CoachDescription, string CoachCourt)
        {
            var task = await firebase
              .Child("Coach")
              .PostAsync(new Coach{ CoachName = CoachName, CoachPhone = CoachPhone, CoachPrice = CoachPrice, CoachDescription = CoachDescription, CoachCourt = CoachCourt});
        }

        public async Task DeleteCoach(string key)
        {
            var toDeletePerson = (await firebase
              .Child("Coach")
              .OnceAsync<Coach>()).Where(a => a.Key == key).FirstOrDefault(); //linkq concept
            await firebase.Child("Coach").Child(toDeletePerson.Key).DeleteAsync(); //delete data
        }

        public async Task UpdateCoach(string key, string CoachPhone, string CoachPrice, string CoachDescription, string CoachCourt)
        {

            var toUpdateCoach = (await firebase
             .Child("Coach")
             .OnceAsync<InsertUpdateCoach>()).Where(a => a.Key == key).FirstOrDefault();


            //setting CoachName cannot be updated
            toUpdateCoach.Object.CoachPhone = CoachPhone;
            toUpdateCoach.Object.CoachPrice = CoachPrice;
            toUpdateCoach.Object.CoachDescription = CoachDescription;
            toUpdateCoach.Object.CoachCourt = CoachCourt;

            await firebase
             .Child("Coach")
             .Child(key)
             .PutAsync(JsonConvert.SerializeObject(toUpdateCoach.Object)); //put is the update  
        }



        //Customer Booking Session
        public async Task<List<Hire>> GetAllHires()
        {

            return (await firebase
              .Child("HireCoach") 
              .OnceAsync<Hire>()).Select(item => {
                  Debug.WriteLine("Coach ID: " + item.Key.ToString());
                  return new Hire
                  {
                      FirebaseUserId = item.Key.ToString(),
                      userEmail = item.Object.userEmail,
                      userPhone = item.Object.userPhone,
                      preferDate = item.Object.preferDate,
                      preferTime = item.Object.preferTime,
                      userSelectedCoach = item.Object.userSelectedCoach,
                  };
              }).ToList();
        }

        //customer hire coach --> customer side
        public async Task AddHire(string userEmail, string userPhone,string preferDate, string preferTime,string userSelectedCoach)
        {
            await firebase
              .Child("HireCoach")
              .PostAsync(new Hire() { userEmail = userEmail, userPhone = userPhone, preferDate = preferDate, preferTime = preferTime, userSelectedCoach = userSelectedCoach  }); //PostAsync is the insert
        }

        public async Task DeleteHire(string key)
        {
            var toDeletePerson = (await firebase
              .Child("HireCoach")
              .OnceAsync<Hire>()).Where(a => a.Key == key).FirstOrDefault(); //linkq concept
            await firebase.Child("HireCoach").Child(toDeletePerson.Key).DeleteAsync(); //delete data
        }


        //Jason Part
        public async Task<List<Merchandise>> GetAllMerchandise()
        {
            List<Merchandise> merchs = null;
            var task = await firebase.Child("Merchandise").OnceAsync<Merchandise>();
            Debug.WriteLine("Number of records: " + task.Count);
            string name = task.First().Key;
            Debug.WriteLine("Number of records: " + name);
            try
            {
                merchs = (await firebase
                     .Child("Merchandise")
                     .OnceAsync<Merchandise>())
                     .Select(item =>
                     {
                         Debug.WriteLine("Merchandise ID: " + item.Key.ToString());
                         return new Merchandise
                         {
                             MerchandiseID = item.Key.ToString(),
                             Name = item.Object.Name,
                             Type = item.Object.Type,
                             Price = item.Object.Price,
                             ImageURL = item.Object.ImageURL,
                         };
                     }).ToList();

            }
            catch (FirebaseException firebaseException)
            {
                Debug.WriteLine(firebaseException.Message);
            }
            return merchs;
        }

        public async Task CreateNewMerch(string name, string type, string price, string imageURL)
        {
            Merchandise newMerch = new Merchandise { Name = name, Type = type, Price = price, ImageURL = imageURL };

            var Result = await firebase.Child("Merchandise").PostAsync(new Merchandise { Name = name, Type = type, Price = price, ImageURL = imageURL });
        }

        public async Task UpdateItem(string name, string type, string price, string imageurl)
        {
            var toUpdateItem = (await firebase
              .Child("Merchandise")
              .OnceAsync<InsertUpdateMerch>()).Where(a => a.Object.Name == name).FirstOrDefault();

            string key = toUpdateItem.Key;
            toUpdateItem.Object.ImageURL = imageurl;
            toUpdateItem.Object.Name = name;
            toUpdateItem.Object.Type = type;
            toUpdateItem.Object.Price = price;

            await firebase
             .Child("Merchandise")
             .Child(key)
             .PutAsync(JsonConvert.SerializeObject(toUpdateItem.Object));
        }

        public async Task DeleteItem(string name)
        {
            var toDeleteItem = (await firebase
              .Child("Merchandise")
              .OnceAsync<Merchandise>()).Where(a => a.Object.Name == name).FirstOrDefault();

            string key = toDeleteItem.Key;

            await firebase.Child("Merchandise").Child(toDeleteItem.Key).DeleteAsync();

        }



        //Shin Jie part
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



        //Kee Ling part
        public async Task<List<Court>> GetAllCourts()
        {
            return (await firebase
              .Child("KLCourt")
              .OnceAsync<Court>()).Select(item => new Court
              {
                  CourtId = item.Key.ToString(),
                  Availability = item.Object.Availability
              }).ToList();
        }


        public async Task AddPerson(string UserName, string UserEmail, string feees)
        {
            await firebase
              .Child("KLConfirmBooking")
              .PostAsync(new KLConfirmBooking() { UserName = UserName, UserEmail = UserEmail, BookingFees = feees });
        }


        public async Task UpdateCondition(string code)
        {
            await firebase
              .Child("KLCourt")
              .Child(code)
              .PutAsync(new Court() { Availability = "not null" });
        }


        public async Task AddSlot(string code) //check condition
        {
            await firebase
              .Child("KLBooking")
              .PostAsync(new KLBooking() { BookedSlot = code });
        }


        public async Task<List<KLBooking>> GetBookedCourt()
        {
            return (await firebase
                .Child("KLBooking")
                .OnceAsync<KLBooking>()).Select(item => new KLBooking
                {
                    BookedId = item.Key.ToString(),
                    BookedSlot = item.Object.BookedSlot,
                    //CourtId = item.Object.CourtId
                }).ToList();
        }


        public async Task DeleteSlot(string keykey)
        {
            var delslot = (await firebase
              .Child("KLBooking")
              .OnceAsync<KLBooking>()).Where(a => a.Key == keykey).FirstOrDefault();
            await firebase.Child("KLBooking").Child(delslot.Key).DeleteAsync();

        }


        public async Task<List<FileTitle>> GetAllFileTitle()
        {

            return (await firebase
              .Child("FileTitle")
              .OnceAsync<FileTitle>()).Select(item => new FileTitle
              {
                  FirebaseId = item.Key.ToString(),
                  FileTitle2 = item.Object.FileTitle2,
                  FileName = item.Object.FileName,
                  FileURL = item.Object.FileURL
              }).ToList();
        }


        public async Task AddImageTitle(string title, string imageName, string imageURL)
        {

            await firebase
              .Child("FileTitle")
              .PostAsync(new ImageTitle() { Title = title, ImageName = imageName, ImageURL = imageURL });
        }



    }
}
