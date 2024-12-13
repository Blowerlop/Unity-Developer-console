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
    }
}