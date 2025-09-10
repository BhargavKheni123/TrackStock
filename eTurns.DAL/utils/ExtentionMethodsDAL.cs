using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace eTurns.DAL
{
    /// <summary>
    /// Common extention methods for DAL
    /// </summary>
    public static class ExtentionMethodsDAL
    {
        /// <summary>
        /// Return value or DBNull.Value for sql parameters
        /// </summary>
        /// <param name="sqlParaVal"></param>
        /// <returns></returns>
        public static object ToDBNull(this Int64? sqlParaVal)
        {
            if (sqlParaVal == null)
            {
                return DBNull.Value;
            }
            return sqlParaVal.Value;
        }

        public static object ToDBNull(this float? sqlParaVal)
        {
            if (sqlParaVal == null)
            {
                return DBNull.Value;
            }
            return sqlParaVal.Value;
        }

        public static object ToDBNull(this double? sqlParaVal)
        {
            if (sqlParaVal == null)
            {
                return DBNull.Value;
            }
            return sqlParaVal.Value;
        }

        public static object ToDBNull(this DateTime? sqlParaVal)
        {
            if (sqlParaVal == null)
            {
                return DBNull.Value;
            }
            return sqlParaVal.Value;
        }

        public static object ToDBNull(this string sqlParaVal)
        {
            if (string.IsNullOrWhiteSpace(sqlParaVal))
            {
                return DBNull.Value;
            }
            return sqlParaVal;
        }

        public static object ToDBNull(this bool? sqlParaVal)
        {
            if (sqlParaVal == null)
            {
                return DBNull.Value;
            }
            return sqlParaVal.Value;
        }

        public static string TrimEnd2(this string input, string suffixToRemove)
        {
            StringComparison comparisonType = StringComparison.CurrentCulture;
            if (input != null && suffixToRemove != null
              && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else return input;
        }

        public static string ToXML<T>(this List<T> list)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<T>));
            string strXml = "";
            using (StringWriter sw = new StringWriter())
            {
                ser.Serialize(sw, list);
                strXml = sw.ToString();
            }

            if (!string.IsNullOrWhiteSpace(strXml))
            {
                strXml = strXml.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").Trim();
                strXml = strXml.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "")
                    .Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "")
                    .Replace("xsi:nil=\"true\"", "");

            }

            return strXml;
        }

        public static string ToSQLParaNameCSV(this SqlParameter[] sqlPara)
        {
            string csv = "";

            foreach (SqlParameter para in sqlPara)
            {
                csv += para.ParameterName + ",";
            }

            if (!string.IsNullOrWhiteSpace(csv))
            {
                csv = csv.TrimEnd(',');
            }

            return csv;
        }

    }// class



    public class EFCommandInterceptor : DbCommandInterceptor
    {
        private static readonly Regex _tableAliasRegex =
        new Regex(@"(?<tableAlias>AS \[Extent\d+\](?! WITH \(NOLOCK\)))",
            RegexOptions.Multiline | RegexOptions.IgnoreCase);

        public override void ReaderExecuting(System.Data.Common.DbCommand command, DbCommandInterceptionContext<System.Data.Common.DbDataReader> interceptionContext)
        {
            //CommandExecuting<System.Data.Common.DbDataReader>(base.ReaderExecuting, command, interceptionContext);
            command.CommandText =
                _tableAliasRegex.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
        }
    }
}