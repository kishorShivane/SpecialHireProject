using EbusDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpecialHire.Utilities
{
    public class CommonHelper
    {
        public List<SelectListItem> AddDefaultItem(List<SelectListItem> items)
        {
            items.Insert(0, (new SelectListItem() { Text = "Select", Value = "0", Selected = true }));
            return items;
        }
        public string[] SetMessage(string Message, string MessageType = "E")
        {
            string[] NewMessage = new string[3];
            NewMessage[0] = Message;
            NewMessage[1] = MessageType;
            return NewMessage;
        }

        public List<SelectListItem> GetTime()
        {
            return AddDefaultItem(new List<SelectListItem>() {
                new SelectListItem() { Text = "12:00 AM", Value = "12:00 AM" },
                new SelectListItem() { Text = "12:15 AM", Value = "12:15 AM" },
                new SelectListItem() { Text = "12:30 AM", Value = "12:30 AM" },
                new SelectListItem() { Text = "12:45 AM", Value = "12:45 AM" },
                new SelectListItem() { Text = "01:00 AM", Value = "01:00 AM" },
                new SelectListItem() { Text = "01:15 AM", Value = "01:15 AM" },
                new SelectListItem() { Text = "01:30 AM", Value = "01:30 AM" },
                new SelectListItem() { Text = "01:45 AM", Value = "01:45 AM" },
                new SelectListItem() { Text = "02:00 AM", Value = "02:00 AM" },
                new SelectListItem() { Text = "02:15 AM", Value = "02:15 AM" },
                new SelectListItem() { Text = "02:30 AM", Value = "02:30 AM" },
                new SelectListItem() { Text = "02:45 AM", Value = "02:45 AM" },
                new SelectListItem() { Text = "03:00 AM", Value = "03:00 AM" },
                new SelectListItem() { Text = "03:15 AM", Value = "03:15 AM" },
                new SelectListItem() { Text = "03:30 AM", Value = "03:30 AM" },
                new SelectListItem() { Text = "03:45 AM", Value = "03:45 AM" },
                new SelectListItem() { Text = "04:00 AM", Value = "04:00 AM" },
                new SelectListItem() { Text = "04:15 AM", Value = "04:15 AM" },
                new SelectListItem() { Text = "04:30 AM", Value = "04:30 AM" },
                new SelectListItem() { Text = "04:45 AM", Value = "04:45 AM" },
                new SelectListItem() { Text = "05:00 AM", Value = "05:00 AM" },
                new SelectListItem() { Text = "05:15 AM", Value = "05:15 AM" },
                new SelectListItem() { Text = "05:30 AM", Value = "05:30 AM" },
                new SelectListItem() { Text = "05:45 AM", Value = "05:45 AM" },
                new SelectListItem() { Text = "06:00 AM", Value = "06:00 AM" },
                new SelectListItem() { Text = "06:15 AM", Value = "06:15 AM" },
                new SelectListItem() { Text = "06:30 AM", Value = "06:30 AM" },
                new SelectListItem() { Text = "06:45 AM", Value = "06:45 AM" },
                new SelectListItem() { Text = "07:00 AM", Value = "07:00 AM" },
                new SelectListItem() { Text = "07:15 AM", Value = "07:15 AM" },
                new SelectListItem() { Text = "07:30 AM", Value = "07:30 AM" },
                new SelectListItem() { Text = "07:45 AM", Value = "07:45 AM" },
                new SelectListItem() { Text = "08:00 AM", Value = "08:00 AM" },
                new SelectListItem() { Text = "08:15 AM", Value = "08:15 AM" },
                new SelectListItem() { Text = "08:30 AM", Value = "08:30 AM" },
                new SelectListItem() { Text = "08:45 AM", Value = "08:45 AM" },
                new SelectListItem() { Text = "09:00 AM", Value = "09:00 AM" },
                new SelectListItem() { Text = "09:15 AM", Value = "09:15 AM" },
                new SelectListItem() { Text = "09:30 AM", Value = "09:30 AM" },
                new SelectListItem() { Text = "09:45 AM", Value = "09:45 AM" },
                new SelectListItem() { Text = "10:00 AM", Value = "10:00 AM" },
                new SelectListItem() { Text = "10:15 AM", Value = "10:15 AM" },
                new SelectListItem() { Text = "10:30 AM", Value = "10:30 AM" },
                new SelectListItem() { Text = "10:45 AM", Value = "10:45 AM" },
                new SelectListItem() { Text = "11:00 AM", Value = "11:00 AM" },
                new SelectListItem() { Text = "11:15 AM", Value = "11:15 AM" },
                new SelectListItem() { Text = "11:30 AM", Value = "11:30 AM" },
                new SelectListItem() { Text = "11:45 AM", Value = "11:45 AM" },
                new SelectListItem() { Text = "12:00 PM", Value = "12:00 PM" },
                new SelectListItem() { Text = "12:15 PM", Value = "12:15 PM" },
                new SelectListItem() { Text = "12:30 PM", Value = "12:30 PM" },
                new SelectListItem() { Text = "12:45 PM", Value = "12:45 PM" },
                new SelectListItem() { Text = "13:00 PM", Value = "13:00 PM" },
                new SelectListItem() { Text = "13:15 PM", Value = "13:15 PM" },
                new SelectListItem() { Text = "13:30 PM", Value = "13:30 PM" },
                new SelectListItem() { Text = "13:45 PM", Value = "13:45 PM" },
                new SelectListItem() { Text = "14:00 PM", Value = "14:00 PM" },
                new SelectListItem() { Text = "14:15 PM", Value = "14:15 PM" },
                new SelectListItem() { Text = "14:30 PM", Value = "14:30 PM" },
                new SelectListItem() { Text = "14:45 PM", Value = "14:45 PM" },
                new SelectListItem() { Text = "15:00 PM", Value = "15:00 PM" },
                new SelectListItem() { Text = "15:15 PM", Value = "15:15 PM" },
                new SelectListItem() { Text = "15:30 PM", Value = "15:30 PM" },
                new SelectListItem() { Text = "15:45 PM", Value = "15:45 PM" },
                new SelectListItem() { Text = "16:00 PM", Value = "16:00 PM" },
                new SelectListItem() { Text = "16:15 PM", Value = "16:15 PM" },
                new SelectListItem() { Text = "16:30 PM", Value = "16:30 PM" },
                new SelectListItem() { Text = "16:45 PM", Value = "16:45 PM" },
                new SelectListItem() { Text = "17:00 PM", Value = "17:00 PM" },
                new SelectListItem() { Text = "17:15 PM", Value = "17:15 PM" },
                new SelectListItem() { Text = "17:30 PM", Value = "17:30 PM" },
                new SelectListItem() { Text = "17:45 PM", Value = "17:45 PM" },
                new SelectListItem() { Text = "18:00 PM", Value = "18:00 PM" },
                new SelectListItem() { Text = "18:15 PM", Value = "18:15 PM" },
                new SelectListItem() { Text = "18:30 PM", Value = "18:30 PM" },
                new SelectListItem() { Text = "18:45 PM", Value = "18:45 PM" },
                new SelectListItem() { Text = "19:00 PM", Value = "19:00 PM" },
                new SelectListItem() { Text = "19:15 PM", Value = "19:15 PM" },
                new SelectListItem() { Text = "19:30 PM", Value = "19:30 PM" },
                new SelectListItem() { Text = "19:45 PM", Value = "19:45 PM" },
                new SelectListItem() { Text = "20:00 PM", Value = "20:00 PM" },
                new SelectListItem() { Text = "20:15 PM", Value = "20:15 PM" },
                new SelectListItem() { Text = "20:30 PM", Value = "20:30 PM" },
                new SelectListItem() { Text = "20:45 PM", Value = "20:45 PM" },
                new SelectListItem() { Text = "21:00 PM", Value = "21:00 PM" },
                new SelectListItem() { Text = "21:15 PM", Value = "21:15 PM" },
                new SelectListItem() { Text = "21:30 PM", Value = "21:30 PM" },
                new SelectListItem() { Text = "21:45 PM", Value = "21:45 PM" },
                new SelectListItem() { Text = "22:00 PM", Value = "22:00 PM" },
                new SelectListItem() { Text = "22:15 PM", Value = "22:15 PM" },
                new SelectListItem() { Text = "22:30 PM", Value = "22:30 PM" },
                new SelectListItem() { Text = "22:45 PM", Value = "22:45 PM" },
                new SelectListItem() { Text = "23:00 PM", Value = "23:00 PM" },
                new SelectListItem() { Text = "23:15 PM", Value = "23:15 PM" },
                new SelectListItem() { Text = "23:30 PM", Value = "23:30 PM" },
                new SelectListItem() { Text = "23:45 PM", Value = "23:45 PM" }
            });
        }

        public List<SelectListItem> GetTitles()
        {
            return AddDefaultItem(new List<SelectListItem>() {
                new SelectListItem() { Text = "Mr", Value = "Mr" },
                new SelectListItem() { Text = "Mrs", Value = "Mrs" } });
        }

        //public List<SelectListItem> GetQuotationReportTypes()
        //{
        //    return AddDefaultItem(new List<SelectListItem>() {
        //        new SelectListItem() { Text = "All Quotations", Value = "1" },
        //        new SelectListItem() { Text = "Quotations Done Booking", Value = "2" },
        //            new SelectListItem() { Text = "Quotations Not Booked", Value = "3" }
        //    });
        //}

        public string GetLoggedInUserName()
        {
            string userID = "System";
            if (HttpContext.Current.Session["USER"] != null)
            {
                var user = (ApplicationUser)HttpContext.Current.Session["USER"];
                return user.UserName.ToUpper();
            }
            return userID;
        }

        public bool IsUserLoggedIn()
        {
            bool status = false;
            if (HttpContext.Current.Session["USER"] == null)
            {
                status = true;
            }
            return status;
        }

        public DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
    }

    public enum UserRoll
    {
        NoAccess,
        SuperAdmin,
        Admin,
        Dispatcher
    }
}