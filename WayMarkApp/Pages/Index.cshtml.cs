using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WayMarkApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public List<Submission> Submissions { get; set; } = new List<Submission>();

        public async Task OnGetAsync()
        {
            //var newString = ReverseString("JASON");

            var client = new HttpClient();
            var data = await client.GetStringAsync("https://techtestpersonapi.azurewebsites.net/api/GETPersonsTechTestAPI?code=Z5Dm297Ijn9weSo75EVtsJHN9HoVE0fgJt8zIGXWV4ZOOCGNpaYBtw==");
            var submissions = JsonConvert.DeserializeObject<List<Submission>>(data);

            var sortBy = Request.Query["sortby"];
            var reverse = Request.Query["reverse"];
            var save = Request.Query["save"];

            if (sortBy == "age")
            {
                if (reverse == "true")
                {
                    foreach (var item in submissions)
                    {
                        Submissions.Add( new Submission { Id = item.Id, Age = item.Age, Name = ReverseString(item.Name) });
                    }
                    Submissions = Submissions.OrderBy(s => s.Age).ToList();

                    if (save == "true")
                    {
                        SaveToCSV(Submissions);
                    }
                }
                else
                { 
                    Submissions = submissions.OrderBy(s => s.Age).ToList();
                }
            }
            else
            {
                Submissions = submissions;
            }
        }


        private List<Submission> AgeGreaterThanTenAsync(List<Submission> submissions) {

            return submissions.Where(s => s.Age > 10).ToList();
        }

        private void SaveToCSV(List<Submission> submissions) {

            string filePath = @"C:\Waymark\FEFle123.csv";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Id,Age,Name");
            foreach (var item in submissions)
            {
                sb.AppendLine($"{item.Id},{item.Age},{item.Name}" + Environment.NewLine);
            }
                System.IO.File.WriteAllText(filePath, sb.ToString());
        }

        private string ReverseString(string inputString)
        {
            char[] charArray = inputString.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }



    public class Submission
    {
        public int Age { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

    }

}
