﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
    /// <summary>
    /// Reflection-based implementation of <see cref="IReflectionMethodInfo"/>.
    /// </summary>
    public class ReflectionMethodInfo : LongLivedMarshalByRefObject, IReflectionMethodInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionMethodInfo"/> class.
        /// </summary>
        /// <param name="method">The method to be wrapped.</param>
        public ReflectionMethodInfo(MethodInfo method)
        {
            MethodInfo = method;
        }

        /// <inheritdoc/>
        public bool IsAbstract
        {
            get { return MethodInfo.IsAbstract; }
        }

        /// <inheritdoc/>
        public bool IsPublic
        {
            get { return MethodInfo.IsPublic; }
        }

        /// <inheritdoc/>
        public bool IsStatic
        {
            get { return MethodInfo.IsStatic; }
        }

        /// <inheritdoc/>
        public MethodInfo MethodInfo { get; private set; }

        /// <inheritdoc/>
        public string Name
        {
            get { return MethodInfo.Name; }
        }

        /// <inheritdoc/>
        public ITypeInfo ReturnType
        {
            get { return Reflector.Wrap(MethodInfo.ReturnType); }
        }

        /// <inheritdoc/>
        public ITypeInfo Type
        {
            get { return Reflector.Wrap(MethodInfo.ReflectedType); }
        }

        /// <inheritdoc/>
        public IEnumerable<IAttributeInfo> GetCustomAttributes(string assemblyQualifiedAttributeTypeName)
        {
            return GetCustomAttributes(MethodInfo, assemblyQualifiedAttributeTypeName).ToList();
        }

        static IEnumerable<IAttributeInfo> GetCustomAttributes(MethodInfo method, string assemblyQualifiedAttributeTypeName)
        {
            var attributeType = Reflector.GetType(assemblyQualifiedAttributeTypeName);

            return GetCustomAttributes(method, attributeType, ReflectionAttributeInfo.GetAttributeUsage(attributeType));
        }

        static IEnumerable<IAttributeInfo> GetCustomAttributes(MethodInfo method, Type attributeType, AttributeUsageAttribute attributeUsage)
        {
            IEnumerable<IAttributeInfo> results =
                CustomAttributeData.GetCustomAttributes(method)
                                   .Where(attr => attributeType.IsAssignableFrom(attr.Constructor.ReflectedType))
                                   .OrderBy(attr => attr.Constructor.ReflectedType.Name)
                                   .Select(Reflector.Wrap)
                                   .Cast<IAttributeInfo>()
                                   .ToList();

            if (attributeUsage.Inherited && (attributeUsage.AllowMultiple || !results.Any()))
            {
                // Need to find the parent method, which may not necessarily be on the parent type

                var baseMethod = GetParent(method);
                if (baseMethod != null)
                    results = results.Concat(GetCustomAttributes(baseMethod, attributeType, attributeUsage));
            }

            return results;
        }

        static MethodInfo GetParent(MethodInfo m)
        {
            if (!m.IsVirtual)
                return null;

            var baseType = m.DeclaringType.BaseType;
            if (baseType == null)
                return null;

            return baseType.GetMethod(m.Name, m.GetBindingFlags(), null, m.GetParameters().Select(p => p.ParameterType).ToArray(), null);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return MethodInfo.ToString();
        }

        /// <inheritdoc/>
        public IEnumerable<IParameterInfo> GetParameters()
        {
            return MethodInfo.GetParameters()
                             .Select(Reflector.Wrap)
                             .Cast<IParameterInfo>()
                             .ToList();
        }
    }
}