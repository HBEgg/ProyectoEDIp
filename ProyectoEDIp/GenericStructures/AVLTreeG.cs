using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoEDIp.GenericStructures
{
    public class AVLTreeG<T> where T : IComparable
    {
        public AVLNode<T> Root;
        public bool Empty { get { return Root == null; } }
        int height = 0;
        public int newValue = 0;
        public void Add(T item, AVLNode<T> node, Comparison<T> comparison)
        {
            if (Root != null)
            {
                if (comparison.Invoke(item, node.Patient) < 0)
                {
                    if (node.Right != null)
                    {
                        Add(item, node.Left, comparison);
                    }
                    else
                    {
                        node.Left = new AVLNode<T>(item);
                    }
                }
                else
                {
                    if (node.Right != null)
                    {
                        Add(item, node.Right, comparison);
                    }
                    else
                    {
                        node.Right = new AVLNode<T>(item);
                    }
                }
            }
            else
            {
                Root = new AVLNode<T>(item);
            }
        }
        public void CreateTree(List<T> patientslist, Comparison<T> comparison)
        {
            foreach (var item in patientslist)
            {
                this.Add(item, Root, comparison);
            }
        }
        public T Search(T patient, AVLNode<T> node, Comparison<T> comparison)
        {
            if (Root != null)
            {
                if (comparison.Invoke(patient, node.Patient) < 0)
                {
                    if (node.Left != null)
                    {
                        return Search(patient, node.Left, comparison);
                    }
                    else
                    {
                        return default;
                    }
                }
                else if (comparison.Invoke(patient, node.Patient) > 0)
                {
                    if (node.Right != null)
                    {
                        return Search(patient, node.Right, comparison);
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    return node.Patient;
                }
            }
            else
            {
                return default;
            }
        }
    }
}