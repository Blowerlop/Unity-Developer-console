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

        public static class StringExtensions
        {
            public enum ESearchOrder
            {
                Start,
                End
            }
            
            /// <summary>
            /// 
            /// </summary>
            /// <returns>If found, return character index, otherwise return -1</returns>
            public static int Find(this string str, ESearchOrder searchOrder, char character)
            {
                if (searchOrder == ESearchOrder.Start)
                {
                    return FindForward(str, character);
                }

                return FindBackward(str, character);
            }
            
            private static int FindForward(this string str, char character)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == character) return i;
                }

                return -1;
            }
            
            private static int FindBackward(this string str, char character)
            {
                for (int i = str.Length - 1; i >= 0; i--)
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