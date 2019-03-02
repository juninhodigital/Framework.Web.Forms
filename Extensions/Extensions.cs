using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Framework.Web
{
    /// <summary>
    /// Providers Extension Methods that are a special kind of static method, 
    /// but they are called as if they were instance methods on the extended type. 
    /// </summary>
    public static partial class Extensions
    {
        #region| Methods |

        /// <summary>
        ///   Reloads the current page / handler by performing a redirect to the current url
        /// </summary>
        /// <param name = "response">The HttpResponse to perform on.</param>
        public static void Reload(this HttpResponse response)
        {
            response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }

        /// <summary>
        /// Changes the display value of the visibility property to ''
        /// </summary>
        /// <param name="HtmlControl">HtmlControl</param>
        public static void Show(this HtmlControl @HtmlControl)
        {
            @HtmlControl.Style.Add("display", "");
        }

        /// <summary>
        /// Changes the display value of the visibility property to 'none'
        /// </summary>
        /// <param name="HtmlControl">HtmlControl</param>
        public static void Hide(this HtmlControl @HtmlControl)
        {
            @HtmlControl.Style.Add("display", "none");
        }

        /// <summary>
        /// Changes the visible property to 'true'
        /// </summary>
        /// <param name="WebControl">WebControl</param>
        public static void Show(this WebControl @WebControl)
        {
            @WebControl.Visible = true;
        }

        /// <summary>
        /// Changes the visible property to 'false'
        /// </summary>
        /// <param name="WebControl">WebControl</param>
        public static void Hide(this WebControl @WebControl)
        {
            @WebControl.Visible = false;
        }

        /// <summary>
        ///   Gets or sets the source containing a list of values used to populate the items within the control.
        /// </summary>
        /// <param name="Control">@Control</param>
        /// <param name="DataSource">DataSource</param>
        public static void DataSource(this BaseDataList @Control, object DataSource)
        {
            @Control.DataSource = DataSource;
            @Control.DataBind();
        }

        /// <summary>
        /// Select an ListItem in the DropDownList by it's value or text
        /// </summary>
        /// <param name="Control">DropDownList</param>
        /// <param name="input">input to compare the ListItem value or text</param>
        /// <param name="SelectionType">DropDownSelectionType</param>
        /// <example>
        /// <code>
        ///     DropDownList1.SelectItem("ABCD", DropDownSelectionType.ByText);
        ///     DropDownList1.SelectItem("11", DropDownSelectionType.ByValue);
        /// </code>
        /// </example>
        public static void SelectItem(this DropDownList @Control, string input, DropDownSelectionTypeEnumerator SelectionType)
        {
            @Control.ClearSelection();

            System.Web.UI.WebControls.ListItem oListItem;

            if (SelectionType == DropDownSelectionTypeEnumerator.ByValue)
            {
                oListItem = @Control.Items.FindByValue(input.Trim());
            }
            else
            {
                oListItem = @Control.Items.FindByText(input.Trim());
            }

            if (oListItem!=null)
            {
                oListItem.Selected = true;
            }
        }

        /// <summary>
        ///     A HttpResponse extension method that redirects.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="format">The URL format.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <example>
        ///     <code>
        ///           <![CDATA[@this.Redirect("http://www.yourpage.com?ID={0}&Name={1}", "10", "Sample"); // Results: http://www.yourpage.com?ID=10&Name=Sample]]>
        ///     </code>
        /// </example>
        public static void Redirect(this HttpResponse @this, string format, params object[] values)
        {
            string URL = string.Format(format, values);

            @this.Redirect(URL, true);
        }

        /// <summary>
        /// Add the event triggered through javascript on the onclick
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        public static void OnClick(this WebControl @control, string value)
        {
            @control.Attributes.Add("onclick", value);
        }

        #endregion
    }
}
