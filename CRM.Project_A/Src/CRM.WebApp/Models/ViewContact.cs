﻿using EntityLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.WebApp.Models
{
    public class ViewContact
    {
        public ViewContact()
        {
            GuID = new Guid();
            DateInserted = null;
            EmailLists = new Dictionary<int, string>();
        }
        public ViewContact(Contact contact)
        {
            EmailLists = new Dictionary<int, string>();
            FullName = contact.FullName;
            CompanyName = contact.CompanyName;
            Position = contact.Position;
            Country = contact.Country;
            Email = contact.Email;
            GuID = contact.GuID;
            DateInserted = contact.DateInserted;

            foreach (var EmailList in contact.EmailLists)
            {
                EmailLists.Add(EmailList.EmailListID, EmailList.EmailListName);
            }
        }

        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public Guid GuID { get; set; }
        public DateTime? DateInserted { get; set; }
        public Dictionary<int, string> EmailLists { get; set; }
    }
}