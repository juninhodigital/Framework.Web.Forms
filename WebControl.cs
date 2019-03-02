using System.Web;

namespace Framework.Web
{
    /// <summary>
    /// Base Class that inherits from System.Web.UI.UserControl and implements the Core Services for Data Access
    /// </summary>
    public abstract class WebControl : System.Web.UI.UserControl
    {
        #region| Properties |
        
        /// <summary>
        /// Gets the page that hosts the usercontrol
        /// </summary>
        protected WebPage HostPage
        {
            get
            {
                return ((WebPage)this.Page);
            }
        }
        
        /// <summary>
        ///  Gets or sets the System.Web.HttpContext object for the current HTTP request
        /// </summary>
        protected static HttpContext Web
        {
            get
            {
                return HttpContext.Current;
            }
            set
            {
                HttpContext.Current = value;
            }
        }

        #endregion
    }
}
