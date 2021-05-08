using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProyectoEDIp.Helpers;
using ProyectoEDIp.GenericStructures;
using ProyectoEDIp.Models;

namespace ProyectoEDIp.Controllers
{

    public class VaccineController : Controller
    {
        //private readonly ISingleton _singleton;
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
                VaccinationQueue = new ProyectoEDIp.GenericStructures.PriorityQueueG<Patientinfo>(),
                PatientsQueue = new ProyectoEDIp.GenericStructures.PriorityQueueG<Patientinfo>()
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
                case "RegisterPatient":
                    return RedirectToAction("RegisterPatient");
                case "RegistrationCenterList":
                    return RedirectToAction("RegistrationCenterList");
                case "PatientsList":
                    return RedirectToAction("PatientsList");
                case "Statistics":
                    return RedirectToAction("Statistics");
            }
            return View();
        }


        public ActionResult RegisterPatient()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterPatient(FormCollection collection)
        {
            try
            {
                if (HasWrongCharacter(collection["Name"]) || HasWrongCharacter(collection["LastName"]) || HasWrongCharacter(collection["Municipio"]) || HasWrongCharacter(collection["Symptoms"]) || HasWrongCharacter(collection["Infection_Description"]))
                {
                    ModelState.AddModelError("Name", "Por favor ingrese datos no numéricos en los campos pertinentes.");
                    return View("RegisterPatient");
                }
                if (int.Parse(collection["Age"]) < 0 || int.Parse(collection["Age"]) > 122)
                {
                    ModelState.AddModelError("Age", "Por favor ingrese una edad válida");
                    return View("RegisterPatient");
                }
                else if (collection["Departmento"] == "Seleccionar Departamento")
                {
                    ModelState.AddModelError("Departmento", "Por favor seleccione un departamento");
                    return View("RegisterPatient");
                }
                foreach (var patient in Storage.Instance.PatientsHash.GetAsNodes())
                {
                    if (patient.Value.DPI == collection["DPI"])
                    {
                        ModelState.AddModelError("DPI", "Un paciente con el mismo dpi ya ha sido ingresado en el sistema. Ingrese otro paciente.");
                        return View("RegisterPatient");
                    }
                }
                var newPatient = new PatientModel()
                {
                    Name = collection["Name"],
                    LastName = collection["LastName"],
                    Departamento = collection["Departamento"],
                    RegistrationCenter = GetRegistrationCenter(collection["Departamento"]),
                    Municipio = collection["Municipio"],
                    Symptoms = collection["Symptoms"],
                    DPI = collection["DPI"],
                    Age = int.Parse(collection["Age"]),
                    Vaccinated = false,
                    Appointment = DateTime.Parse(collection["Appointment"]),
                    Status = "No Vacunado"
                };
                newPatient.SetEffectiivenessChance(GetBool(collection["PFizer"]), GetBool(collection["Moderna"]), GetBool(collection["Johnson"]));
                newPatient.PriorityAssignment();
                var infoPatient = new Patientinfo()
                {
                    Name = newPatient.Name,
                    LastName = newPatient.LastName,
                    RegistrationCenter = newPatient.RegistrationCenter,
                    DPI = newPatient.DPI,
                    Age = newPatient.Age,
                    Vaccinated =newPatient.Vaccinated,
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
                Storage.Instance.PatientsByName.AddPatient(infoPatient, Patientinfo.Comparebyname);
                Storage.Instance.PatientsByLastName.AddPatient(infoPatient, Patientinfo.ComparebyLastName);
                Storage.Instance.PatientsByDPI.AddPatient(infoPatient, Patientinfo.ComparebyID);
                Storage.Instance.CountryStatistics.Suspicious++;
                SendToRegistrationCenter(infoPatient);
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("InfectionDescription", "Por favor asegúrese de haber llenado todos los campos correctamente.");
                return View("RegisterPatient");
            }
        }

        private bool HasWrongCharacter(string data)
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
            var RC = Storage.Instance.RegistrationCenters.First(x => x.CenterName == patient.RegistrationCenter);
            RC.PatientsQueue.AddPatient(patient.DPI, patient.Appointment, patient, patient.Priority);
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

        public ActionResult PatientsList(int? page, string search, string criteria)
        {
            var patientsList = GetPatients(null, null);
            int pageSize = 10;
            int pageNumber = page ?? 1;
            if (criteria == "Seleccionar Criterio" && search != "")
            {
                TempData["Error"] = "Por favor escoja un criterio de búsqueda.";
                return View(patientsList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                patientsList = GetPatients(search, criteria);
            }
            if (patientsList.Count == 0)
            {
                TempData["Error"] = "No se ha encontrado ningún paciente que coincida con los datos ingresados.";
            }
            return View(patientsList.ToPagedList(pageNumber, pageSize));
        }

        private List<Patientinfo> GetPatients(string search, string criteria)
        {
            var list = new List<Patientinfo>();
            var patient = new Patientinfo();
            if (search != null && search != "")
            {
                switch (criteria)
                {
                    case "Name":
                        patient.Name = search;
                        if (Storage.Instance.RepeatedNames.Contains(patient.Name))
                        {
                            list = Storage.Instance.PatientsByName.Search(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname);
                        }
                        else
                        {
                            list.Add(Storage.Instance.PatientsByName.Search(Patientinfo.Comparebyname, patient, Storage.Instance.PatientsByName.Root).Patient);
                        }
                        break;
                    case "LastName":
                        patient.LastName = search;
                        if (Storage.Instance.RepeatedLNames.Contains(patient.LastName))
                        {
                            list = Storage.Instance.PatientsByLastName.Search(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName);
                        }
                        else
                        {
                            list.Add(Storage.Instance.PatientsByLastName.Search(Patientinfo.ComparebyLastName, patient, Storage.Instance.PatientsByLastName.Root).Patient);
                        }
                        break;
                    case "DPI":
                        patient.DPI = search;
                        list.Add(Storage.Instance.PatientsByDPI.Search(Patientinfo.ComparebyID, patient, Storage.Instance.PatientsByDPI.Root).Patient);
                        break;
                }
            }
            else
            {
                foreach (var node in Storage.Instance.PatientsByName.ExtractList())
                {
                    list.Add(node.Patient);
                }
            }
            return list;
        }

        public ActionResult Statistics()
        {
            Storage.Instance.CountryStatistics.GetPercentage();
            if (Storage.Instance.CountryStatistics != null)
            {
                return View(Storage.Instance.CountryStatistics);
            }
            else
            {
                return View(new Statistics());
            }
        }

        public ActionResult RegistrationCenterList()
        {
            return View(Storage.Instance.RegistrationCenters);
        }

        public ActionResult RegistrationCenter(string name, string advice)
        {
            var showRC = Storage.Instance.RegistrationCenters.Find(x => x.CenterName == name);
            var newqueue = new PriorityQueueG<Patientinfo>();
            showRC.VaccinationList = new List<Patientinfo>();
            showRC.PatientsList = new List<Patientinfo>();
            var queueClone = showRC.VaccinationQueue;
            var node = queueClone.GetFirst();
            while (node != null)
            {
                showRC.VaccinationList.Add(node.Patient);
                newqueue.AddPatient(node.Patient.DPI, node.Patient.Appointment, node.Patient, node.Patient.Priority);
                node = queueClone.GetFirst();
            }
            showRC.VaccinationQueue = newqueue;
            newqueue = new PriorityQueueG<Patientinfo>();
            queueClone = showRC.PatientsQueue;
            node = queueClone.GetFirst();
            while (node != null)
            {
                showRC.PatientsList.Add(node.Patient);
                newqueue.AddPatient(node.Patient.DPI, node.Patient.Appointment, node.Patient, node.Patient.Priority);
                node = queueClone.GetFirst();
            }
            showRC.PatientsQueue = newqueue;

            if (advice != "")
            {
                TempData["Error"] = advice;
            }
            return View(showRC);
        }


        private int GetMultiplier(string RC)
        {
            switch (RC)
            {
                case "Capital":
                    return 1;
                case "Quetzaltenango":
                    return 2;
                case "Petén":
                    return 3;
                case "Escuintla":
                    return 4;
                case "Oriente":
                    return 5;
            }
            return -1;
        }

        public ActionResult Test(string regiCenter)
        {
            var RC = Storage.Instance.RegistrationCenters.Find(x => x.CenterName == regiCenter);
            if (RC.VaccinationQueueFull())
            {
                return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "La cola de infectados está llena, por favor libere una cama antes de continuar." });
            }
            else if (RC.BedFull())
            {
                var patient = hosp.SuspiciousQueue.GetFirst().Patient;
                var infected = Storage.Instance.PatientsHash.Search(patient.CUI).Value.InfectionTest();
                if (infected)
                {
                    patient.Status = "Contagiado";
                    patient.IsInfected = true;
                    Storage.Instance.PatientsHash.Search(patient.CUI).Value.PriorityAssignment();
                    patient.PriorityAssignment();
                    Storage.Instance.PatientsByCUI.ChangeValue(patient, Storage.Instance.PatientsByCUI.Root, PatientStructure.CompareByCUI, PatientStructure.CompareByCUI);
                    Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, PatientStructure.CompareByName, PatientStructure.CompareByCUI);
                    Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, PatientStructure.CompareByLastName, PatientStructure.CompareByCUI);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    Storage.Instance.CountryStatistics.Infected++;
                    Storage.Instance.Hospitals.Find(x => x.HospitalName == patient.Hospital).InfectedQueue.AddPatient(patient.CUI, patient.ArrivalDate, patient, patient.Priority);
                    return RedirectToAction("Hospital", new { name = hosp.HospitalName, advice = "El paciente ha resultado contagiado." });
                }
                else
                {
                    patient.Status = "NoInfectado";
                    Storage.Instance.PatientsByCUI.ChangeValue(patient, Storage.Instance.PatientsByCUI.Root, PatientStructure.CompareByCUI, PatientStructure.CompareByCUI);
                    Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, PatientStructure.CompareByName, PatientStructure.CompareByCUI);
                    Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, PatientStructure.CompareByLastName, PatientStructure.CompareByCUI);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    return RedirectToAction("Hospital", new { name = hosp.HospitalName, advice = "La prueba del paciente ha salido negativa, se ha descartado su caso" });
                }
            }
            else if (hosp.InfectedQueue.Root != null)
            {
                if (hosp.InfectedQueue.Root.Patient.Priority < hosp.SuspiciousQueue.Root.Patient.Priority)
                {
                    if (hosp.BedFull())
                    {
                        return RedirectToAction("Hospital", new { name = hosp.HospitalName, advice = "Hay un paciente que necesita ser atendido antes de que realice más pruebas de COVID-19, por favor libere una cama." });
                    }
                }
                else
                {
                    var patient = hosp.InfectedQueue.GetFirst().Patient;
                    var infected = Storage.Instance.PatientsHash.Search(patient.CUI).Value.InfectionTest();
                    if (infected)
                    {
                        patient.Status = "Contagiado";
                        patient.IsInfected = true;
                        Storage.Instance.PatientsHash.Search(patient.CUI).Value.PriorityAssignment();
                        patient.PriorityAssignment();
                        Storage.Instance.PatientsByCUI.ChangeValue(patient, Storage.Instance.PatientsByCUI.Root, PatientStructure.CompareByCUI, PatientStructure.CompareByCUI);
                        Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, PatientStructure.CompareByName, PatientStructure.CompareByCUI);
                        Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, PatientStructure.CompareByLastName, PatientStructure.CompareByCUI);
                        Storage.Instance.CountryStatistics.Suspicious--;
                        Storage.Instance.CountryStatistics.Infected++;
                        Storage.Instance.Hospitals.Find(x => x.HospitalName == patient.Hospital).InfectedQueue.AddPatient(patient.CUI, patient.ArrivalDate, patient, patient.Priority);
                        return RedirectToAction("Hospital", new { name = hosp.HospitalName, advice = "El paciente ha resultado contagiado." });
                    }
                    else
                    {
                        patient.Status = "NoInfectado";
                        Storage.Instance.PatientsByCUI.ChangeValue(patient, Storage.Instance.PatientsByCUI.Root, PatientStructure.CompareByCUI, PatientStructure.CompareByCUI);
                        Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, PatientStructure.CompareByName, PatientStructure.CompareByCUI);
                        Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, PatientStructure.CompareByLastName, PatientStructure.CompareByCUI);
                        Storage.Instance.CountryStatistics.Suspicious--;
                        return RedirectToAction("Hospital", new { name = hosp.HospitalName, advice = "La prueba del paciente ha salido negativa, se ha descartado su caso" });
                    }
                }
            }
            else
            {
                if (hosp.SuspiciousQueue.Root == null)
                {
                    return RedirectToAction("Hospital", new { name = hosp.HospitalName, advice = "No hay pacientes esperando para realizar la prueba." });
                }
                var patient = hosp.SuspiciousQueue.GetFirst().Patient;
                var infected = Storage.Instance.PatientsHash.Search(patient.CUI).Value.InfectionTest();
                if (infected)
                {
                    patient.Status = "Contagiado";
                    patient.IsInfected = true;
                    Storage.Instance.PatientsHash.Search(patient.CUI).Value.PriorityAssignment();
                    patient.PriorityAssignment();
                    Storage.Instance.PatientsByCUI.ChangeValue(patient, Storage.Instance.PatientsByCUI.Root, PatientStructure.CompareByCUI, PatientStructure.CompareByCUI);
                    Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, PatientStructure.CompareByName, PatientStructure.CompareByCUI);
                    Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, PatientStructure.CompareByLastName, PatientStructure.CompareByCUI);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    Storage.Instance.CountryStatistics.Infected++;
                    if (hosp.BedFull())
                    {
                        Storage.Instance.Hospitals.Find(x => x.HospitalName == patient.Hospital).InfectedQueue.AddPatient(patient.CUI, patient.ArrivalDate, patient, patient.Priority);
                    }
                    else
                    {
                        Storage.Instance.BedHash.Insert(new Bed() { Patient = patient, Availability = "No Disponible" }, patient.CUI, GetMultiplier(patient.Hospital));
                        Storage.Instance.Hospitals.First(x => x.HospitalName == hosp.HospitalName).BedList = new List<Bed>();
                        for (int i = 0; i < 10; i++)
                        {
                            var node = Storage.Instance.BedHash.GetT(i, GetMultiplier(hospital));
                            if (node != null)
                            {
                                Storage.Instance.Hospitals.First(x => x.HospitalName == hosp.HospitalName).BedList.Add(node.Value);
                            }
                        }
                        Storage.Instance.Hospitals.First(x => x.HospitalName == hosp.HospitalName).BedsInUse = Storage.Instance.Hospitals.First(x => x.HospitalName == hosp.HospitalName).BedList.Count();
                    }
                    return RedirectToAction("Hospital", new { name = hosp.HospitalName, advice = "El paciente ha resultado confirmado." });
                }
                else
                {
                    patient.Status = "NoInfectado";
                    Storage.Instance.PatientsByCUI.ChangeValue(patient, Storage.Instance.PatientsByCUI.Root, PatientStructure.CompareByCUI, PatientStructure.CompareByCUI);
                    Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, PatientStructure.CompareByName, PatientStructure.CompareByCUI);
                    Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, PatientStructure.CompareByLastName, PatientStructure.CompareByCUI);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    return RedirectToAction("Hospital", new { name = hosp.HospitalName, advice = "La prueba del paciente ha salido negativa, se ha descartado su caso" });
                }
            }
            return RedirectToAction("Hospital");
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