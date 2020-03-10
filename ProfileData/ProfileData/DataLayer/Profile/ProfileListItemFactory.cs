using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ProfileData.DataLayer.Profile
{
    public class ProfileListItemFactory<T> where T : ProfileListItem, new()
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