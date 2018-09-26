using System;

namespace KellermanSoftware.Serialization
{
    /// <summary>
    /// This field will not be saved or loaded from the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class IgnoreColumnAttribute : Attribute
    {	
    }
}