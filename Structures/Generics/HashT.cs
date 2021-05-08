using System;
using System.Collections.Generic;
using System.Text;

namespace Structures.Generics
{
    class HashT<T> where T : IComparable
    {
        public int Length;
        public HashNode<T>[] HashTable;

        public HashT(int length)
        {
            Length = length;
            HashTable = new HashNode<T>[Length];
        }

        public void Insert(T InsertV, string key)
        {
            HashNode<T> T1 = new HashNode<T>();
            T1.Value = InsertV;
            T1.Key = key;
            int code = GetCode(T1.Key);
            if (HashTable[code] != null)
            {
                HashNode<T> Aux = HashTable[code];
                while (Aux.Next != null)
                {
                    Aux = Aux.Next;
                }
                Aux.Next = T1;
                T1.Previous = Aux;
            }
            else
            {
                HashTable[code] = T1;
            }
        }

        public HashNode<T> Search(string searchedKey)
        {
            int code = GetCode(searchedKey);

            if (HashTable[code] != null)
            {

                if (HashTable[code].Key != searchedKey)
                {
                    HashNode<T> Aux = HashTable[code];
                    while (Aux.Key != searchedKey && Aux.Next != null)
                    {
                        Aux = Aux.Next;
                    }
                    if (Aux.Key == searchedKey)
                    {
                        return Aux;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return HashTable[code];
                }
            }
            else
            {
                return null;
            }
        }
        private int GetCode(string Key)
        {
            int length = Key.Length;
            int code = 0;
            for (int i = 0; i < length; i++)
            {
                code += Convert.ToInt32(Key.Substring(i, 1));
            }
            code = (code * 7) % Length;
            return code;
        }

        public List<HashNode<T>> GetAsNodes()
        {
            var returnList = new List<HashNode<T>>();
            var currentNode = new HashNode<T>();
            foreach (var task in HashTable)
            {
                currentNode = task;
                while (currentNode != null)
                {
                    returnList.Add(currentNode);
                    currentNode = currentNode.Next;
                }
            }
            return returnList;
        }


        public List<T> GetFilteredList(Func<T, bool> predicate)
        {
            List<T> FilteredList = new List<T>();
            var currentNode = new HashNode<T>();
            foreach (var task in HashTable)
            {
                currentNode = task;
                while (currentNode != null)
                {
                    if (predicate(currentNode.Value))
                    {
                        FilteredList.Add(currentNode.Value);
                    }
                    currentNode = currentNode.Next;
                }
            }
            return FilteredList;
        }


        public HashNode<T> GetT(int pos, int block)
        {
            return HashTable[pos + block];
        }
    }
}
