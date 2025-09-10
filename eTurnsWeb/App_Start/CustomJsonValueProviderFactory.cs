//using Newtonsoft.Json.Serialization;
//using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace eTurnsWeb
{
    public sealed class CustomJsonValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            var jsonData = GetDeserializedObject(controllerContext);
            if (jsonData == null)
            {
                return null;
            }

            var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var backingStoreWrapper = new EntryLimitedDictionary(backingStore);
            AddToBackingStore(backingStoreWrapper, String.Empty, jsonData);
            var v = new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
            return v;
        }

        private static void AddToBackingStore(EntryLimitedDictionary backingStore, string prefix, object value)
        {
            var d = value as IDictionary<string, object>;
            if (d != null)
            {
                foreach (var entry in d)
                {
                    AddToBackingStore(backingStore, MakePropertyKey(prefix, entry.Key), entry.Value);
                }
                return;
            }

            var l = value as IList;
            if (l != null)
            {
                for (var i = 0; i < l.Count; i++)
                {
                    AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
                }
                return;
            }

            // primitive  
            backingStore.Add(prefix, value);
        }

        private static object GetDeserializedObject(ControllerContext controllerContext)
        {
            if (
                !controllerContext.HttpContext.Request.ContentType.StartsWith("application/json",
                    StringComparison.OrdinalIgnoreCase))
            {
                // not JSON request  
                return null;
            }

            var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            var bodyText = reader.ReadToEnd();
            if (String.IsNullOrEmpty(bodyText))
            {
                // no JSON data  
                return null;
            }

            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };

            var jsonData = serializer.DeserializeObject(bodyText);
            return jsonData;
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
        }

        private class EntryLimitedDictionary
        {
            private readonly IDictionary<string, object> _innerDictionary;
            private int _itemCount;

            public EntryLimitedDictionary(IDictionary<string, object> innerDictionary)
            {
                _innerDictionary = innerDictionary;
            }

            public void Add(string key, object value)
            {
                if (++_itemCount > MaximumDepth)
                {
                    throw new InvalidOperationException(eTurns.DTO.Resources.ResMessage.MaxJsonLengthexceed);
                }

                _innerDictionary.Add(key, value);
            }

            private static int GetMaximumDepth()
            {
                var appSettings = ConfigurationManager.AppSettings;

                var valueArray = appSettings.GetValues("aspnet:MaxJsonDeserializerMembers");
                if (valueArray != null && valueArray.Length > 0)
                {
                    int result;
                    if (Int32.TryParse(valueArray[0], out result))
                    {
                        return result;
                    }
                }

                return 1000; // Fallback default  
            }

            private static readonly int MaximumDepth = GetMaximumDepth();
        }
    }


    public sealed class JsonDotNetValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                return null;

            var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            var bodyText = reader.ReadToEnd();
            var v = new DictionaryValueProvider<object>(JsonConvert.DeserializeObject<ExpandoObject>(bodyText, new ExpandoObjectConverter()), CultureInfo.CurrentCulture);
            return String.IsNullOrEmpty(bodyText) ? null : v;
        }
    }
}