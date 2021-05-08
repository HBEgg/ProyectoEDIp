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
        public static Storage _instance = null; 
        public static Storage Instance
        {
            get
            {
                if (_instance == null) _instance = new Storage();
                return _instance; 
            }
        }
        public HashG<Vaccines> VaccineHash = new HashG<Vaccines>(50);
        public HashG<Patientinfo> PatientsHash = new HashG<Patientinfo>(100);
        public AVLTreeG<Patientinfo> PatientsByName = new AVLTreeG<Patientinfo>();
        public AVLTreeG<Patientinfo> PatientsByLastName = new AVLTreeG<Patientinfo>();
        public AVLTreeG<Patientinfo> PatientsByDPI = new AVLTreeG<Patientinfo>();
        public List<RegistrationCenter> RegistrationCenters = new List<RegistrationCenter>();
        public Statistics CountryStatistics = new Statistics();
        public List<string> RepeatedNames = new List<string>();
        public List<string> RepeatedLNames = new List<string>();
    }
}