using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network
{
    public class UserInfoDesc
    {
        public enum UserStatus
        {
            Unknown         = 0,
            Unregistered    = 1,
            Online          = 2,
            OnlineAway      = 3,
            Offline         = 4,            
        }


        public /*readonly*/ string UserId;
        public /*readonly*/ string SignalRId;
        public /*readonly*/ string FirstName;
        public /*readonly*/ string LastName;
        public /*readonly*/ string Gender;
        public /*readonly*/ string BirthYear;
        public /*readonly*/ string BirthMonth;
        public /*readonly*/ string BirthDay;
        public /*readonly*/ string Height;
        public /*readonly*/ string Weight;
        public /*readonly*/ string RegistrationDate;
        public /*readonly*/ string Online;
        public /*readonly*/ string ChatOnline;

        public UserInfoDesc(string userId, string signalRId, string first, string last, string gender,
            string byear, string bmonth, string bday, string height, string weight, string regDate, string inChat, string online)
        {
            this.UserId = userId;
            this.SignalRId = signalRId;
            this.FirstName = first;
            this.LastName = last;
            this.Gender = gender;
            this.BirthYear = byear;
            this.BirthMonth = bmonth;
            this.BirthDay = bday;
            this.Height = height;
            this.Weight = weight;
            this.RegistrationDate = regDate;
            this.ChatOnline = inChat;
        }

        public UserInfoDesc() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        { }
        public override string ToString()
        {
            return string.Format(@"User: {0} SignalR: {1} First: {2} Last: {3} Gender: {4} Birth: {5}/{6}/{7}",
                this.UserId,
                this.SignalRId,
                this.FirstName,
                this.LastName,
                this.Gender,
                this.BirthDay,
                this.BirthMonth,
                this.BirthYear);
        }
    }
}
