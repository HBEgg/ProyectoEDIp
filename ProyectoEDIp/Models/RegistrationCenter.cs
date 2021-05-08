using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ProyectoEDIp.Helpers;
using ProyectoEDIp.GenericStructures;

namespace ProyectoEDIp.Models
{
    public class RegistrationCenter
    {
        [Display(Name = "RegistrationCenter")]
        public string CenterName { get; set; }
        public List<string> Departments { get; set; }
        public PriorityQueueG<Patientinfo> PatientsQueue { get; set; }
        public PriorityQueueG<Patientinfo> VaccinationQueue { get; set; }
        public List<Patientinfo> PatientsList { get; set; }
        public List<Patientinfo> VaccinationList { get; set; }
        [Display(Name = "VaccinesUsed")]
        public int VaccinesUsed { get; set; }
        public List<Vaccines> VaccinesList { get; set; }

        public RegistrationCenter()
        {
            VaccinesList = new List<Vaccines>();
        }

        public void GetDepartments()
        {
            Departments = new List<string>();
            switch(CenterName)
            {
                case "Guatemala":
                    Departments.Add("Guatemala");
                    break;
                case "Alta Verapaz":
                    Departments.Add("Alta Verapaz");
                    break;
                case "Baja Verapaz":
                    Departments.Add("Baja Verapaz");
                    break;
                case "Chimaltenango":
                    Departments.Add("Chimaltenango");
                    break;
                case "El Progreso":
                    Departments.Add("El Progreso");
                    break;
                case "Escuintla":
                    Departments.Add("Escuintla");
                    break;
                case "Huehuetenango":
                    Departments.Add("Huehuetenango");
                    break;
                case "Izabal":
                    Departments.Add("Izabal");
                    break;
                case "Jalapa":
                    Departments.Add("Jalapa");
                    break;
                case "Jutiapa":
                    Departments.Add("Jutiapa");
                    break;
                case "Petén":
                    Departments.Add("Petén");
                    break;
                case "Quetzaltenango":
                    Departments.Add("Quetzaltenango");
                    break;
                case "Quiché":
                    Departments.Add("Quiché");
                    break;
                case "Retalhuleu":
                    Departments.Add("Retalhuleu");
                    break;
                case "Sacatepéquez":
                    Departments.Add("Sacatepéquez");
                    break;
                case "San Marcos":
                    Departments.Add("San Marcos");
                    break;
                case "Santa Rosa":
                    Departments.Add("Santa Rosa");
                    break;
                case "Suchiitepequez":
                    Departments.Add("Suchiitepequez");
                    break;
                case "Quiquimula":
                    Departments.Add("Quiquimula");
                    break;
                case "Sololá":
                    Departments.Add("Sololá");
                    break;
                case "Totonicapán":
                    Departments.Add("Totonicapán");
                    break;
            }
        }
        public bool NoVaccines()
        {
            if (VaccinesUsed==3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VaccinationQueueFull()
        {
            return VaccinationQueue.IsFull();
        }

        public bool PatientsQueueFull()
        {
            return PatientsQueue.IsFull();
        }
    }
}