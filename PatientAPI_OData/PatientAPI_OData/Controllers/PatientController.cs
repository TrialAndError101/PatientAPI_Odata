using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Newtonsoft.Json;
using PatientAPI_OData.Models;
using PatientAPI_OData.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PatientAPI_OData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public PatientController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        //incase you can't run this in swagger, please use postman
        // https://localhost:<port_here>/api/Patient

        [HttpGet]
        [EnableQuery]
        public ActionResult<List<PatientModel>> GetAllPatient()
        {
            Console.WriteLine("C# HTTP trigger AddPatients processed a request.");

            List<PatientModel> result = _cacheService.GetPatientsData().Result;

            if (result != null)
            {
                return new OkObjectResult(result);
            }
            else
            {
                Console.WriteLine("Error in getting patient data");
                return new BadRequestObjectResult("Error in getting patient data");
            }
        }

        //not necessary because we can Use OData query options to filter the data
        [HttpGet("{id}")]
        public async Task<ActionResult<List<PatientModel>>> GetPatientsById(string id)
        {
            Console.WriteLine("C# HTTP trigger AddPatients processed a request.");

            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult("Invalid id");
            }

            List<PatientModel> result = await _cacheService.GetPatientsData(id);

            if (result != null)
            {
                return new OkObjectResult(result);
            }
            else
            {
                Console.WriteLine("Error in getting patient data");
                return new BadRequestObjectResult("Error in getting patient data");
            }
        }

        //Run this endpoint first to insert data in the cache
        [HttpPost]
        public async Task<ActionResult<List<PatientModel>>> AddPatient([FromBody] PatientModel requestBody)
        {
            Console.WriteLine("C# HTTP trigger function AddPatients processed a request.");

            //string requestBody = await new StreamReader(req).ReadToEndAsync();


            try
            {
                //PatientModel data = JsonConvert.DeserializeObject<PatientModel>(requestBody);
                PatientModel data = requestBody;

                if (data == null)
                {
                    return new BadRequestObjectResult("Invalid data");
                }
                else
                {
                    string dataInputErrors = await _cacheService.DataInputErrors(data);

                    if (!string.IsNullOrEmpty(dataInputErrors))
                    {
                        return new BadRequestObjectResult(dataInputErrors);
                    }
                }

                bool result = await _cacheService.InsertPatientData(data);

                if (result)
                {
                    return new OkObjectResult("Patient data added successfully");
                }
                else
                {
                    return new BadRequestObjectResult("Error in adding patient data");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPatients: {ex.Message} {ex.StackTrace}");
                return new BadRequestObjectResult("Something went wrong.");
            }
        }


        //update patient
        [HttpPut("{id}")]
        public async Task<ActionResult<List<PatientModel>>> UpdatePatientById(string id, [FromBody]PatientModel requestBody)
        {
            Console.WriteLine("C# HTTP trigger function AddPatients processed a request.");

            try
            {
                PatientModel data = requestBody;

                if (data == null)
                {
                    return new BadRequestObjectResult("Invalid data");
                }
                else
                {
                    string dataInputErrors = await _cacheService.DataInputErrors(data);

                    if (!string.IsNullOrEmpty(dataInputErrors))
                    {
                        return new BadRequestObjectResult(dataInputErrors);
                    }
                }

                List<PatientModel> result = await _cacheService.UpdatePatientData(id, data);

                if (result != null)
                {
                    return new OkObjectResult(result);
                }
                else
                {
                    return new BadRequestObjectResult("Error in updating patient data");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPatients: {ex.Message} {ex.StackTrace}");
                return new BadRequestObjectResult("Something went wrong.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeletePatientById(string id)
        {
            Console.WriteLine("C# HTTP trigger function AddPatients processed a request.");

            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult("Invalid id");
            }

            bool result = await _cacheService.RemovePatientData(id);

            if (result)
            {
                return new OkObjectResult("Patient data deleted successfully");
            }
            else
            {
                return new BadRequestObjectResult("Error in deleting patient data");
            }
        }
    }
}
