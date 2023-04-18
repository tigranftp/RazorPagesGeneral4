using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RazorPagesGeneral.Models;

namespace RazorPagesGeneral.Pages
{

    public class contactModel : PageModel
    {

        private readonly IContactsService contactsService;

        public contactModel(IContactsService service)
        {
            this.contactsService = service;
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            var newContact = new Contact();
            newContact.first_name = Request.Form["first_name"];
            newContact.last_name = Request.Form["last_name"];
            newContact.email = Request.Form["email"];
            newContact.phone = Request.Form["phone"];
            newContact.select_service = Request.Form["select_service"];
            newContact.select_price = Request.Form["select_price"];
            newContact.comments = Request.Form["comments"];
            writeToDBContacts(newContact);
        }

        public static void writeToDBContacts(Contact cnt)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseSqlite(config.GetConnectionString("Default"))
                .Options;

            using (var context = new AppDBContext(options))
            {
                context.Database.EnsureCreated();
                context.Database.Migrate();

                context.Contacts.Add(cnt);
                context.SaveChanges();
            }
        }
    }

}
