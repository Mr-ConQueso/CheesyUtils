using System;

namespace CheesyUtils.AttributeFields
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredFieldAttribute : Attribute { }
}
