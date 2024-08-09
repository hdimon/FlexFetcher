namespace FlexFetcher.Utils;

internal class TypeHelper
{
    public static bool IsInstanceOfGenericType(object obj, Type genericTypeDefinition)
    {
        var objectType = obj.GetType();
        var baseType = objectType;

        while (baseType != null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                var typeArguments = baseType.GetGenericArguments();
                var constructedGenericType = genericTypeDefinition.MakeGenericType(typeArguments);

                if (constructedGenericType.IsAssignableFrom(objectType))
                {
                    return true;
                }
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
}