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
        private List<AVLNode<T>> ResultList;
        public void AddPatient(T v, Comparison<T> comparison)
        {
            var newN = new AVLNode<T>() { Patient = v };
            Insert(Root, newN, comparison);

        }

        private void Insert(AVLNode<T> current, AVLNode<T> newN, Comparison<T> comparison)
        {

            if (current == null && current == Root)
            {
                current = newN;
                Root = current;
            }
            else if (comparison.Invoke(newN.Patient, current.Patient) < 0)
            {
                if (current.Left == null)
                {
                    current.Left = newN;
                    newN.Parent = current;
                    Balance(current);
                }
                else
                {
                    Insert(current.Left, newN, comparison);
                }
            }
            else
            {
                if (current.Right == null)
                {
                    current.Right = newN;
                    newN.Parent = current;
                    Balance(current);
                }
                else
                {
                    Insert(current.Right, newN, comparison);
                }
            }
        }

        public void Delete(AVLNode<T> current, AVLNode<T> v, Comparison<T> comparison)
        {

            if (comparison.Invoke(v.Patient, current.Patient) == 0)
            {
                if (current.Left != null)
                {
                    current.Patient = ReplaceLeft(current.Left).Patient;
                    Delete(current.Left, ReplaceLeft(current.Left), comparison);
                }
                else if (current.Right != null)
                {
                    current.Patient = ReplaceRight(current.Right).Patient;
                    Delete(current.Right, ReplaceRight(current.Right), comparison);
                }
                else
                {
                    var compareNode = current;
                    if (current.Parent.Left == compareNode)
                    {
                        current.Parent.Left = null;
                    }
                    else
                    {
                        current.Parent.Right = null;
                    }
                    Balance(current.Parent);
                }
            }
            else if (comparison.Invoke(v.Patient, current.Patient) < 0)
            {
                Delete(current.Left, v, comparison);
            }
            else
            {
                Delete(current.Right, v, comparison);
            }
        }

        private AVLNode<T> ReplaceLeft(AVLNode<T> current)
        {
            if (current.Right != null)
            {
                return ReplaceLeft(current.Right);
            }
            else
            {
                return current;
            }
        }
        private AVLNode<T> ReplaceRight(AVLNode<T> current)
        {
            if (current.Left != null)
            {
                return ReplaceRight(current.Left);
            }
            else
            {
                return current;
            }
        }
        private void Balance(AVLNode<T> node)
        {
            if (node.GetBalanceIndex() == -2)
            {
                if (node.Left.GetBalanceIndex() == 1)
                {
                    LeftRot(node.Left);
                    RightRot(node);
                }
                else
                {
                    RightRot(node);
                }
            }
            else if (node.GetBalanceIndex() == 2)
            {
                if (node.Right.GetBalanceIndex() == -1)
                {
                    RightRot(node.Right);
                    LeftRot(node);
                }
                else
                {
                    LeftRot(node);
                }
            }
            if (node.Parent != null)
            {
                Balance(node.Parent);
            }
        }


        private void RightRot(AVLNode<T> node)
        {
            AVLNode<T> newLeft = node.Left.Right;
            node.Left.Right = node;
            node.Left.Parent = node.Parent;
            if (node.Parent != null)
            {
                if (node.Parent.Right == node)
                {
                    node.Parent.Right = node.Left;
                }
                else
                {
                    node.Parent.Left = node.Left;
                }
            }
            node.Parent = node.Left;
            node.Left = newLeft;
            if (newLeft != null)
            {
                newLeft.Parent = node;
            }

            if (node.Parent.Parent == null)
            {
                Root = node.Parent;
            }
        }



        private void LeftRot(AVLNode<T> node)
        {
            AVLNode<T> newRight = node.Right.Left;
            node.Right.Left = node;
            node.Right.Parent = node.Parent;
            if (node.Parent != null)
            {
                if (node.Parent.Right == node)
                {
                    node.Parent.Right = node.Right;
                }
                else
                {
                    node.Parent.Left = node.Right;
                }
            }
            node.Parent = node.Right;
            node.Left = newRight;
            if (newRight != null)
            {
                newRight.Parent = node;
            }

            if (node.Parent.Parent == null)
            {
                Root = node.Parent;
            }
        }

        public List<AVLNode<T>> ExtractList()
        {
            ResultList = new List<AVLNode<T>>();
            if (Root != null)
            {
                InOrder(Root);
            }
            return ResultList;
        }

        private void InOrder(AVLNode<T> current)
        {
            if (current.Left != null)
            {
                InOrder(current.Left);
            }
            ResultList.Add(current);
            if (current.Right != null)
            {
                InOrder(current.Right);
            }
        }
        //Search all nodes that match with the search
        public List<T> Search(T Patient, AVLNode<T> node, Comparison<T> comparison)
        {
            List<T> Patients = new List<T>();
            if (comparison.Invoke(Patient, node.Patient) == 0)
            {
                Patients.Add(node.Patient);
                List<T> RepeatedValues = Search(node.Right, Patient, comparison);
                if (RepeatedValues.Count > 0)
                {
                    foreach (var item in RepeatedValues)
                    {
                        Patients.Add(item);
                    }
                }
                RepeatedValues = Search(node.Left, Patient, comparison);
                if (RepeatedValues.Count > 0)
                {
                    foreach (var item in RepeatedValues)
                    {
                        Patients.Add(item);
                    }
                }
                return Patients;
            }
            else if (comparison.Invoke(Patient, node.Patient) > 0)
            {
                if (node.Right != null)
                {
                    return Search(Patient, node.Right, comparison);
                }
                else
                {
                    return new List<T>();
                }
            }
            else
            {
                if (node.Left != null)
                {
                    return Search(Patient, node.Left, comparison);
                }
                else
                {
                    return new List<T>();
                }
            }
        }
        //Recursive search of patient value
        public AVLNode<T> Search(Comparison<T> comparison, T Patient, AVLNode<T> node)
        {
            if (node != null)
            {
                if (comparison.Invoke(Patient, node.Patient) < 0)
                {
                    return Search(comparison, Patient, node.Left);
                }
                else if (comparison.Invoke(Patient, node.Patient) > 0)
                {
                    return Search(comparison, Patient, node.Right);
                }
                else
                {
                    return node;
                }
            }
            return null;
        }
        //Recursive node search
        private List<T> Search(AVLNode<T> node, T Patient, Comparison<T> comparison)
        {
            List<T> Patients = new List<T>();
            List<T> RepeatedValues = new List<T>();
            if (node != null)
            {
                if (comparison.Invoke(Patient, node.Patient) == 0)
                {
                    Patients.Add(node.Patient);
                    RepeatedValues = Search(node.Right, Patient, comparison);
                    if (RepeatedValues.Count > 0)
                    {
                        foreach (var item in RepeatedValues)
                        {
                            Patients.Add(item);
                        }
                    }
                    RepeatedValues = Search(node.Left, Patient, comparison);
                    if (RepeatedValues.Count > 0)
                    {
                        foreach (var item in RepeatedValues)
                        {
                            Patients.Add(item);
                        }
                    }
                    return Patients;
                }
                else if (comparison.Invoke(Patient, node.Patient) > 0)
                {
                    RepeatedValues = Search(node.Right, Patient, comparison);
                    if (RepeatedValues.Count > 0)
                    {
                        foreach (var item in RepeatedValues)
                        {
                            Patients.Add(item);
                        }
                    }
                    return Patients;
                }
                else
                {
                    RepeatedValues = Search(node.Left, Patient, comparison);
                    if (RepeatedValues.Count > 0)
                    {
                        foreach (var item in RepeatedValues)
                        {
                            Patients.Add(item);
                        }
                    }
                    return Patients;
                }
            }
            else
            {
                return Patients;
            }

        }

        public void EditValue(T newP, AVLNode<T> node, Comparison<T> comparison1, Comparison<T> comparison2)
        {
            if (node != null)
            {
                if (comparison1.Invoke(newP, node.Patient) < 0)
                {
                    EditValue(newP, node.Left, comparison1, comparison2);
                }
                else if (comparison1.Invoke(newP, node.Patient) == 0)
                {
                    if (comparison2.Invoke(newP, node.Patient) == 0)
                    {
                        node.Patient = newP;
                    }
                    else
                    {
                        if (node.Left != null)
                        {
                            EditValue(newP, node.Left, comparison1, comparison2);
                        }
                        if (node.Right != null)
                        {
                            EditValue(newP, node.Right, comparison1, comparison2);
                        }
                    }
                }
                else
                {
                    EditValue(newP, node.Right, comparison1, comparison2);
                }
            }
        }

        //

    }
}