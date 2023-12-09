

namespace Code.Helpers.GlobalExtensions
{
    public static class ObjectExtension
    {
        public static bool IsNull(this object nullableObject)
        {
            return nullableObject == null;
        }
        
        public static bool IsNotNull(this object nullableObject)
        {
            return nullableObject != null;
        }
    }
}