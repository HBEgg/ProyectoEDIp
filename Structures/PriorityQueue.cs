using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structures
{
    class PriorityQueue<T> : ICloneable, IEnumerable<T>
    {
        public PQNode<T> Root;
        public int PatientsNumber;
        public PriorityQueue<T> queueCopy;

        public PriorityQueue()
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
            var newNode = new PQNode<T>(key, date, patient, priority);
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

        private void OrderDowntoUp(PQNode<T> current)
        {
            if (current.Father != null)
            {
                if (current.Priority < current.Father.Priority)
                {
                    ChangeNodes(current);
                }
                else if (current.Priority == current.Father.Priority)
                {
                    if (current.DatePriority < current.Father.DatePriority)
                    {
                        ChangeNodes(current);
                    }
                }
                OrderDowntoUp(current.Father);
            }
        }


        private void OrderUptoDown(PQNode<T> current)
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
                        if (current.DatePriority > current.RightSon.DatePriority)
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
                        if (current.DatePriority > current.LeftSon.DatePriority)
                        {
                            ChangeNodes(current.LeftSon);
                            OrderUptoDown(current.LeftSon);
                        }
                    }
                }
                else
                {
                    if (current.LeftSon.DatePriority > current.RightSon.DatePriority)
                    {
                        if (current.Priority > current.RightSon.Priority)
                        {
                            ChangeNodes(current.RightSon);
                            OrderUptoDown(current.RightSon);
                        }
                        else if (current.Priority == current.RightSon.Priority)
                        {
                            if (current.DatePriority > current.RightSon.DatePriority)
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
                            if (current.DatePriority > current.LeftSon.DatePriority)
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
                    if (current.DatePriority > current.RightSon.DatePriority)
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
                    if (current.DatePriority > current.LeftSon.DatePriority)
                    {
                        ChangeNodes(current.LeftSon);
                        OrderUptoDown(current.LeftSon);
                    }
                }
            }
        }


        private void ChangeNodes(PQNode<T> node)
        {
            var Priority1 = node.Priority;
            var Key1 = node.Key;
            var Date1 = node.DatePriority;
            var Patient1 = node.Patient;
            node.Priority = node.Father.Priority;
            node.Key = node.Father.Key;
            node.DatePriority = node.Father.DatePriority;
            node.Patient = node.Father.Patient;
            node.Father.Priority = Priority1;
            node.Father.Key = Key1;
            node.Father.DatePriority = Date1;
            node.Father.Patient = Patient1;

        }


        public PQNode<T> GetFirst()
        {
            if (Root == null)
            {
                return null;
            }
            PQNode<T> LastNode = SearchLastNode(Root, 1);
            PQNode<T> FirstNode = (PQNode<T>)Root.Clone();
            var LastNodeCopy = (PQNode<T>)LastNode.Clone();
            Root.Key = LastNodeCopy.Key;
            Root.Priority = LastNodeCopy.Priority;
            Root.Patient = LastNodeCopy.Patient;
            Root.DatePriority = LastNodeCopy.DatePriority;
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


        private PQNode<T> SearchLastNode(PQNode<T> current, int number)
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
            var queueCopy = new PriorityQueue<T>() { Root = this.Root, PatientsNumber = this.PatientsNumber };
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
