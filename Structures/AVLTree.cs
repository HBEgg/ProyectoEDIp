using System;
using System.Collections.Generic;

namespace Structures
{
    public class AVLTree<T>
    {
        public AVLNode<T> Root { get; set; }
        public bool Empty { get { return Root == null; } }
        int height = 0;
        public int newvalue = 0; 
        public void Add (T item, AVLNode<T> node, Comparison<T> comparison)
        {
            if (Root != null)
            {
                if (comparison.Invoke(item,node.Patient)< 0) 
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
        public T Search(T patient, AVLNode<T> node, Comparison<T>comparison)
        {
            if (Root !=null)
            {
                if (comparison.Invoke(patient, node.Patient) < 0)
                {
                    if (node.Left !=null)
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
