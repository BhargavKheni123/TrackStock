using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

public static class DataTableHelper
{
    /// <summary>
    /// Converts a DataTable to a list with generic objects
    /// </summary>
    /// <typeparam name="T">Generic object</typeparam>
    /// <param name="table">DataTable</param>
    /// <returns>List with generic objects</returns>
    public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
    {
        try
        {
            List<T> list = new List<T>();

            foreach (var row in table.AsEnumerable())
            {
                T obj = new T();

                foreach (var prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                        propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    }
                    catch
                    {
                        continue;
                    }
                }

                list.Add(obj);
            }

            return list;
        }
        catch
        {
            return null;
        }
    }

    public static List<TSource> ToList<TSource>(this DataTable dataTable) where TSource : new()
    {
        var dataList = new List<TSource>();

        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                             select new
                             {
                                 Name = aProp.Name,
                                 Type = Nullable.GetUnderlyingType(aProp.PropertyType) ??
                         aProp.PropertyType
                             }).ToList();
        var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                 select new
                                 {
                                     Name = aHeader.ColumnName,
                                     Type = aHeader.DataType
                                 }).ToList();
        var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

        foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
        {
            var aTSource = new TSource();
            foreach (var aField in commonFields)
            {
                PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField.Name);
                var value = (dataRow[aField.Name] == DBNull.Value) ?
                null : dataRow[aField.Name]; //if database field is nullable
                propertyInfos.SetValue(aTSource, value, null);
            }
            dataList.Add(aTSource);
        }
        return dataList;
    }
}