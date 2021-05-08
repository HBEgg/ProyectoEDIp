using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations; 

namespace ProyectoEDIp.Models
{
    public class Patientinfo :IComparable
    {
        public string DPI { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "LastName")]
        public string LastName { get; set; }
        [Display(Name = "Age")]
        public int Age { get; set; }
        public int Priority { get; set; }
        public string RegistrationCenter { get; set; }
        public string Status { get; set; }
        public bool NotVaccinated { get; set; }
        [Display(Name = "Appointment")]
        public DateTime Appointment { get; set; }

        public int CompareTo(object obj)
        {
            return this.DPI.CompareTo(((Patientinfo)obj).DPI);
        }
        public void PriorityAssignment()
        {
            if (Age >=55)
            {
                if (NotVaccinated)
                {
                    Priority = 1;
                }
                else
                {
                    Priority = 4;
                }

            }
            else if (Age < 16 && Age < 55)
            {
                if (NotVaccinated)
                {
                    Priority = 3;
                }
                else
                {
                    Priority = 6;
                }
            }
            else if (Age < 1 && Age <= 16)
            {
                if (NotVaccinated)
                {
                    Priority = 5;
                }
                else
                {
                    Priority = 8;
                }
            }
            else
            {
                if (NotVaccinated)
                {
                    Priority = 2;
                }
                else
                {
                    Priority = 6;
                }
            }
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