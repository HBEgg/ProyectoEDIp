using System;
using System.Collections.Generic;
using System.Text;

namespace Structures
{
    class AVLNode<T>
    {
        public T Patient { get; set; }
        public int index { get; set; }
        public AVLNode<T> Right { get; set; }
        public AVLNode<T> Left { get; set; }
        public AVLNode<T> Parent { get; set; }
        public int  height {get;set;}

        public AVLNode(T patient)
        {
            this.Patient = patient; 
        }

        public bool Root()
        {
            if (Parent !=null)
            {
                return false; 
            }
            else
            {
                return true; 
            }
        }

        public bool LeafVal()
        {
            if (Right ==null)
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }

        public bool RightLeafVal ()
        {
            if (Right !=null)
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }
        public bool LeftLeafVal ()
        {
            if (Left != null)
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }
        public bool PatientVer()
        {
            if (Patient !=null)
            {
                return true;
            }
            else
            {
                return false; 
            }
        }
    }

}
