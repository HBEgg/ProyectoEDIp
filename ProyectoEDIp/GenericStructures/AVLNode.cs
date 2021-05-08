using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoEDIp.GenericStructures
{
    public class AVLNode<T> where T : IComparable
    {
        public T Patient { get; set; }
        public AVLNode<T> Right { get; set; }
        public AVLNode<T> Left { get; set; }
        public AVLNode<T> Parent { get; set; }


        public int GetBalanceIndex()
        {
            if (this.Left != null && this.Right != null)
            {
                return this.Right.TreeHeight() - this.Left.TreeHeight();
            }
            else if (this.Left == null)
            {
                if (this.Right == null)
                {
                    return 0;
                }
                else
                {
                    return this.Right.TreeHeight();
                }
            }
            else
            {
                return this.Left.TreeHeight() * -1;
            }
        }



        public int TreeHeight()
        {
            if (this.Left == null && this.Right == null)
            {
                return 1;
            }
            else if (this.Left == null || this.Right == null)
            {
                if (this.Left == null)
                {
                    return this.Right.TreeHeight() + 1;
                }
                else
                {
                    return this.Left.TreeHeight() + 1;
                }
            }
            else
            {
                if (this.Left.TreeHeight() > this.Right.TreeHeight())
                {
                    return this.Left.TreeHeight() + 1;
                }
                else
                {
                    return this.    Right.TreeHeight() + 1;
                }
            }
        }
    }
}