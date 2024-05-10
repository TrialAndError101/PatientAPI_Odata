using Microsoft.Extensions.Logging;
using PatientAPI_OData.Models;
using PatientAPI_OData.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;


namespace PatientAPI_OData.Services
{
    public class CacheService : ICacheService
    {

        private const string CacheKey = "PatientDataKey";

        public CacheService()
        {

        }

        public async Task<string> DataInputErrors(PatientModel patientInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(patientInfo.Firstname))
            {
                Console.WriteLine("Firstname is required;");
                stringBuilder.Append("Firstname is required;");
            }
            if (string.IsNullOrEmpty(patientInfo.Lastname))
            {
                Console.WriteLine("Lastname is required;");
                stringBuilder.Append("Lastname is required;");
            }
            if (string.IsNullOrEmpty(patientInfo.City))
            {
                Console.WriteLine("City is required;");
                stringBuilder.Append("City is required;");
            }
            if (string.IsNullOrEmpty(patientInfo.Active))
            {
                Console.WriteLine("Active is required;");
                stringBuilder.Append("Active is required;");
            }
            else
            {
                if (!(patientInfo.Active.ToLower() == "yes" || patientInfo.Active.ToLower() == "no"))
                {
                    Console.WriteLine("Active should be Yes or No;");
                    stringBuilder.Append("Active should be Yes or No;");
                }
            }
            return stringBuilder.ToString();
            
        }
        public async Task<bool> InsertPatientData(PatientModel patientInfo)
        {
            try
            {
                // Create a MemoryCache instance
                MemoryCache cache = MemoryCache.Default;

                // Check if cache already contains the key
                if (cache.Contains(CacheKey))
                {
                    // Get data from cache
                    List<PatientModel> patients = (List<PatientModel>)cache.Get(CacheKey);

                    int maxId = patients.Max(x => x.id);

                    // Set new id
                    patientInfo.id = maxId + 1;

                    // Add new data to existing data
                    patients.Add(patientInfo);

                    // Add data to cache
                    cache.Add(CacheKey, patients, DateTimeOffset.Now.AddMinutes(10)); // Expires after 10 minutes
                }
                else
                {
                    // Add data to cache
                    cache.Add(CacheKey, new List<PatientModel> { patientInfo }, DateTimeOffset.Now.AddMinutes(10)); // Expires after 10 minutes
                }

                Console.WriteLine($"ID: {patientInfo.id} ; {patientInfo.Firstname}'s Data added to cache");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InsertPatientData: {ex.Message} {ex.StackTrace}");
                return false;
            }

        }

        public async Task<List<PatientModel>> GetPatientsData(string id = null)
        {
            Console.WriteLine("Getting all patients data from cache");

            List<PatientModel> patients = new List<PatientModel>();

            try
            {
                // Create a MemoryCache 
                MemoryCache cache = MemoryCache.Default;

                //Get current cache count
                long cacheCount = cache.GetCount();

                if(cacheCount == 0)
                {
                    Console.WriteLine("No data in cache");
                }
                else
                {
                    // Get data from cache
                    patients = (List<PatientModel>)cache.Get(CacheKey);

                    if (!string.IsNullOrEmpty(id))
                    {
                        // Filter data based on id
                        patients = patients.Where(x => x.id == Convert.ToInt32(id)).ToList();
                    }
                }
                return patients;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPatientsData: {ex.Message} {ex.StackTrace}");
                return null;
            }            
        }

        public async Task<List<PatientModel>> UpdatePatientData(string id,PatientModel patientInfo)
        {
            try
            {
                // Create a MemoryCache instance
                MemoryCache cache = MemoryCache.Default;

                // Check if cache already contains the key
                if (cache.Contains(CacheKey))
                {
                    // Get data from cache
                    List<PatientModel> patients = (List<PatientModel>)cache.Get(CacheKey);

                    if (!string.IsNullOrEmpty(id))
                    {
                        // Update data based on id
                        PatientModel patient = patients.Where(x => x.id == Convert.ToInt32(id)).FirstOrDefault();

                        if (patient != null)
                        {
                            patient.Firstname = patientInfo.Firstname;
                            patient.Lastname = patientInfo.Lastname;
                            patient.City = patientInfo.City;
                            patient.Active = patientInfo.Active;
                        }
                        else
                        {
                            Console.WriteLine($"No data found for id: {id}");
                            return null;
                        }

                        // Add data to cache
                        cache.Add(CacheKey, patients, DateTimeOffset.Now.AddMinutes(10)); // Expires after 10 minutes
                    }
                }

                return await GetPatientsData(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InsertPatientData: {ex.Message} {ex.StackTrace}");
                return null;
            }

        }


        public async Task<bool> RemovePatientData(string id)
        {
            try
            {
                // Create a MemoryCache instance
                MemoryCache cache = MemoryCache.Default;

                // Check if cache already contains the key
                if (cache.Contains(CacheKey))
                {
                    // Get data from cache
                    List<PatientModel> patients = (List<PatientModel>)cache.Get(CacheKey);

                    if (!string.IsNullOrEmpty(id))
                    {
                        // Update data based on id
                        PatientModel patient = patients.Where(x => x.id == Convert.ToInt32(id)).FirstOrDefault();

                        if (patient != null)
                        {
                            patients.Remove(patient);
                        }
                        else
                        {
                            Console.WriteLine($"No data found for id: {id}");
                            return false;
                        }

                        // Add data to cache
                        cache.Add(CacheKey, patients, DateTimeOffset.Now.AddMinutes(10)); // Expires after 10 minutes
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RemovePatientData: {ex.Message} {ex.StackTrace}");
                return false;
            }

        }
    }
}
