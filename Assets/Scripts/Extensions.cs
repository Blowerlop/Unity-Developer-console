using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    namespace Extensions
    {
        public static class GameObjectExtensions
        {
            public static void DestroyChildren(this UnityEngine.GameObject gameObject)
            {
                gameObject.transform.DestroyChildren();
            }
        }
    
        public static class TransformExtensions
        {
            public static void DestroyChildren(this UnityEngine.Transform transform)
            {
                int childCount = transform.childCount;
                
                for (int i = childCount - 1; i >= 0; i--)
                {
                    UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
        
        public static class CollectionsExtensions
        {
            public static T Next<T>(this IReadOnlyCollection<T> list, ref int index)
            {
                return Next((IList<T>) list, ref index);
            }
            
            public static T Previous<T>(this IReadOnlyCollection<T> list, ref int index)
            {
                if (list == null)
                {
                    throw new NullReferenceException();
                }
                if (list.Count == 0) return default;
                
                index--;
                if (index < 0)
                {
                    index = list.Count - 1;
                }

                return Previous((IList<T>) list, ref index);
            }
            
            public static T Next<T>(this IList<T> list, ref int index)
            {
                if (list == null)
                {
                    throw new NullReferenceException();
                }
                if (list.Count == 0) return default;
                
                index++;
                if (index >= list.Count)
                {
                    index = 0;
                }

                return list[index];
            }
            
            public static T Previous<T>(this IList<T> list, ref int index)
            {
                if (list == null)
                {
                    throw new NullReferenceException();
                }
                if (list.Count == 0) return default;
                
                index--;
                if (index < 0)
                {
                    index = list.Count - 1;
                }

                return list[index];
            }
        }
        
        public static class StringExtensions
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns>If found, return character index, otherwise return -1</returns>
            public static int Find(this string str, char character)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == character) return i;
                }

                return -1;
            }
            
            public static int Count(this string str, char character)
            {
                int count = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == character) count++;
                }

                return count;
            }
        }
    }
}