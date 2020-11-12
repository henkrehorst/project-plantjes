using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_c_plantjes.Models.Users
{
    public class UserData : User
    {
        public int UserDataID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ZipCode { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Avatar { get; set; }

        public UserData(int uid, string email, string pw, int status, DateTime created, int udid, string fname, string lname, string zcode, double lat, double lng, string avatar) : base(uid, email, pw, status, created)
        {
            UserDataID = udid;
            FirstName = fname;
            LastName = lname;
            ZipCode = zcode;
            Lat = lat;
            Lng = lng;
            Avatar = avatar;
        }

    }
}
