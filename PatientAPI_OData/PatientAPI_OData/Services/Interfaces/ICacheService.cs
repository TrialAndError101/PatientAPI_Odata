using PatientAPI_OData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAPI_OData.Services.Interfaces
{
    public interface ICacheService
    {
        Task<bool> InsertPatientData(PatientModel patientInfo);
        Task<List<PatientModel>> GetPatientsData(string id = null);
        Task<List<PatientModel>> UpdatePatientData(string id, PatientModel patientInfo);
        Task<string> DataInputErrors(PatientModel patientInfo);
        Task<bool> RemovePatientData(string id);
    }
}
