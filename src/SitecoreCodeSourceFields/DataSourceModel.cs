using System;
using System.Globalization;
using System.Reflection;

namespace SitecoreCodeSourceFields
{
    public class DataSourceModel
    {
        public string ClassName { get; internal set; }
        public string AssemblyName { get; internal set; }
        public string MemberName { get; internal set; }
        public MemberTypes MemberType { get; internal set; }

        public bool Valid
        {
            get { return !string.IsNullOrEmpty(ClassName) && !string.IsNullOrEmpty(MemberName); }
        }

        public static DataSourceModel Parse(string dataSource)
        {
            var result = new DataSourceModel();
            var assemblyChuncks = dataSource.Split(new[] { ',' }, 2);
            if (assemblyChuncks.Length == 2)
            {
                result.AssemblyName = assemblyChuncks[1].Trim();
            }

            var memberIndex = assemblyChuncks[0].LastIndexOf(".", StringComparison.InvariantCulture);
            if (memberIndex <= 0 || memberIndex + 2 >= assemblyChuncks[0].Length)
            {
                result.ClassName = assemblyChuncks[0].Trim();
                return result;
            }

            result.ClassName = assemblyChuncks[0].Substring(0, memberIndex);
            var memberName = assemblyChuncks[0].Substring(memberIndex + 1).Trim();
            if (memberName.EndsWith("()"))
            {
                result.MemberName = memberName.TrimEnd(new[] { '(', ')' });
                result.MemberType = MemberTypes.Method;
            }
            else
            {
                result.MemberName = memberName;
                result.MemberType = MemberTypes.Property;
            }

            return result;
        }

    }

    public static class DataSourceModelExtensions
    {
        public static object Invoke(this DataSourceModel model)
        {
            var type = GetType(model);
            var member = GetMember(type, model);
            var value = member.Invoke(null, new object[0]);
            return value;
        }

        private static Type GetType(DataSourceModel model)
        {
            var fullname = model.AssemblyName == null
                ? model.ClassName
                : model.ClassName + ", " + model.AssemblyName;
            return Type.GetType(fullname);
        }

        private static MethodInfo GetMember(Type type, DataSourceModel model)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            MethodInfo result;

            switch (model.MemberType)
            {
                case MemberTypes.Property:
                    var property = type.GetProperty(model.MemberName, bindingFlags);
                    if (property == null || !property.CanRead)
                    {
                        throw new MissingMemberException(string.Format(CultureInfo.InvariantCulture,
                            "No readable public static property named {0} found in {1}", model.MemberName, type.FullName));
                    }

                    result = property.GetMethod;
                    break;

                case MemberTypes.Method:
                    result = type.GetMethod(model.MemberName, bindingFlags, null, new Type[0], new ParameterModifier[0]);
                    break;

                default:
                    throw new InvalidOperationException("Unsupported membertype: " + model.MemberType);
            }

            if (result == null)
            {
                throw new MissingMemberException(string.Format(CultureInfo.InvariantCulture,
                    "Could not find any public static {0} named {1} in class {2}",
                    model.MemberType, model.MemberName, type.FullName));
            }

            return result;
        }
    }

}
