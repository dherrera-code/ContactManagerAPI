using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManagerAPI.Context;
using ContactManagerAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerAPI.Services
{
    public class ContactServices
    {
        private readonly DataContext _dataContact;
        public ContactServices(DataContext dataContext)
        {   
            _dataContact = dataContext;
        }
        public async Task<bool> CreateContact(ContactModel newContact)
        {
            if(DoesNameExist(newContact.Name) == null) return false;
            
            ContactModel contact = new();
            contact.Name = newContact.Name;
            contact.Email = newContact.Email;
            contact.PhoneNumber = newContact.PhoneNumber;
            await _dataContact.Contacts.AddAsync(contact);
            return await _dataContact.SaveChangesAsync() != 0;
        }
        private async Task<bool> DoesNameExist(string name)
        {
            return await _dataContact.Contacts.SingleOrDefaultAsync(contact => contact.Name == name) != null;
        }

        public async Task<List<ContactModel>> GetAllContactsAsync() => await _dataContact.Contacts.ToListAsync();
        
    }
}