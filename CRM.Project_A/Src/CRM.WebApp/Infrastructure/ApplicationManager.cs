﻿using CRM.WebApi.Models;
using CRM.WebApp.Models;
using EntityLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace CRM.WebApp.Infrastructure
{
    public class ApplicationManager : IDisposable
    {

        // public ApplicationManager(){}
        DataBaseCRMEntityes db = new DataBaseCRMEntityes();
        ModelFactory factory = new ModelFactory();
        public async Task<List<ContactResponseModel>> GetAllContacts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            List<Contact> dbContactList = await db.Contacts.ToListAsync();
            List<ContactResponseModel> responseContactList = new List<ContactResponseModel>();

            return dbContactList.Select(x => factory.CreateContactResponseModel(x)).ToList();

        }
        //public async Task<List<Contact>> GetContactPage(int start, int numberRows, bool flag)
        //{
        //    var query = await db.Contacts.OrderBy(x => x.DateInserted).Skip(start).Take(numberRows).ToListAsync();

        //    for (int i = 0; i < query.Count; i++)
        //    {
        //        query[i].EmailLists = new List<EmailList>();
        //    }
        //    return query;
        //}

        public async Task<ContactResponseModel> GetContactByGuid(Guid id)
        {
            var contact = await db.Contacts.FirstOrDefaultAsync(t => t.GuID == id);

            return factory.CreateContactResponseModel(contact);
        }

        public async Task<int> GetContactsPageCounter()
        {
            return await db.Contacts.CountAsync() > 10 ? await db.Contacts.CountAsync() / 10 : 1;
        }

        public async Task<List<ContactResponseModel>> GetContactsByGuIdList(List<Guid> GuIdList)
        {
            List<ContactResponseModel> ContactsList = new List<ContactResponseModel>();
            foreach (var guid in GuIdList)
            {
                ContactsList.Add(await GetContactByGuid(guid));
            }

            return ContactsList;
        }
        public async Task<bool> UpdateContact(string guid, ContactRequestModel contact)
        {
            var dbContactToUpdate = await db.Contacts.FirstOrDefaultAsync(c => c.GuID.ToString() == guid);

            if (dbContactToUpdate == null) return false;

            dbContactToUpdate.FullName = contact.FullName;
            dbContactToUpdate.Country = contact.Country;
            dbContactToUpdate.Position = contact.Position;
            dbContactToUpdate.CompanyName = contact.CompanyName;
            dbContactToUpdate.Email = contact.Email;

            db.Entry(dbContactToUpdate).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                //if (!await ContactExistsAsync(Guid.Parse(guid)))
                //{
                //    return false;
                //}
                //else
                //{
                //    throw;
                //}

            }
            return true;
        }


        public async Task<Contact> AddContact(ContactRequestModel contact)
        {
            Contact contacts = factory.CreateContact(contact);

            db.Contacts.Add(contacts);
            await db.SaveChangesAsync();

            return contacts;
        }
        public async Task<ContactResponseModel> RemoveContact(string guid)
        {
            var contact = await db.Contacts.FirstOrDefaultAsync(c => c.GuID.ToString() == guid);

            var resModel = factory.CreateContactResponseModel(contact);
            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();

            return resModel;
        }
        public async Task<bool> ContactExistsAsync(Guid id)
        {
            return await db.Contacts.CountAsync(e => e.GuID == id) > 0;
        }
        public async void SaveDb()
        {
            await db.SaveChangesAsync();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

}