using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace ProyectoEDIp.GenericStructures
{
    public class HashNode<T> where T : IComparable
    {
        public string Key { get; set; }
        public T Value { get; set; }
        public HashNode<T> Previous { get; set; }
        public HashNode<T> Next { get; set; }
    }
}