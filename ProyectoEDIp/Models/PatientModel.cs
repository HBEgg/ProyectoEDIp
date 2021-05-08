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
        public int Time { get; set; }
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }
        [Display(Name = "Infection_Description")]
        public string InfectionDescription { get; set; }
        public int EffectivenessChance { get; set; }
        public void SetEffectiivenessChance (bool PFizer, bool Moderna, bool Johnson)
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
                Vaccinated = true;
                Status = "Vaccinated";
                return true;
            }
            else
            {
                Status = "NotVaccinated";
                RegistrationCenter = null;
                return false;
            }
        }
    }
}