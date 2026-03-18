using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManagerAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerAPI.Context
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<UserInfoModel> Users { get; set; }
    }
}