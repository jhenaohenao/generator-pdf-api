using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace Formatos.Pdf.Infrastructure.Extensions
{
    public static class DataSetsExtension
    {
        #region DataTableToList
        /// <summary>
        /// Este metodo covierte un DataTable en un List<T>
        /// </summary>
        /// <typeparam name="T">Tipo de objeto de la lista</typeparam>
        /// <param name="table">DataTable que va a ser convertido</param>
        /// <returns>List<T></returns>
        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            List<T> list;
            try
            {
                var listDto = new ConcurrentBag<T>();
                var objAux = new T();
                var properties = objAux.GetType().GetProperties();

                Parallel.ForEach(table.AsEnumerable(), row =>
                {
                    var obj = new T();
                    Parallel.ForEach(properties, property =>
                    {

                        var propertyInfo = obj.GetType().GetProperty(property.Name);
                        var attr = obj.GetType().GetProperty(property.Name).GetCustomAttributes();
                        NotMappedAttribute notMapped = new NotMappedAttribute();
                        if (!attr.Contains(notMapped))
                        {
                            if (propertyInfo == null) return;
                            var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? property.PropertyType;
                            var propertyName = propertyInfo.GetDisplayName();
                            var safeValue = (row[propertyName] is DBNull) ? null : Convert.ChangeType(row[propertyName], type);
                            propertyInfo.SetValue(obj, safeValue, null);
                        }
                    });
                    listDto.Add(obj);
                });
                list = listDto.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
        #endregion

        #region ToObject
        /// <summary>
        /// Este metodo convierte un DataRow en un Object<T>
        /// </summary>
        /// <typeparam name="T">Tipo de objeto al que se va a convertir el DataRow</typeparam>
        /// <param name="row">DataRow que va a ser convertido</param>
        /// <returns>Object<T></returns>
        public static T ToObject<T>(this DataRow row) where T : class, new()
        {
            T obj = new T();
            var properties = obj.GetType().GetProperties();
            try
            {
                Parallel.ForEach(properties, property =>
                {
                    PropertyInfo propertyInfo = obj.GetType().GetProperty(property.Name);
                    if (propertyInfo != null)
                    {
                        Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? property.PropertyType;
                        string? propertyName = propertyInfo.GetDisplayName();
                        object safeValue = (row[propertyName] == null) ? null : Convert.ChangeType(row[propertyName], type);
                        propertyInfo.SetValue(obj, safeValue, null);
                    }
                });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return obj;
        }
        #endregion
    }
}
