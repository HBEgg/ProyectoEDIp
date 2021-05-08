using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoEDIp.Helpers
{
    public class Singleton : ISingleton
    {
        private static Singleton _instance = null; 
        public static Singleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Singleton(); 
                }
                return _instance; 
            }
        }
        public CQueue CQueue { get; set;  }
        public CQueue[] Array { get; set; }
        public List<Patientsinfo> Patientsinf { get; set; }
    }
}