using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Differenti8.DataLayer.Profile
{
    public class ProfileEntryFactory<T> where T : ProfileEntry, new()
    {
        public T Create(XElement entry)
        {
            Type entityType = typeof(T);
            Type[] parameterTypes = new Type[] { typeof(XElement) };
            ConstructorInfo constructorInfo = entityType.GetConstructor(parameterTypes);
            object[] parameterValues = new object[] { entry };
            T entity = (T)constructorInfo.Invoke(parameterValues);
            return entity;
        }

        public T Create(object parent, XElement entry)
        {
            Type entityType = typeof(T);
            Type[] parameterTypes = new Type[] { typeof(object), typeof(XElement) };
            ConstructorInfo constructorInfo = entityType.GetConstructor(parameterTypes);
            object[] parameterValues = new object[] { parent, entry };
            T entity = (T)constructorInfo.Invoke(parameterValues);
            return entity;
        }
    }
}