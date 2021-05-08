using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoEDIp.Models;
using ProyectoEDIp.GenericStructures;

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
        public HashG<Vaccines> VaccineHash = new HashG<Vaccines>(50);
        public HashG<Patientinfo> PatientsHash = new HashG<Patientinfo>(100);
        public List<Patientinfo> PatientList = new List<Patientinfo>();
        public List<Patientinfo> InformationList = new List<Patientinfo>();
        public List<Patientinfo> VaccinatedPatientList = new List<Patientinfo>();
        public int Patientswvaccineshot = 0;
        public int Patientpvaccineshot = 0;
        public int Patient2vaccineshot = 0;
        public List<RegistrationCenter> RegistrationCenters = new List<RegistrationCenter>();
        public List<string> RepeatedNames = new List<string>();
        public List<string> RepeatedLNames = new List<string>();
        public Statistics CountryStatistics = new Statistics();
        public AVLTreeG<Patientinfo> PatientsByName = new AVLTreeG<Patientinfo>();
        public AVLTreeG<Patientinfo> PatientsByLastName = new AVLTreeG<Patientinfo>();
        public AVLTreeG<Patientinfo> PatientsByCUI = new AVLTreeG<Patientinfo>();
        public Dictionary<string, Patientinfo> PatientNameinfo = new Dictionary<string, Patientinfo>();
        public Dictionary<string, Patientinfo> PatientLastNameinfo = new Dictionary<string, Patientinfo>();
        public Dictionary<string, Patientinfo> PatientIDList = new Dictionary<string, Patientinfo>(); 
    }
}