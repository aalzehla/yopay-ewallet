using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ewallet.application.Library
{
    public class ProjectGrid
    {

        public static IDictionary<string, string> column { get; set; }
        /// <summary>
        /// Make Grid/table 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">List Of object</param>
        /// <param name="ControlerName">Controller Name</param>
        /// <param name="Search">Search Value(not used)</param>
        /// <param name="Pagesize">page size(not used)</param>
        /// <param name="allowAdd">allow add button</param>
        /// <param name="DateField"></param>
        /// <param name="RowId"></param>
        /// <param name="breadcrumb1"></param>
        /// <param name="breadcrumb2"></param>
        /// <param name="breadcrumb2url"></param>
        /// <param name="addPageUrl">if allowAdd is true addPageUrl is visible and can be customized</param>
        /// <param name="className">Class name for table</param>
        /// <returns></returns>
        public static string MakeGrid<T>(List<T> obj, string ControlerName, string Search, int Pagesize, bool allowAdd = true, string DateField = "", string RowId = "", string breadcrumb1 = "", string breadcrumb2 = "", string breadcrumb2url = "", string addPageUrl = "",string className="datatable")
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"breadcrumb-line breadcrumb-line-light header-elements-md-inline\">");

            if (ControlerName.ToLower() != "hidebreadcrumb")
            {
                sb.AppendLine("<div class=\"d-flex\">");
                sb.AppendLine("<div class=\"breadcrumb\">");
                sb.AppendLine("<a href=\"" + ApplicationUtilities.GenerateUrl("~/") + "\" class=\"breadcrumb-item\"><i class=\"icon-home2 mr-2\"></i>" + (breadcrumb1 == "" ? "Dashboard" : breadcrumb1) + "</a>");
                sb.AppendLine(breadcrumb2 != "" ? "<a href=\"" + (breadcrumb2url == "" ? "#" : breadcrumb2url) + "\" class=\"breadcrumb-item\">" + breadcrumb2 + "</a>" : "");
                sb.AppendLine("<span class=\"breadcrumb-item active\">" + ControlerName + "</span>");
                sb.AppendLine("</div>");

                sb.AppendLine("<a href=\"#\" class=\"header-elements-toggle text-default d-md-none\"><i class=\"icon-more\"></i></a>");
                sb.AppendLine("</div>");

                sb.AppendLine("<div class=\"header-elements d-none\">");
                sb.AppendLine("<div class=\"breadcrumb justify-content-center\">");
                //sb.AppendLine("<div class=\"breadcrumb-elements-item dropdown p-0\"><a href=\"#\" class=\"breadcrumb-elements-item\" onclick=\"GoBack();\">");
                //sb.AppendLine("<i class=\"icon-arrow-left52 mr-2\"></i>");
                //sb.AppendLine("Back");
                //sb.AppendLine("</a></div>");
                if (allowAdd && ApplicationUtilities.HasPageRight(addPageUrl))
                {
                    sb.AppendLine("<div class=\"breadcrumb-elements-item dropdown p-0\"><a href='" + (String.IsNullOrEmpty(addPageUrl) ? "#" : addPageUrl) + "' class=\"btn btn-primary\"><i class=\"icon-plus2 mr-2\"></i> Add New</a></div>");
                }
                //sb.AppendLine("<div class=\"breadcrumb-elements-item dropdown p-0\">");
                //	sb.AppendLine("<a href=\"#\" class=\"breadcrumb-elements-item dropdown-toggle\" data-toggle=\"dropdown\">");
                //		sb.AppendLine("<i class=\"icon-gear mr-2\"></i>");
                //		sb.AppendLine("Settings");
                //	sb.AppendLine("</a>");

                //	sb.AppendLine("<div class=\"dropdown-menu dropdown-menu-right\">");
                //		sb.AppendLine("<a href=\"#\" class=\"dropdown-item\"><i class=\"icon-user-lock\"></i> Account security</a>");
                //		sb.AppendLine("<a href=\"#\" class=\"dropdown-item\"><i class=\"icon-statistics\"></i> Analytics</a>");
                //		sb.AppendLine("<a href=\"#\" class=\"dropdown-item\"><i class=\"icon-accessibility\"></i> Accessibility</a>");
                //		sb.AppendLine("<div class=\"dropdown-divider\"></div>");
                //		sb.AppendLine("<a href=\"#\" class=\"dropdown-item\"><i class=\"icon-gear\"></i> All settings</a>");
                //	sb.AppendLine("</div>");
                //sb.AppendLine("</div>");
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</div><br /><br />");
            sb.AppendLine("<div class=\"card\">");
            if (ControlerName.ToLower() != "hidebreadcrumb")
            {
                sb.AppendLine("<div class='card-header header-elements-inline'>");
                sb.AppendLine("<h5 class='card-title'>" + ControlerName + "</h5>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("<div class=\"panel-body list-body table-responsive\">");
            sb.AppendLine("<table class=\"table "+className+"\">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>S.N</th>");
            foreach (var item in column)
            {
                sb.AppendLine("<th>" + item.Value + "</th>");
                //sql += ", @" + item.Key + " = " + wDao.FilterString(item.Value);
            }
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            int SN = 0;

            foreach (var item in obj)
            {
                SN++;
                int num = 0;
                sb.AppendLine("<tr id=" + SN + ">");
                sb.AppendLine("<td>" + SN + "</td>");
                Type t = item.GetType();
                foreach (var col in column)
                {
                    num++;
                    var value = item.GetType().GetProperty(col.Key).GetValue(item, null);
                    if (!string.IsNullOrWhiteSpace(DateField) && DateField.Split('|').Contains(num.ToString()))
                    {
                        if (null != value)
                        {
                            //value = StaticData.DBToFrontDate(value.ToString());
                            value = (string.IsNullOrWhiteSpace(value.ToString()) ? "" : value.ToString().Substring(0, 10));
                        }
                    }
                    if (col.Key.Contains("IsActive") && null != value)
                    {
                        value = (value.ToString() == "True" ? "Yes" : (value.ToString() == "1" ? "Yes" : "No"));
                        //value = (value.ToString()=="1" ? "Yes" : "No");
                    }
                    sb.AppendLine("<td>" + value + "</td>");
                }
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        public static string MakeGrid1<T>(List<T> obj, string _Value)
        {
            foreach (var item in obj)
            {
                Type t = item.GetType();
                foreach (var col in column)
                {
                    var a = item.GetType().GetProperty(col.Key).GetValue(item, null);

                }
            }



            return "";
        }

        public static string GetBreadCum(string breadcrumb = "Home|Management|Manage", string ButtonName = "Submit")
        {
            var label = breadcrumb.Split('|');
            string breadCum = "";
            breadCum += "<div class='page-header' id='stickHeader'>";
            breadCum += "<div class='row'>";
            breadCum += "<div class='col-sm-6'>";
            breadCum += "<ol class='breadcrumb'>";
            breadCum += "<li><a href='#'>" + label[0] + "</a></li>";
            breadCum += "<li><a href='#'>" + label[1] + "</a></li>";
            breadCum += "<li class='active'>" + label[2] + "</li>";
            breadCum += "</ol>";
            breadCum += "</div>";
            breadCum += "<div class='col-sm-6'>";
            breadCum += "<div class='form-group form-action'>";
            breadCum += "<a onclick='GoBack();' class='btn btn-default'><i class='mdi mdi mdi-arrow-left'></i> Back </a>";
            if (!string.IsNullOrWhiteSpace(ButtonName))
            {
                breadCum += "<button type='submit' class='btn btn-success'><i class='mdi mdi-check-circle-outline'></i> " + ButtonName + "</button>";
            }
            breadCum += "</div>";
            breadCum += "</div>";
            breadCum += "</div>";
            breadCum += "</div>";
            return breadCum;

        }

        public static string GetAddBreadCum(string breadcrumb, string controller, string AddFunctionId)
        {
            var label = breadcrumb.Split('|');
            string breadCum = "";
            breadCum += "<div class=\"page-header\">";
            breadCum += "<div class=\"row\">";
            breadCum += "<div class=\"col-sm-6\">";
            breadCum += "<ol class=\"breadcrumb\">";
            breadCum += "<li><a href='#'>" + label[0] + "</a></li>";
            breadCum += "<li><a href='#'>" + label[1] + "</a></li>";
            breadCum += "<li class='active'>" + label[2] + "</li>";
            breadCum += "</ol>";
            breadCum += "</div>";
            breadCum += "<div class=\"col-sm-6\">";
            breadCum += "<div class=\"form-group form-action\">";
            breadCum += "<button class=\"btn btn-default\" onclick=\"GoBack();\"><i class=\"mdi mdi-arrow-left\"></i> Back</button>";

            if (StaticData.HasRight(controller, AddFunctionId))
            {
                breadCum += "<a href=\"/" + controller + "/Manage\" class=\"btn btn-success\"><i class=\"mdi mdi-plus\"></i> Add New</a>";
            }
            breadCum += "</div></div></div></div>";
            return breadCum;
        }

    }
}