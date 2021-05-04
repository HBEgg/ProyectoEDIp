using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoEDIp.Models
{
    public interface ISingleton
    {
        CQueue CQueue { get; set; }
        CQueue[] Array { get; set; }
        List<Patientinfo> patientsinf { get; set; }
    }
}