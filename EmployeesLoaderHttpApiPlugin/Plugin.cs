using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using PhoneApp.Domain.DTO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace EmployeesLoaderHttpApiPlugin
{
    [Author(Name = "Ilya Gainulin")]
    public class Plugin : IPluggable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            List<EmployeesDTO> employees = args != null ? args.Cast<EmployeesDTO>().ToList() : new List<EmployeesDTO>();
            HttpClient httpClient = new HttpClient();
            string url = "https://dummyjson.com/users";

            logger.Info($"Loading employees from {url}");

            var httpResponse = httpClient.GetAsync(url).Result;
            string jsonStr = httpResponse.Content.ReadAsStringAsync().Result;

            JObject jsonObj = JObject.Parse(jsonStr);
            JArray usrArray = (JArray)jsonObj["users"];

            int loadedEmplCount = 0;
            foreach (var user in usrArray)
            {
                string fullName = $"{user["firstName"]} {user["lastName"]}";
                employees.Add(new EmployeesDTO(fullName, user["phone"].ToString()));
                loadedEmplCount++;
            }

            logger.Info($"Loaded {loadedEmplCount} employees");
            return employees.Cast<DataTransferObject>();
        }
    }
}
