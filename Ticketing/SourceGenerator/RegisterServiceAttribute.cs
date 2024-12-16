using System;

namespace SourceGenerator
{
    public enum LifeTime
    {
        Singleton,
        Scoped,
        Transient
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterServiceAttribute : Attribute
    {
        public Type ServiceType { get; }
        public LifeTime LifeTime { get; }

        public RegisterServiceAttribute(Type serviceType, LifeTime lifeTime)
        {
            ServiceType = serviceType;
            LifeTime = lifeTime;
        }
    }
}
