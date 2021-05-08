using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations; 

namespace ProyectoEDIp.Models
{
    public class Patientinfo :IComparable
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "LastName")]
        public string LastName { get; set; }
        [Display(Name = "DPI")]
        public string DPI { get; set; }
        public int Age { get; set; }
        public int Priority { get; set; }
        public string RegistrationCenter { get; set; }
        public string Status { get; set; }
        [Display(Name = "Departamento")]
        public string Departamento { get; set; }
        [Display(Name = "Municipio")]
        public string Municipio { get; set; }
        public int Time { get; set; }
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }
        public bool Infected { get; set; }

        [Required(ErrorMessage = "{0} Es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}", ApplyFormatInEditMode =true)]
        [Display(Name = "Appointment")]
        public System.DateTime Appointment { get; set; }
        public object patientinfo { get; private set; }

        public bool Save () 
	    {	  
            try
            {
                Helpers.Storage.Instance.PatientList.Add(this);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public void PriorityAssignment()
        {
            if (Age >=55)
            {
                Priority = 1;
            }
            else if (Age < 16 && Age < 50)
            {
                Priority = 2;
            }
            else if (Age < 1 && Age <= 16)
            {
                Priority = 3;
            }
        }
	    public int CompareTo(object obj)
        {
            return Name.CompareTo(((Patientinfo)obj).Name); 
        }
        public static Comparison<Patientinfo> Comparebyname = delegate (Patientinfo patientinfo1, Patientinfo patientinfo2)
        {
            return patientinfo1.Name.CompareTo(patientinfo2.Name);
        };
        public static Comparison<Patientinfo> ComparebyLastName = delegate (Patientinfo patientinfo1, Patientinfo patientinfo2)
        {
            return patientinfo1.LastName.CompareTo(patientinfo2.LastName);
        };
        public static Comparison<Patientinfo> ComparebyID = delegate (Patientinfo patientinfo1, Patientinfo patientinfo2)
        {
            return patientinfo1.DPI.CompareTo(patientinfo2.DPI);
        };
        public static Comparison<Patientinfo> ComparebyPriority = delegate (Patientinfo patientinfo1, Patientinfo patientinfo2)
        {
            return patientinfo1.Priority.CompareTo(patientinfo2.Priority);
        };

    }
}