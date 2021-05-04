using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoEDIp.Models; 

namespace ProyectoEDIp.Helpers
{
    public class Storage
    {
        private static Storage _instance = null; 
        public static Storage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Storage();
                }
                return _instance; 
            }
        }
        public List<Patientinfo> PatientList = new List<Patientinfo>();
        public List<Patientinfo> InformationList = new List<Patientinfo>();
        public int Patientswvaccineshot = 0;
        public int Patientpvaccineshot = 0;
        public int Patient2vaccineshot = 0; 
        public Dictionary<string, Patientinfo> PatientNameinfo = new Dictionary<string, Patientinfo>();
        public Dictionary<string, Patientinfo> PatientLastNameinfo = new Dictionary<string, Patientinfo>();
        public Dictionary<string, Patientinfo> PatientIDList = new Dictionary<string, Patientinfo>(); 
    }
}