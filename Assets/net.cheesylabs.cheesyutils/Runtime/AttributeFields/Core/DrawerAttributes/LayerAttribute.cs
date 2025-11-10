using System;

namespace CheesyUtils.CheesyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LayerAttribute : DrawerAttribute
    {
    }
}