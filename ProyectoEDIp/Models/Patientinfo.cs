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
        public string Name { get; set; }
        public string LastName { get; set; }
        public int DPI { get; set; }
        public int Age { get; set; }
        public string Departamento { get; set; }
        public string Municipio { get; set; }
        public int Time { get; set; }

        [Required(ErrorMessage = "{0} Es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}", ApplyFormatInEditMode =true)]
        public System.DateTime Appointment { get; set; }
        public object patientinfo { get; private set; }

        public bool Save () 
	    {	  
            try
            {
                Helpers.Storage.Instance.VaccineList.Add(this);
                retun true;
            }
            catch
            {
                return false;
            }

        }
	public int CompareTo(object obj)
        {
            return Name.CompareTo(((Patientinfo))obj).Name); 
        }
        public static Comparison<Patientinfo> Comparebyname = delegate (Patientinfo patientinfo1, Patientinfo patientinfo2)
        {
            return patientinfo1.Name.CompareTo(patientinfo2.Name);
        };
        public static Comparison<Patientinfo> ComparebyLastName = delegate (Patientinfo patientinfo1, Patientinfo patientinfo2)
        {
            return patientinfo1.LastName.CompareTo(patientinfo2.LastName)
        };
        public static Comparison<Patientinfo> ComparebyID = delegate (Patientinfo patientinfo1, Patientinfo patientinfo2)
        {
            return patientinfo1.DPI.CompareTo(patientinfo2.DPI);
        };

    }
}