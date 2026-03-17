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
            // if(DoesNameExist(newContact.Name) == null) return false;
            
            ContactModel contact = new();
            contact.Name = newContact.Name;
            contact.Email = newContact.Email;
            contact.PhoneNumber = newContact.PhoneNumber;
            await _dataContact.Contacts.AddAsync(contact);
            return await _dataContact.SaveChangesAsync() != 0;
        }
        // private async Task<bool> DoesNameExist(string name)
        // {
        //     return await _dataContact.Contacts.SingleOrDefaultAsync(contact => contact.Name == name) != null;
        // }

        public async Task<List<ContactModel>> GetAllContactsAsync() => await _dataContact.Contacts.ToListAsync();

        private async Task<ContactModel> GetContactById(int id) =>  await _dataContact.Contacts.FindAsync(id);
        

        public async Task<bool> UpdateContact(ContactModel updatedContact)
        {
            var contactToEdit = await GetContactById(updatedContact.Id);

            if(contactToEdit == null) return false;

            contactToEdit.Name = updatedContact.Name;
            contactToEdit.Email = updatedContact.Email;
            contactToEdit.PhoneNumber = updatedContact.PhoneNumber;
            _dataContact.Update(contactToEdit);
            return await _dataContact.SaveChangesAsync() != 0;
        }

        public async Task<bool> RemoveContact(int id)
        {
            var contactToRemove = await GetContactById(id);
            if(contactToRemove == null) return false;

            _dataContact.Remove(contactToRemove);
            return await _dataContact.SaveChangesAsync() != 0;
        }
    }
}