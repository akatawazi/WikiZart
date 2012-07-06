using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Principal;
using System.Web.Mvc;
using System.Configuration;

namespace System.Web
{
    public static class JavascriptManagerHelper
    {
        /// <summary>This creates the table Paginator Control for the Grid</summary>
        public static IHtmlString JavascriptManager(this HtmlHelper helper, string WebConfigEntryName)
        {
            string javascriptSuffix = String.Empty;
            if (ConfigurationManager.AppSettings["ApplicationVersion"] != null)
            {
                javascriptSuffix = "?v=" + ConfigurationManager.AppSettings["ApplicationVersion"].ToString();
            }

            if (String.IsNullOrEmpty(WebConfigEntryName))
                WebConfigEntryName = "MasterPage";
            StringBuilder stringBuilder = new StringBuilder();
            var javascriptString = ConfigurationManager.AppSettings[WebConfigEntryName].ToString().ToLower().Replace("[staticfileurl]", ConfigurationManager.AppSettings["StaticFileURL"].ToString()).Replace("[applicationurl]", ConfigurationManager.AppSettings["ApplicationURL"].ToString());

            var files = javascriptString.Split(',');
            foreach (var file in files)
            {
                if (file.Contains(".css"))
                {
                    stringBuilder.Append("<link href='" + file + javascriptSuffix + "' rel='stylesheet' type='text/css' />");
                }
                if (file.Contains(".js"))
                {
                    stringBuilder.Append("<script src='" + file + javascriptSuffix + "' type='text/javascript'></script>");
                }
            }

            var controlOutput = new System.Web.HtmlString(stringBuilder.ToString());
            return controlOutput;
        }
    }
}