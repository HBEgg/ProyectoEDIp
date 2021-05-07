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
        public PriorityQueueG<Patientinfo> InfectedQueue { get; set; }
        public List<Patientinfo> PatientsList { get; set; }
        public List<Patientinfo> InfectedList { get; set; }
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
                case "Capital":
                    Departments.Add("Guatemala");
                    Departments.Add("Sacatepéquez");
                    Departments.Add("Chimaltenango");
                    break;
                case "Quetzaltenango":
                    Departments.Add("Quetzaltenango");
                    Departments.Add("Totonicapán");
                    Departments.Add("San Marcos");
                    Departments.Add("Huehuetenango");
                    break;
                case "Petén":
                    Departments.Add("Petén");
                    Departments.Add("Alta Verapaz");
                    Departments.Add("Baja verapaz");
                    Departments.Add("Sololá");
                    Departments.Add("Quiché");
                    break;
                case "Escuintla":
                    Departments.Add("Escuintla");
                    Departments.Add("Santa Rosa");
                    Departments.Add("Jutiapa");
                    Departments.Add("Suchitepéquez");
                    Departments.Add("Retalhuleu");
                    break;
                case "Oriente":
                    Departments.Add("Izabal");
                    Departments.Add("Zacapa");
                    Departments.Add("Chiquimula");
                    Departments.Add("Jalapa");
                    Departments.Add("El Progreso");
                    break;
            }
        }
        public bool NoVaccines()
        {
            if (VaccinesUsed==10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InfectedQueueFull()
        {
            return InfectedQueue.IsFull();
        }

        public bool PatientsQueueFull()
        {
            return PatientsQueue.IsFull();
        }
    }
}