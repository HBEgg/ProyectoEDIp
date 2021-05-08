using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProyectoEDIp.Helpers;
using ProyectoEDIp.GenericStructures;
using ProyectoEDIp.Models;
using DotNet.Highcharts;

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
                if (HasIncorrectCharacter(collection["Name"]) || HasIncorrectCharacter(collection["LastName"]) || HasIncorrectCharacter(collection["Municipio"]) || HasIncorrectCharacter(collection["Symptoms"]) || HasIncorrectCharacter(collection["InfectionDescription"]))
                {
                    ModelState.AddModelError("Name", "Por favor ingrese datos no numéricos en los campos pertinentes.");
                    return View("RegisterPatient");
                }
                if (int.Parse(collection["Age"]) < 0 || int.Parse(collection["Age"]) > 122)
                {
                    ModelState.AddModelError("Age", "Por favor ingrese una edad válida");
                    return View("RegisterPatient");
                }
                else if (collection["Departamento"] == "Seleccionar Departamento")
                {
                    ModelState.AddModelError("Departamento", "Por favor seleccione un departamento");
                    return View("RegisterPatient");
                }
                foreach (var patient in Storage.Instance.PatientsHash.GetAsNodes())
                {
                    if (patient.Value.DPI == collection["DPI"])
                    {
                        ModelState.AddModelError("CUI", "Un paciente con el mismo dpi ya ha sido ingresado en el sistema. Ingrese otro paciente.");
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
                    InfectionDescription = collection["InfectionDescription"],
                    NotVaccinated = false,
                    Appointment = DateTime.Parse(collection["Appointment"]),
                    Status = "Paciente"
                };
                newPatient.SetEffectivenessChance(GetBool(collection["PFizer"]), GetBool(collection["Moderna"]), GetBool(collection["Johnson"]));
                newPatient.PriorityAssignment();
                var infoPatient = new Patientinfo()
                {
                    Name = newPatient.Name,
                    LastName = newPatient.LastName,
                    RegistrationCenter = newPatient.RegistrationCenter,
                    DPI = newPatient.DPI,
                    Age = newPatient.Age,
                    NotVaccinated = newPatient.NotVaccinated,
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
                foreach (var node in Storage.Instance.PatientsByName.GetList())
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

        //[HttpPost]
        //public ActionResult Create(PatientModel patientmodel)
        //{
        //    if (Storage.Instance.PatientNameinfo.ContainsKey(patientmodel.Name))
        //    {
        //        return View();
        //    }
        //    patientmodel.Status = "Sospechoso";
        //    Storage.Instance.PatientNameinfo.Add(patientmodel.Name, patientmodel);
        //    Storage.Instance.PatientLastNameinfo.Add(patientmodel.LastName, patientmodel);
        //    Storage.Instance.PatientIDList.Add(patientmodel.DPI, patientmodel);
        //    return RedirectToAction("Create");

        //}
        //public ActionResult FindPatient(string name, string lastname, string DPI)
        //{

        //    // enfermosinfo.Find(x => x.)
        //    if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(lastname) && String.IsNullOrEmpty(DPI))
        //    {
        //        if (Storage.Instance.PatientNameinfo.Count == 0)
        //        {
        //            return RedirectToAction("Create");
        //        }
        //        return View(Storage.Instance.PatientNameinfo.Values.ToList());
        //    }
        //    else if (!String.IsNullOrEmpty(name))
        //    {
        //        //var paciente = arbolpaciente.Buscar(new Enfermos() { Nombre = nombre }, arbolpaciente.Raiz, Enfermos.CompararPorNombre);
        //        if (Storage.Instance.PatientNameinfo.ContainsKey(name))
        //        {
        //            return View(Storage.Instance.PatientNameinfo.Values.Where(x => x.Name == name));
        //        }
        //    }
        //    else if (!String.IsNullOrEmpty(lastname))
        //    {
        //        //var paciente = arbolpaciente.Buscar(new Enfermos() { Apellido = apellido }, arbolpaciente.Raiz, Enfermos.CompararPorApellido
        //        if (Storage.Instance.PatientLastNameinfo.ContainsKey(lastname))
        //        {
        //            return View(Storage.Instance.PatientLastNameinfo.Values.Where(x => x.LastName == lastname));
        //        }
        //    }
        //    else
        //    {
        //        //var paciente = arbolpaciente.Buscar(new Enfermos() { Dpi = DPI }, arbolpaciente.Raiz, Enfermos.CompararPorDPI);
        //        if (Storage.Instance.PatientIDList.ContainsKey(DPI))
        //        {
        //            var paciente = Storage.Instance.PatientIDList[DPI];
        //            return View(Storage.Instance.PatientIDList.Values.Where(x => x.DPI == DPI));
        //        }
        //    }
        //    return View(Storage.Instance.PatientIDList.Values.ToList());
        //}

        public ActionResult ChartsVaccination()
        {
            Highcharts ColumnChart = new Highcharts("ColumnChart");
            ColumnChart.InitChart(new DotNet.Highcharts.Options.Chart()
            {
                Type = DotNet.Highcharts.Enums.ChartTypes.Column,
                BackgroundColor = new DotNet.Highcharts.Helpers.BackColorOrGradient(System.Drawing.Color.BlanchedAlmond),
                Style = "fontWeight: 'Bold' , FontSize: '17ptx'",
                BorderColor = System.Drawing.Color.Azure,
                BorderRadius = 0,
                BorderWidth = 2,
            });
            ColumnChart.SetTitle(new DotNet.Highcharts.Options.Title()
            {
                Text = "Gráfica de Vacunación"
            });
            ColumnChart.SetSubtitle(new DotNet.Highcharts.Options.Subtitle() // subtitulo y descripción de la gráfica 
            {
                Text = "Cantidad de personas vacunadas"
            });
            ColumnChart.SetXAxis(new DotNet.Highcharts.Options.XAxis()     //obtiene la información de los datos que irán en el eje de las x
            {
                Type = DotNet.Highcharts.Enums.AxisTypes.Category,
                Title = new DotNet.Highcharts.Options.XAxisTitle() { Text = "Casos", Style = "fontWeight : 'bold' , fontSize: '17ptx'" },
                Categories = new[] { "Ingreso de Pacientes", "Personas vacunadas" }

            });
            ColumnChart.SetYAxis(new DotNet.Highcharts.Options.YAxis()  //obtiene la información de los datos que irán en el eje de las y
            {
                Title = new DotNet.Highcharts.Options.YAxisTitle()
                {
                    Text = "Cantidades",
                    Style = "fontWeight: 'bold', fontSize: '17ptx'"
                },
                ShowFirstLabel = true,  //muestra que los datos si se han tomado con validez 
                ShowLastLabel = true,
                Min = 0 //empieza desde el valor de 0
            });
            ColumnChart.SetLegend(new DotNet.Highcharts.Options.Legend
            {
                Enabled = true,
                BorderColor = System.Drawing.Color.Aquamarine,
                BorderRadius = 6,
                BackgroundColor = new DotNet.Highcharts.Helpers.BackColorOrGradient(System.Drawing.ColorTranslator.FromHtml("#ADE6D8")) //color verde analogo
            });
            ColumnChart.SetSeries(new DotNet.Highcharts.Options.Series[]
            {
                new DotNet.Highcharts.Options.Series
                {
                    Name = "Ingreso de Contagiados",
                    Data = new DotNet.Highcharts.Helpers.Data(new object[]{Storage.Instance.cantVacunados}) //se puede acceder a lo que contiene la lista contagiados, pero se puede hacer una lista de contagiados etc
                },
                new DotNet.Highcharts.Options.Series()
                {
                    Name = "Ingreso de Sospechosos",
                    Data = new DotNet.Highcharts.Helpers.Data (new object[]{Storage.Instance.cantPendiente}) //se agrega la cantidad de recuperados que hay recordar que se puede referenciar los datos que ya se han obtenido
                }
            });
            return View(ColumnChart);
        }
    
    private int GetMultiplier(string RC)
        {
            switch (RC)
            {
                case "Alta Verapaz":
                    return 1;
                case "Baja Verapaz":
                    return 2;
                case "Chimaltenango":
                    return 3;
                case "Escuintla":
                    return 4;
                case "El Peten":
                    return 5;
                case "El Progreso":
                    return 1;
                case "Quetzaltenango":
                    return 2;
                case "Petén":
                    return 3;
                case "Guatemala":
                    return 4;
                case "Huehuetenango":
                    return 5;
                case "Quiche":
                    return 1;
                case "Izabal":
                    return 2;
                case "Jalapa":
                    return 3;
                case "Jutiapa":
                    return 4;
                case "San Marcos":
                    return 5;
                case "Retalhuleu":
                    return 1;
                case "Sacatepequez":
                    return 2;
                case "Santa Rosa":
                    return 3;
                case "Suchitepequez":
                    return 4;
                case "Zacapa":
                    return 5;
                case "Totonicapan":
                    return 4;
            }
            return -1;
        }

        public ActionResult Test(string registrationCenter)
        {
            var RC = Storage.Instance.RegistrationCenters.Find(x => x.CenterName == registrationCenter);
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
                    patient.Status = "NoVacunado";
                    patient.NotVaccinated = true;
                    Storage.Instance.PatientsHash.Search(patient.DPI).Value.PriorityAssignment();
                    patient.PriorityAssignment();
                    Storage.Instance.PatientsByDPI.ChangeValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    Storage.Instance.CountryStatistics.Vaccinated++;
                    Storage.Instance.RegistrationCenters.Find(x => x.CenterName == patient.RegistrationCenter).VaccinationQueue.AddPatient(patient.DPI, patient.Appointment, patient, patient.Priority);
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente no habia sido vacunado." });
                }
                else
                {
                    patient.Status = "Vacunado";
                    Storage.Instance.PatientsByDPI.ChangeValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente ha sido vacunado ya antes" });
                }
            }
            else if (RC.VaccinationQueue.Root != null)
            {
                if (RC.VaccinationQueue.Root.Patient.Priority < RC.PatientsQueue.Root.Patient.Priority)
                {
                    if (RC.NoVaccines())
                    {
                        return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "Ya no quedan más vacunas, por favor conseguir más." });
                    }
                }
                else
                {
                    var patient = RC.VaccinationQueue.GetFirst().Patient;
                    var infected = Storage.Instance.PatientsHash.Search(patient.DPI).Value.VaccinationTest();
                    if (infected)
                    {
                        patient.Status = "NoVacunado";
                        patient.NotVaccinated = true;
                        Storage.Instance.PatientsHash.Search(patient.DPI).Value.PriorityAssignment();
                        patient.PriorityAssignment();
                        Storage.Instance.PatientsByDPI.ChangeValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                        Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                        Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                        Storage.Instance.CountryStatistics.Suspicious--;
                        Storage.Instance.CountryStatistics.Vaccinated++;
                        Storage.Instance.RegistrationCenters.Find(x => x.CenterName == patient.RegistrationCenter).PatientsQueue.AddPatient(patient.DPI, patient.Appointment, patient, patient.Priority);
                        return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente no habia sido vacunado." });
                    }
                    else
                    {
                        patient.Status = "Vacunado";
                        Storage.Instance.PatientsByDPI.ChangeValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                        Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                        Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                        Storage.Instance.CountryStatistics.Suspicious--;
                        return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente ha sido vacunado ya antes" });
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
                    patient.Status = "NoVacunado";
                    patient.NotVaccinated = true;
                    Storage.Instance.PatientsHash.Search(patient.DPI).Value.PriorityAssignment();
                    patient.PriorityAssignment();
                    Storage.Instance.PatientsByDPI.ChangeValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
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
                            var node = Storage.Instance.VaccineHash.GetT(i, GetMultiplier(registrationCenter));
                            if (node != null)
                            {
                                Storage.Instance.RegistrationCenters.First(x => x.CenterName == RC.CenterName).VaccinesList.Add(node.Value);
                            }
                        }
                        Storage.Instance.RegistrationCenters.First(x => x.CenterName == RC.CenterName).VaccinesUsed = Storage.Instance.RegistrationCenters.First(x => x.CenterName == RC.CenterName).VaccinesList.Count();
                    }
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente no ha sido vacunado pero si ha sido realocado para otra cita" });
                }
                else
                {
                    patient.Status = "Vaccinated";
                    Storage.Instance.PatientsByDPI.ChangeValue(patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByName.ChangeValue(patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
                    Storage.Instance.PatientsByLastName.ChangeValue(patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
                    Storage.Instance.CountryStatistics.Suspicious--;
                    return RedirectToAction("RegistrationCenter", new { name = RC.CenterName, advice = "El paciente ya había sido vacunado antes" });
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
            Patient.NotVaccinated = false;
            Storage.Instance.PatientsHash.Search(Patient.DPI).Value.Status = "Recuperado";
            Storage.Instance.PatientsHash.Search(Patient.DPI).Value.NotVaccinated = false;
            Storage.Instance.PatientsByDPI.ChangeValue(Patient, Storage.Instance.PatientsByDPI.Root, Patientinfo.ComparebyID, Patientinfo.ComparebyID);
            Storage.Instance.PatientsByName.ChangeValue(Patient, Storage.Instance.PatientsByName.Root, Patientinfo.Comparebyname, Patientinfo.ComparebyID);
            Storage.Instance.PatientsByLastName.ChangeValue(Patient, Storage.Instance.PatientsByLastName.Root, Patientinfo.ComparebyLastName, Patientinfo.ComparebyID);
            Storage.Instance.CountryStatistics.Infected--;
            Storage.Instance.CountryStatistics.Vaccinated++;
            var RC = Storage.Instance.RegistrationCenters.First(x => x.CenterName == Patient.RegistrationCenter);
            if (RC.VaccinationQueue.Root != null)
            {
                var patient = RC.VaccinationQueue.GetFirst().Patient;
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