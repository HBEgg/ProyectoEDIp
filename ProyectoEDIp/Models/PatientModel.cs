using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ProyectoEDIp.Models
{
    public class PatientModel : Patientinfo
    {
        [Display(Name = "Departamento")]
        public string Departamento { get; set; }
        [Display(Name = "Municipio")]
        public string Municipio { get; set; }
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }
        [Display(Name = "InfectionDescription")]
        public string InfectionDescription { get; set; }
        public int EffectivenessChance { get; set; }

        public void SetEffectivenessChance (bool PFizer, bool Moderna, bool Johnson)
        {
            EffectivenessChance = 10;
            if (PFizer)
            {
                EffectivenessChance += 80;
            }
            if (Moderna)
            {
                EffectivenessChance += 70;
            }
            if (Johnson)
            {
                EffectivenessChance += 85;
            }

        }

        public bool VaccinationTest()
        {
            Random RND = new Random();
            if (RND.Next(100) <= EffectivenessChance)
            {
                NotVaccinated = true;
                Status = "NotVaccinated";
                return true;
            }
            else
            {
                Status = "Vaccinated";
                RegistrationCenter = null;
                return false;
            }
        }
    }
}