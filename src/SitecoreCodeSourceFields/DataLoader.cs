using System;
using System.Collections.Generic;
using System.Linq;

namespace SitecoreCodeSourceFields
{
    public static class DataLoader
    {
        public static bool SafeGetKeyValue(string datasource, out IEnumerable<KeyValuePair<string, string>> result,
            out string error)
        {
            try
            {
                result = GetKeyValue(datasource);
                error = null;
                return true;
            }
            catch (Exception exc)
            {
                error = exc.Message;
                result = null;
                return false;
            }
        }
        public static ICollection<KeyValuePair<string, string>> GetKeyValue(string datasource)
        {
            var model = DataSourceModel.Parse(datasource);
            var value = model.Invoke();

            if (value == null)
            {
                return new KeyValuePair<string, string>[0];
            }

            var result = value as IEnumerable<KeyValuePair<string, string>>;
            if (result != null)
            {
                return result.ToList();
            }

            var enumerable = value as IEnumerable<string>;
            if (enumerable != null)
            {
                return enumerable.Select(x => new KeyValuePair<string, string>(x, x)).ToList();
            }

            throw new InvalidOperationException("Returned value was of unsupported type: " + value.GetType().FullName);
        }
    }
}
