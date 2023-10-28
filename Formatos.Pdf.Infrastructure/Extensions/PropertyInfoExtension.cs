using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Formatos.Pdf.Infrastructure.Extensions
{
    public static class PropertyInfoExtension
    {
        public static string? GetDisplayName(this PropertyInfo? propertyInfo)
        {
            try
            {
                var propertyName = string.Empty;
                if (propertyInfo == null) return propertyName;
                propertyName = propertyInfo.Name;
                var displayNames = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true).ToList();
                if (displayNames.Any())
                {
                    propertyName = (displayNames.FirstOrDefault() as ColumnAttribute)?.Name;
                }
                return propertyName;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
