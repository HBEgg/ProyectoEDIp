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
                case "San Marcos":
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
            else if (RC.NoVaccines())
            {
                var patient = RC.PatientsQueue.GetFirst().Patient;
                var vaccinated = Storage.Instance.PatientsHash.Search(patient.DPI).Value.VaccinationTest();
                if (vaccinated)
                {
                    patient.Status = "NotVaccinated";
                    patient.Vaccinated = true;
                    Storage.Instance.PatientsHash.Search(patient.DPI).Value.PriorityAssignment();
                    patient.PriorityAssignment();
                    Storage.Instance.PatientsByDPI.EditValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByName.EditValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByLastName.EditValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    Storage.Instance.CountryStatistics.Infected++;
                    Storage.Instance.RegistrationCenters.Find(x => x.CenterName == patient.RegistrationCenter).VaccinationQueue.AddPatient(patient.DPI, patient.Appointment, patient, patient.Priority);
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente ha resultado vaccinado." });
                }
                else
                {
                    patient.Status = "Vaccinated";
                    Storage.Instance.PatientsByDPI.EditValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByName.EditValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByLastName.EditValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "La prueba del paciente ha salido negativa, se ha descartado su caso" });
                }
            }
            else if (RC.VaccinationQueue.Root != null)
            {
                if (RC.VaccinationQueue.Root.Patient.Priority < RC.PatientsQueue.Root.Patient.Priority)
                {
                    if (RC.NoVaccines())
                    {
                        return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "Hay un paciente que necesita ser atendido antes de que realice más pruebas de COVID-19, por favor libere una cama." });
                    }
                }
                else
                {
                    var patient = RC.VaccinationQueue.GetFirst().Patient;
                    var infected = Storage.Instance.PatientsHash.Search(patient.DPI).Value.VaccinationTest();
                    if (infected)
                    {
                        patient.Status = "NotVaccinated";
                        patient.Vaccinated = true;
                        Storage.Instance.PatientsHash.Search(patient.DPI).Value.PriorityAssignment();
                        patient.PriorityAssignment();
                        Storage.Instance.PatientsByDPI.EditValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                        Storage.Instance.PatientsByName.EditValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                        Storage.Instance.PatientsByLastName.EditValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                        Storage.Instance.CountryStatistics.Suspicious--;
                        Storage.Instance.CountryStatistics.Infected++;
                        Storage.Instance.RegistrationCenters.Find(x => x.CenterName == patient.RegistrationCenter).PatientsQueue.AddPatient(patient.DPI, patient.Appointment, patient, patient.Priority);
                        return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente ha resultado contagiado." });
                    }
                    else
                    {
                        patient.Status = "Vaccinated";
                        Storage.Instance.PatientsByDPI.EditValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                        Storage.Instance.PatientsByName.EditValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                        Storage.Instance.PatientsByLastName.EditValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                        Storage.Instance.CountryStatistics.Suspicious--;
                        return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "La prueba del paciente ha salido negativa, se ha descartado su caso" });
                    }
                }
            }
            else
            {
                if (RC.PatientsQueue.Root == null)
                {
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "No hay pacientes esperando para realizar la prueba." });
                }
                var patient = RC.PatientsQueue.GetFirst().Patient;
                var infected = Storage.Instance.PatientsHash.Search(patient.DPI).Value.VaccinationTest();
                if (infected)
                {
                    patient.Status = "NotVaccinated";
                    patient.Vaccinated = true;
                    Storage.Instance.PatientsHash.Search(patient.DPI).Value.PriorityAssignment();
                    patient.PriorityAssignment();
                    Storage.Instance.PatientsByDPI.EditValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByName.EditValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByLastName.EditValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    Storage.Instance.CountryStatistics.Infected++;
                    if (RC.NoVaccines())
                    {
                        Storage.Instance.RegistrationCenters.Find(x => x.CenterName == patient.RegistrationCenter).VaccinationQueue.AddPatient(patient.DPI, patient.Appointment, patient, patient.Priority);
                    }
                    else
                    {
                        Storage.Instance.VaccineHash.Insert(new Vaccines() { Patient = patient, Availability = "No Disponible" }, patient.DPI, GetMultiplier(patient.RegistrationCenter));
                        Storage.Instance.RegistrationCenters.First(x => x.CenterName == RC.CenterName).VaccinesList = new List<Vaccines>();
                        for (int i = 0; i < 10; i++)
                        {
                            var node = Storage.Instance.VaccineHash.GetT(i, GetMultiplier(regiCenter));
                            if (node != null)
                            {
                                Storage.Instance.RegistrationCenters.First(x => x.CenterName == RC.CenterName).VaccinesList.Add(node.Value);
                            }
                        }
                        Storage.Instance.RegistrationCenters.First(x => x.CenterName == RC.CenterName).VaccinesUsed = Storage.Instance.RegistrationCenters.First(x => x.CenterName == RC.CenterName).VaccinesList.Count();
                    }
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente ha resultado confirmado." });
                }
                else
                {
                    patient.Status = "Vaccinated";
                    Storage.Instance.PatientsByDPI.EditValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByName.EditValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByLastName.EditValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "La prueba del paciente ha salido negativa, se ha descartado su caso" });
                }
            }
            return RedirectToAction("RegistrationCenter");
        }
        public ActionResult GetVaccinated(string code)
        {
            var Patient = new Patientinfo() { DPI = code };
            Patient = Storage.Instance.PatientsByDPI.Search(Patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID).First();
            Storage.Instance.VaccineHash.Delete(new Vaccines() { Availability = "No Disponible", Patient = Patient }, Patient.DPI, GetMultiplier(Patient.RegistrationCenter));
            Patient.Status = "Recuperado";
            Patient.Vaccinated = false;
            Storage.Instance.PatientsHash.Search(Patient.DPI).Value.Status = "Recuperado";
            Storage.Instance.PatientsHash.Search(Patient.DPI).Value.Vaccinated = false;
            Storage.Instance.PatientsByDPI.EditValue(Patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
            Storage.Instance.PatientsByName.EditValue(Patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
            Storage.Instance.PatientsByLastName.EditValue(Patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
            Storage.Instance.CountryStatistics.Infected--;
            Storage.Instance.CountryStatistics.Vaccinated++;
            var hosp = Storage.Instance.RegistrationCenters.First(x => x.CenterName == Patient.RegistrationCenter);
            if (hosp.VaccinationQueue.Root != null)
            {
                var patient = hosp.VaccinationQueue.GetFirst().Patient;
                Storage.Instance.VaccineHash.Insert(new Vaccines() { Patient = patient, Availability = "No Disponible" }, patient.DPI, GetMultiplier(patient.RegistrationCenter));
            }
            Storage.Instance.RegistrationCenters.First(x => x.CenterName == Patient.RegistrationCenter).VaccinesList = new List<Vaccines>();
            for (int i = 0; i < 10; i++)
            {
                var node = Storage.Instance.VaccineHash.GetT(i, GetMultiplier(Patient.RegistrationCenter));
                if (node != null)
                {
                    Storage.Instance.RegistrationCenters.First(x => x.CenterName == Patient.RegistrationCenter).VaccinesList.Add(node.Value);
                }
            }
            Storage.Instance.RegistrationCenters.First(x => x.CenterName == Patient.RegistrationCenter).VaccinesUsed = Storage.Instance.RegistrationCenters.First(x => x.CenterName == Patient.RegistrationCenter).VaccinesList.Count();
            return RedirectToAction("RegistrationCenter", new { name = Patient.RegistrationCenter });
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