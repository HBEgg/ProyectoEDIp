using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoEDIp.GenericStructures
{
    public class PriorityQueueG<T> : ICloneable, IEnumerable<T>
    {
        public PQNodeG<T> Root;
        public int PatientsNumber;
        public PriorityQueueG<T> queueCopy;

        public PriorityQueueG()
        {
            PatientsNumber = 0;
        }

        public bool IsEmpty()
        {
            return Root == null ? true : false;
        }

        public bool IsFull()
        {
            return PatientsNumber == 10 ? true : false;
        }



        public void AddPatient(string key, DateTime date, T patient, int priority)
        {
            var newNode = new PQNodeG<T>(key, date, patient, priority);
            if (IsEmpty())
            {
                Root = newNode;
                PatientsNumber = 1;
            }
            else
            {
                PatientsNumber++;
                var NewNodeFather = SearchLastNode(Root, 1);
                if (NewNodeFather.LeftSon != null)
                {
                    NewNodeFather.RightSon = newNode;
                    newNode.Father = NewNodeFather;
                    OrderDowntoUp(newNode);
                }
                else
                {
                    NewNodeFather.LeftSon = newNode;
                    newNode.Father = NewNodeFather;
                    OrderDowntoUp(newNode);
                }

            }
        }

        private void OrderDowntoUp(PQNodeG<T> current)
        {
            if (current.Father != null)
            {
                if (current.Priority < current.Father.Priority)
                {
                    ChangeNodes(current);
                }
                else if (current.Priority == current.Father.Priority)
                {
                    if (current.DPriority < current.Father.DPriority)
                    {
                        ChangeNodes(current);
                    }
                }
                OrderDowntoUp(current.Father);
            }
        }


        private void OrderUptoDown(PQNodeG<T> current)
        {
            if (current.RightSon != null && current.LeftSon != null)
            {
                if (current.LeftSon.Priority > current.RightSon.Priority)
                {
                    if (current.Priority > current.RightSon.Priority)
                    {
                        ChangeNodes(current.RightSon);
                        OrderUptoDown(current.RightSon);
                    }
                    else if (current.Priority == current.RightSon.Priority)
                    {
                        if (current.DPriority > current.RightSon.DPriority)
                        {
                            ChangeNodes(current.RightSon);
                            OrderUptoDown(current.RightSon);
                        }
                    }
                }
                else if (current.LeftSon.Priority < current.RightSon.Priority)
                {
                    if (current.Priority > current.LeftSon.Priority)
                    {
                        ChangeNodes(current.LeftSon);
                        OrderUptoDown(current.LeftSon);
                    }
                    else if (current.Priority == current.LeftSon.Priority)
                    {
                        if (current.DPriority > current.LeftSon.DPriority)
                        {
                            ChangeNodes(current.LeftSon);
                            OrderUptoDown(current.LeftSon);
                        }
                    }
                }
                else
                {
                    if (current.LeftSon.DPriority > current.RightSon.DPriority)
                    {
                        if (current.Priority > current.RightSon.Priority)
                        {
                            ChangeNodes(current.RightSon);
                            OrderUptoDown(current.RightSon);
                        }
                        else if (current.Priority == current.RightSon.Priority)
                        {
                            if (current.DPriority > current.RightSon.DPriority)
                            {
                                ChangeNodes(current.RightSon);
                                OrderUptoDown(current.RightSon);
                            }
                        }
                    }
                    else
                    {
                        if (current.Priority > current.LeftSon.Priority)
                        {
                            ChangeNodes(current.LeftSon);
                            OrderUptoDown(current.LeftSon);
                        }
                        else if (current.Priority == current.LeftSon.Priority)
                        {
                            if (current.DPriority > current.LeftSon.DPriority)
                            {
                                ChangeNodes(current.LeftSon);
                                OrderUptoDown(current.LeftSon);
                            }
                        }
                    }
                }
            }
            else if (current.RightSon != null)
            {
                if (current.Priority > current.RightSon.Priority)
                {
                    ChangeNodes(current.RightSon);
                    OrderUptoDown(current.RightSon);
                }
                else if (current.Priority == current.RightSon.Priority)
                {
                    if (current.DPriority > current.RightSon.DPriority)
                    {
                        ChangeNodes(current.RightSon);
                        OrderUptoDown(current.RightSon);
                    }
                }
            }
            else if (current.LeftSon != null)
            {
                if (current.Priority > current.LeftSon.Priority)
                {
                    ChangeNodes(current.LeftSon);
                    OrderUptoDown(current.LeftSon);
                }
                else if (current.Priority == current.LeftSon.Priority)
                {
                    if (current.DPriority > current.LeftSon.DPriority)
                    {
                        ChangeNodes(current.LeftSon);
                        OrderUptoDown(current.LeftSon);
                    }
                }
            }
        }


        private void ChangeNodes(PQNodeG<T> node)
        {
            var Priority1 = node.Priority;
            var Key1 = node.Key;
            var Date1 = node.DPriority;
            var Patient1 = node.Patient;
            node.Priority = node.Father.Priority;
            node.Key = node.Father.Key;
            node.DPriority = node.Father.DPriority;
            node.Patient = node.Father.Patient;
            node.Father.Priority = Priority1;
            node.Father.Key = Key1;
            node.Father.DPriority = Date1;
            node.Father.Patient = Patient1;

        }


        public PQNodeG<T> GetFirst()
        {
            if (Root == null)
            {
                return null;
            }
            PQNodeG<T> LastNode = SearchLastNode(Root, 1);
            PQNodeG<T> FirstNode = (PQNodeG<T>)Root.Clone();
            var LastNodeCopy = (PQNodeG<T>)LastNode.Clone();
            Root.Key = LastNodeCopy.Key;
            Root.Priority = LastNodeCopy.Priority;
            Root.Patient = LastNodeCopy.Patient;
            Root.DPriority = LastNodeCopy.DPriority;
            if (LastNode.Father == null)
            {
                Root = null;
                PatientsNumber--;
                return LastNode;
            }
            else
            {
                if (LastNode.Father.LeftSon == LastNode)
                {
                    LastNode.Father.LeftSon = null;
                }
                else
                {
                    LastNode.Father.RightSon = null;
                }
            }
            OrderUptoDown(Root);
            PatientsNumber--;
            return FirstNode;
        }


        private PQNodeG<T> SearchLastNode(PQNodeG<T> current, int number)
        {
            try
            {
                int previousn = PatientsNumber;
                if (previousn == number)
                {
                    return current;
                }
                else
                {
                    while (previousn / 2 != number)
                    {
                        previousn = previousn / 2;
                    }
                    if (previousn % 2 == 0)
                    {
                        if (current.LeftSon != null)
                        {
                            return SearchLastNode(current.LeftSon, previousn);
                        }
                        else
                        {
                            return current;
                        }
                    }
                    else
                    {
                        if (current.RightSon != null)
                        {
                            return SearchLastNode(current.RightSon, previousn);
                        }
                        else
                        {
                            return current;
                        }
                    }
                }
            }
            catch
            {
                return current;
            }

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var queueCopy = new PriorityQueueG<T>() { Root = this.Root, PatientsNumber = this.PatientsNumber };
            var current = queueCopy.Root;
            while (current != null)
            {
                yield return current.Patient;
                current = queueCopy.GetFirst();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}