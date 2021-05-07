using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoEDIp.Models
{
    public class Vaccines :IComparable
    {
        [Display(Name = "Availability")]
        public string Availability { get; set; }
        public Patientinfo Patient { get; set; }


        public int CompareTo(object obj)
        {
            return this.Patient.DPI.CompareTo(((Vaccines)obj).Patient.DPI);
        }


        public void NoVaccine()
        {
            Patient = null;
            Availability = "Available";
        }
    }
}