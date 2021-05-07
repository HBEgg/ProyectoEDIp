using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace ProyectoEDIp.GenericStructures
{
    public class PQNodeG<T> : ICloneable
    {
        public PQNodeG<T> Father;
        public PQNodeG<T> RightSon;
        public PQNodeG<T> LeftSon;
        public T Patient;
        public string Key;
        public int Priority;
        public DateTime DatePriority;
        public PQNodeG(string key, DateTime Date, T patient, int priority)
        {
            Key = key;
            DatePriority = Date;
            Patient = patient;
            Priority = priority;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}