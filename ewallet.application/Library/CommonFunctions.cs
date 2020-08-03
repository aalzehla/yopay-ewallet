using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ewallet.application.Library
{
    public static class CommonFunctions
    {

        public static string SingnatureGenerator<T>(T item)
        {
            Type type = item.GetType();
            PropertyInfo[] props = type.GetProperties();
            var signature = "";
            foreach (var prop in props.ToList().OrderBy(x => x.Name))
            {

                try
                {
                    if (!prop.Name.Equals("Signature"))
                        signature += prop.GetValue(item).ToString();
                }
                catch
                {
                    signature += "";
                }

            }
            return signature;
        }


        public static string SerializeObjectToJSON(this object obj)
        {
            string serializedObject = string.Empty;
            serializedObject = JsonConvert.SerializeObject(obj);
            return serializedObject;
        }

        public static string Getwayurl()
        {
            return "https://gatewaysandbox.nepalpayment.com/payment/index";
        }
    }
}