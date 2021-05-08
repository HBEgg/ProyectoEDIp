using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ProyectoEDIp.Helpers;
using ProyectoEDIp.GenericStructures;
using ProyectoEDIp.Models;

namespace ProyectoEDIp.Controllers
{

    public class VaccineController : Controller
    {
        public static bool FirstTime = true;
        public ActionResult Index()
        {
            if (FirstTime)
            {
                LoadRegistrationCenterByDepartment();
                FirstTime = false;
            }
            return View();
        }
        
        private void LoadRegistrationCenterByDepartment()
        {
            AddRegistrationCenter("Alta Verapaz");
            AddRegistrationCenter("Baja Verapaz");
            AddRegistrationCenter("Chimaltenango");
            AddRegistrationCenter("El Peten");
            AddRegistrationCenter("El Progreso");
            AddRegistrationCenter("Escuintla");
            AddRegistrationCenter("Guatemala");
            AddRegistrationCenter("Huehuetenango");
            AddRegistrationCenter("Izabal");
            AddRegistrationCenter("Jalapa");
            AddRegistrationCenter("Jutiapa");
            AddRegistrationCenter("Quetzaltenango");
            AddRegistrationCenter("Quiche");
            AddRegistrationCenter("Retalhuleu");
            AddRegistrationCenter("Sacatepequez");
            AddRegistrationCenter("San Marcos");
            AddRegistrationCenter("Santa Rosa");
            AddRegistrationCenter("Solola");
            AddRegistrationCenter("Suchitepequez");
            AddRegistrationCenter("Totonicapan");
            AddRegistrationCenter("Zacapa");
        }

        private void AddRegistrationCenter(string RegistrationCenter)
        {
            var newRC = new RegistrationCenter()
            {
                CenterName = RegistrationCenter,
                VaccinesUsed = 0,
                InfectedQueue = new PriorityQueueG<Patientinfo>(),
                PatientsQueue = new PriorityQueueG<Patientinfo>()
            };
            newRC.GetDepartments();
            Storage.Instance.RegistrationCenters.Add(newRC);
        }
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            var option = collection["Option"];
            switch (option)
            {
                case "AddCase":
                    return RedirectToAction("AddCase");
                case "RegistrationCenterList":
                    return RedirectToAction("RegistrationCenterList");
                case "PatientsList":
                    return RedirectToAction("PatientsList");
                case "Statistics":
                    return RedirectToAction("Statistics");
            }
            return View();
        }


        public ActionResult NewCase()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewCase(FormCollection collection)
        {
            try
            {
                if (HasIncorrectCharacter(collection["Name"]) || HasIncorrectCharacter(collection["LastName"]) || HasIncorrectCharacter(collection["Municipio"]) || HasIncorrectCharacter(collection["Symptoms"]))
                {
                    ModelState.AddModelError("Name", "Por favor ingrese datos no numéricos en los campos pertinentes.");
                    return View("NewCase");
                }
                if (int.Parse(collection["Age"]) < 0 || int.Parse(collection["Age"]) > 122)
                {
                    ModelState.AddModelError("Age", "Por favor ingrese una edad válida");
                    return View("NewCase");
                }
                else if (collection["Department"] == "Seleccionar Departamento")
                {
                    ModelState.AddModelError("Department", "Por favor seleccione un departamento");
                    return View("NewCase");
                }
                foreach (var patient in Storage.Instance.PatientsHash.GetAsNodes())
                {
                    if (patient.Value.DPI == collection["DPI"])
                    {
                        ModelState.AddModelError("CUI", "Un paciente con el mismo dpi ya ha sido ingresado en el sistema. Ingrese otro paciente.");
                        return View("NewCase");
                    }
                }
                var newPatient = new Patientinfo()
                {
                    Name = collection["Name"],
                    LastName = collection["LastName"],
                    Departamento = collection["Departamento"],
                    RegistrationCenter = GetRegistrationCenter(collection["Department"]),
                    Municipio = collection["Municipio"],
                    Symptoms = collection["Symptoms"],
                    DPI = collection["DPI"],
                    Age = int.Parse(collection["Age"]),
                    Infected = false,
                    Appointment = DateTime.Parse(collection["Appointment"]),
                    Status = "Sospechoso"
                };
                newPatient.PriorityAssignment();
                var infoPatient = new Patientinfo()
                {
                    Name = newPatient.Name,
                    LastName = newPatient.LastName,
                    RegistrationCenter = newPatient.RegistrationCenter,
                    DPI = newPatient.DPI,
                    Age = newPatient.Age,
                    Appointment = newPatient.Appointment,
                    Priority = newPatient.Priority,
                    Status = newPatient.Status
                };
                foreach (var patient in Storage.Instance.PatientsHash.GetAsNodes())
                {
                    if (patient.Value.Name == collection["Name"])
                    {
                        Storage.Instance.RepeatedNames.Add(patient.Value.Name);
                    }
                    if (patient.Value.LastName == collection["LastName"])
                    {
                        Storage.Instance.RepeatedLNames.Add(patient.Value.LastName);
                    }
                }
                Storage.Instance.PatientsHash.Insert(newPatient, newPatient.DPI);
                Storage.Instance.PatientsByName.AddPatient(Patientinfo, Patientinfo.Comparebyname);
                Storage.Instance.PatientsByLastName.AddPatient(structurePatient, PatientStructure.CompareByLastName);
                Storage.Instance.PatientsByCUI.AddPatient(structurePatient, PatientStructure.CompareByCUI);
                Storage.Instance.CountryStatistics.Suspicious++;
                SendToHospital(structurePatient);
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("InfectionDescription", "Por favor asegúrese de haber llenado todos los campos correctamente.");
                return View("NewCase");
            }
        }

        private bool HasIncorrectCharacter(string data)
        {
            try
            {
                var num = int.Parse(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetBool(string data)
        {
            if (data == "true,false")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SendToRegistrationCenter(Patientinfo patient)
        {
            var hospital = Storage.Instance.RegistrationCenters.First(x => x.CenterName == patient.RegistrationCenter);
            hospital.PatientsQueue.AddPatient(patient.DPI, patient.Appointment, patient, patient.Priority);
        }

        private string GetRegistrationCenter(string department)
        {
            foreach (var RC in Storage.Instance.RegistrationCenters)
            {
                if (RC.Departments.Contains(department))
                {
                    return RC.CenterName;
                }
            }
            return null;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contacts page.";

            return View();
        }

    }
}