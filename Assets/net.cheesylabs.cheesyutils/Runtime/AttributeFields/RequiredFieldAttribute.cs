using System;

namespace CheesyUtils.CheesyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredFieldAttribute : Attribute { }
}
