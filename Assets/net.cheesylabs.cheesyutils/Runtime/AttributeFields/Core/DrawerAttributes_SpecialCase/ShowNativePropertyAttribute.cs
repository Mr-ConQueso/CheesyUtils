using System;

namespace CheesyUtils.CheesyAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowNativePropertyAttribute : SpecialCaseDrawerAttribute
    {
    }
}
