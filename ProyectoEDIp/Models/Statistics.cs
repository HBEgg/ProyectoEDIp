using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoEDIp.Models
{
    public class Statistics
    {
        [Display(Name = "Infected")]
        public int Infected { get; set; }

        [Display(Name = "Suspicious")]
        public int Suspicious { get; set; }
        [Display(Name = "Percentage")]
        public double Percentage { get; set; }
        [Display(Name = "Vaccinated")]
        public int Vaccinated { get; set; }

        public Statistics()
        {
            Infected = 0;
            Suspicious = 0;
            Percentage = 0;
            Vaccinated = 0;
        }

        public void GetPercentage()
        {
            if (Suspicious > 0)
            {
                Percentage = Math.Round(((double)Infected / Suspicious) * 100, 2);
            }
        }
    }
}