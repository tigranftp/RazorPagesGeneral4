using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;
using System.Threading;

namespace RazorPagesGeneral
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }


    [Serializable]
    public class Testimonial
    {
        public string? CommentLabel { get; set; }
        public string? Comment { get; set; }
        public string? Name { get; set; }
        public string? JobTitle { get; set; }
        public string? ImageUrl { get; set; }
    }
    public interface ITestimonialService
    {
        IEnumerable<Testimonial> getAll();
    }

    public class TestimonialService : ITestimonialService
    {
        public IEnumerable<Testimonial> getAll()
        {
            var streamReader = new StreamReader("testimonials.json");

            string json = streamReader.ReadToEnd();
            return JsonSerializer.Deserialize<Testimonial[]>(json) ?? new Testimonial[] { };
        }
    }

    public interface IContactsService
    {
        void writeContact(Contact newContact);
    }

    public class Contact
    {
        public String first_name { get; set; }
        public String last_name { get; set; }
        public String email { get; set; }
        public String phone { get; set; }
        public String select_service { get; set; }
        public String select_price { get; set; }
        public String comments { get; set; }
    }
    public class ContactsService : IContactsService
    {
        private Mutex mutexObj = new Mutex();
        private String csvFileName = @"csv\contacts.csv";
        public void writeContact(Contact newContact)
        {
            mutexObj.WaitOne();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using (var stream = File.Open(csvFileName, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecord<Contact>(newContact);
                csv.NextRecord();
            }
            mutexObj.ReleaseMutex();
        }
    }

}
