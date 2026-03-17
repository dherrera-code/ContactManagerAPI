using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManagerAPI.Model;
using ContactManagerAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace ContactManagerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ContactServices _contactService;
        public ContactController(ContactServices contactServices)
        {
            _contactService = contactServices;
        }

        [HttpPost("CreateContact")]
        public async Task<IActionResult> CreateContact(ContactModel newContact)
        {
            var contact = await _contactService.CreateContact(newContact);
            if(contact != null) return Ok(new {contact});

            return Unauthorized(new {message = "Unable to create contact."});
        }

        [HttpGet("GetAllContacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _contactService.GetAllContactsAsync();

            if(contacts != null) return Ok(contacts);

            return NotFound(new {Message = "No Contacts found."});
        }

        [HttpPut("UpdateContact/{id}")]
        public async Task<IActionResult> UpdateContact(ContactModel updatedContact)
        {
            var success = await _contactService.UpdateContact(updatedContact);

            if(success) return Ok(success);

            return BadRequest(new {message = "Successfully updated contacts."});
        }

        [HttpDelete("DeleteContact/{id}")]
        public async Task<IActionResult> RemoveContact(int id)
        {
            var success = await _contactService.RemoveContact(id);

            if(success) return Ok(success);

            return BadRequest(new {message = "Unable to remove contact!"});
        }
    }
}