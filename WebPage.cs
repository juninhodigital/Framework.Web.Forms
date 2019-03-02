using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Framework.Core;
using Framework.Entity;

namespace Framework.Web
{
    /// <summary>
    /// Base Class that inherits from System.Web.UI.Page and implements the Core Services for Data Access
    /// </summary>
    public abstract class WebPage: System.Web.UI.Page
    {
        #region| Methods |
        
        #region| Compression 

        /// <summary>
        /// Enable page compression using gzip encoding in the latest browser version
        /// </summary>
        protected void CompressPage()
        {
            Web.Response.Filter = new GZipStream(Web.Response.Filter, CompressionMode.Compress);

            Web.Response.AppendHeader("Content-encoding", "gzip");
            Web.Response.Cache.VaryByHeaders["Accept-encoding"] = true;

            #region| OLD 

            //var oContext = HttpContext.Current;

            //oContext.Response.Filter = new GZipStream(oContext.Response.Filter, CompressionMode.Compress);

            //HttpContext.Current.Response.AppendHeader("Content-encoding", "gzip");
            //HttpContext.Current.Response.Cache.VaryByHeaders["Accept-encoding"] = true; 

            #endregion
        }

        /// <summary>
        /// Minification is a technique you can use in ASP.NET 4.5 to improve request load time.  
        /// Minification improves load time by reducing the number of requests to the server and reducing the size of requested assets (such as CSS and JavaScript.)
        /// </summary>
        protected void MinifyPage()
        {
            var request  = HttpContext.Current.Request;
            var response = HttpContext.Current.Response;

            response.Filter = new WhiteSpaceResponseFilter(response.Filter, s =>
            {
                //s = Regex.Replace(s, @"\s+", " "); // Se descomentar esta linha, os javascripts da página devem estar em arquivos externos e não inline
                s = Regex.Replace(s, @"\s*\n\s*", "\n");
                s = Regex.Replace(s, @"\s*\>\s*\<\s*", "><");
                s = Regex.Replace(s, @"<!--(.*?)-->", "");   //Remove comments
                s = Regex.Replace(s, @"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}", "");

                // single-line doctype must be preserved 
                var firstEndBracketPosition = s.IndexOf(">");
                if (firstEndBracketPosition >= 0)
                {
                    s = s.Remove(firstEndBracketPosition, 1);
                    s = s.Insert(firstEndBracketPosition, ">");
                }
                return s;
            });
        }
        
        #endregion

        #region| Querystring 

        /// <summary>
        /// Gets the Request.QueryString variable
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>Value</returns>
        protected string GetKey(string Name)
        {
            if (HasKeys())
            {
                return Request.QueryString[Name];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// The System.String Key of the entry that contais the value to get.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected bool HasKey(string Name)
        {
            if (HasKeys())
            {
                return GetKey(Name).IsNull() ? false : true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the System.Collections.Specialized.NameValueCollection contains keys that are not null.
        /// </summary>
        /// <returns>bool</returns>
        protected bool HasKeys()
        {
            return Request.QueryString.HasKeys();
        }

        /// <summary>
        /// Check if all the values exists in the Querystring
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected bool HasKeys(params string[] values)
        {
            var Has = true;

            for (int i = 0; i < values.Length; i++)
            {
                var item = values[i];

                if (HasKey(item) == false)
                {
                    Has = false;
                    break;
                }
            }

            return Has;
        }

        #endregion

        #region| Cache   

        /// <summary>
        /// Check if an item exists in the Cache
        /// </summary>
        /// <param name="CachedItem">Cached Item Name</param>
        /// <returns>bool</returns>
        protected bool HasCache(string CachedItem)
        {
            return Web.Cache[CachedItem] == null ? false : true;
        }

        /// <summary>
        /// Gets An Entity From Cache
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>BussinessEntityStructure</returns>
        protected T GetEntityFromCache<T>(string Name) where T : BusinessEntityStructure
        {
            T Aux = null;

            if (HasCache(Name))
            {
                try
                {
                    Aux = Web.Cache[Name] as T;
                }
                catch (Exception)
                {
                    Aux = null;
                }
            }
            else
            {
                Aux = null;
            }

            return Aux;
        }

        /// <summary>
        /// Gets a Generic List From Cache
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>List<![CDATA[<BussinessEntityStructure>]]></returns>
        protected List<T> GetListFromCache<T>(string Name) where T : BusinessEntityStructure
        {
            List<T> Aux = null;

            if (HasCache(Name))
            {
                try
                {
                    Aux = Web.Cache[Name] as List<T>;
                }
                catch (Exception)
                {

                    Aux = new List<T>();
                }
            }
            else
            {
                Aux = new List<T>();
            }

            return Aux;
        }

        #endregion

        #region| Session 

        /// <summary>
        /// Check if an item exists in the Session
        /// </summary>
        /// <param name="Name">Name</param>
        /// <returns>bool</returns>
        protected bool HasSession(string Name)
        {
            return Web.Session[Name] == null ? false : true;
        }

        /// <summary>
        /// Gets An Entity From Session
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>BussinessEntityStructure</returns>
        protected T GetEntityFromSession<T>(string Name) where T : BusinessEntityStructure
        {
            T Aux = null;

            if (HasSession(Name))
            {
                try
                {
                    Aux = Web.Session[Name] as T;
                }
                catch (Exception)
                {
                    Aux = null;
                }
            }
            else
            {
                Aux = null;
            }

            return Aux;
        }

        /// <summary>
        /// Gets a Generic List From Session
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>List<![CDATA[<BussinessEntityStructure>]]></returns>
        protected List<T> GetListFromSession<T>(string Name) where T : BusinessEntityStructure
        {
            List<T> Aux = null;

            if (HasSession(Name))
            {
                try
                {
                    Aux = Web.Session[Name] as List<T>;
                }
                catch (Exception)
                {
                    Aux = new List<T>();
                }
            }
            else
            {
                Aux = new List<T>();
            }

            return Aux;
        }
        
        #endregion

        #region| S.E.O   

        /// <summary>
        /// Allows programmatic access to the HTML <![CDATA[<meta>]]> tag on the server
        /// </summary>
        /// <param name="Name">Metadata property name (Ex: robots)</param>
        /// <param name="Content">Metadata property value (Ex:noindex,follow)</param>
        protected void AddMetaTag(string Name, string Content)
        {
            using (var oHtmlMeta = new HtmlMeta())
            {
                oHtmlMeta.Name = Name;
                oHtmlMeta.Content = Content;

                Page.Header.Controls.Add(oHtmlMeta);
            }
        }

        /// <summary>
        /// Add the CSS link reference to the page
        /// </summary>
        /// <param name="Path">CSS File Path</param>
        protected void AddCSS(string Path)
        {
            var link = new HtmlLink();

            link.Href = Path;

            link.Attributes.Add("rel", "stylesheet");
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("media", "Screen");

            if (Page.Header.IsNotNull())
            {
                Page.Header.Controls.Add(link);
            }

            link.Dispose();

        }

        /// <summary>
        /// Add the MetaNoFollow in the page header
        /// </summary>
        protected void AddMetaNoFollow()
        {
            HtmlMeta meta = new HtmlMeta();

            meta.Name = "robots";
            meta.Content = "noindex,nofollow";

            Page.Header.Controls.Add(meta);

            meta.Dispose();

        }

        /// <summary>
        /// Add the MetaFollow in the page header
        /// </summary>
        protected void AddMetaFollow()
        {
            HtmlMeta meta = new HtmlMeta();

            meta.Name = "robots";
            meta.Content = "noindex,follow";

            Page.Header.Controls.Add(meta);

            meta.Dispose();
        }

        /// <summary>
        /// Add the Javascript link reference to the page
        /// </summary>
        /// <param name="Path">CSS File Path</param>
        protected void AddJavascript(string Path)
        {
            var oLink = new HtmlGenericControl("script");

            oLink.Attributes.Add("type", "text/javascript");
            oLink.Attributes.Add("src", Path);
            
            this.Page.Header.Controls.AddAt(0, oLink);

        }

        #endregion

        #region| Cookies 

        /// <summary>
        /// Check if an HttpCookie exists in the current request
        /// </summary>
        /// <param name="name">cookie name</param>
        /// <returns>bool</returns>
        protected bool HasCookie(string name)
        {
            var oCookie = Request.Cookies[name];

            if (oCookie.IsNull())
            {
                return false;
            }
            else
            {
                oCookie = null;
                return true;
            }
        }

        /// <summary>
        /// Gets a cookie sent by the client.
        /// </summary>
        /// <param name="Name">Cookie Name</param>
        /// <returns>An System.Web.HttpCookieCollection object representing the client's cookie variables.</returns>
        protected HttpCookie GetCookie(string Name)
        {
            if (HasCookie(Name))
            {
                return Web.Request.Cookies[Name];
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// Remove the http cookie from the client machine
        /// </summary>
        /// <param name="name">cookie name</param>
        protected void ClearCookie(string name)
        {
            var oCookie = Web.Request.Cookies[name];

            if (oCookie.IsNotNull())
            {
                oCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(oCookie);
            }

            oCookie = null;
  
        }

        /// <summary>
        /// Check whether the cookie is enable in the client machine
        /// </summary>
        /// <returns>bool</returns>
        public bool IsCookieEnabled()
        {
            var Name = "FrameworkSupportCookies";
            try
            {
                HttpCookie oCookie = new HttpCookie(Name, "true");
                Web.Response.Cookies.Add(oCookie);

                oCookie = null;
            }
            catch
            {
                return false;
            }

            if (HasCookie(Name) || Web.Request.Browser.Cookies)
            {
                ClearCookie(Name);
                return true;
            }
            else
            {
                return false;
            }
        }
        
        #endregion

        /// <summary>
        /// Gets the IP of the client machine in the current Http Request
        /// </summary>
        /// <returns>IP Address</returns>
        protected string GetIP()
        {
            string Aux = string.Empty;

            try
            {
                Aux = Web.Request.UserHostAddress;

                Aux = Web.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(Aux))
                {
                    Aux = Web.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (Aux == "127.0.0.1")
                {
                    string oHostName = Dns.GetHostName();

                    if (!string.IsNullOrEmpty(oHostName))
                    {
                        if (Dns.GetHostAddresses(oHostName).Length > 0)
                        {
                            Aux = Dns.GetHostAddresses(oHostName)[0].ToString();
                        }
                        else
                        {
                            Aux = Dns.GetHostAddresses(oHostName)[1].ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {
                Aux = Web.Request.UserHostAddress;
            }

            return Aux;
        }

        /// <summary>
        ///  Registers the startup script with the System.Web.UI.Page object using the default ClientScript provide
        /// </summary>
        /// <param name="Script">The startup script literal to register</param>
        protected void ExecuteJS(string Script)
        {
            ExecuteJS(JavascriptProviderTypeEnumerator.ClientScript, Script);
        }

        /// <summary>
        ///  Registers the startup script with the System.Web.UI.Page object
        /// </summary>
        /// <param name="Provider">JavascriptProviderType</param>
        /// <param name="Script">The startup script literal to register</param>
        protected void ExecuteJS(JavascriptProviderTypeEnumerator Provider, string Script)
        {
            if (Provider == JavascriptProviderTypeEnumerator.ScriptManager)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "JS_CORE_KEY", Script, true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "JS_CORE_KEY", Script, true);
            }
        }

        /// <summary>
        /// Check if the page has a MasterpageFile
        /// </summary>
        /// <returns>bool</returns>
        protected bool HasMasterPage()
        {
            return string.IsNullOrEmpty(Page.MasterPageFile) ? false : true;
        }

        /// <summary>
        ///  Maps a virtual path to a physical path on the server
        /// </summary>
        /// <param name="path">Caminho</param>
        /// <returns>string</returns>
        protected string ServerMapPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else if (HostingEnvironment.IsHosted)
            {
                return HostingEnvironment.MapPath(path);
            }
            else if (VirtualPathUtility.IsAppRelative(path))
            {
                string physicalPath = VirtualPathUtility.ToAbsolute(path, "/");
                physicalPath = physicalPath.Replace('/', '\\');
                physicalPath = physicalPath.Substring(1);
                physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, physicalPath);

                return physicalPath;
            }
            else
            {
                throw new Exception("Framework: Could not resolve non-rooted path.");
            }
        }

        /// <summary>
        /// Make and WebRequest using the default System Proxy With the Default Credentials
        /// </summary>
        /// <param name="URL">The Uri that specifies the Internet resource</param>
        /// <returns>Returns a response from the Internet resource</returns>
        public string MakeWebRequest(string URL)
        {
            return MakeWebRequest(URL, false, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Provides an HTTP-specific implementation of the System.Net.WebRequest Class. 
        /// </summary>
        /// <param name="URL">The Uri that specifies the Internet resource</param>
        /// <param name="UseSystemProxy">Indicates if the request will use the proxy information from the Internet Explorer Settings</param>
        /// <param name="ProxyName">Proxy Name</param>
        /// <param name="ProxyPort">Proxy Port</param>
        /// <param name="Username">The Username associated with the Proxy Credential</param>
        /// <param name="Password">The Password associated with the Proxy Credential</param>
        /// <returns>Returns a response from the Internet resource</returns>
        public string MakeWebRequest(string URL, bool UseSystemProxy, string ProxyName, string ProxyPort, string Username, string Password)
        {
            string Result = string.Empty;

            try
            {
                // Create a HttpWebRequest using the URL provided
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

                if (UseSystemProxy)
                {
                    // Get Default Proxy
                    IWebProxy proxy = WebRequest.GetSystemWebProxy();

                    if (proxy != null && proxy.Credentials != null)
                    {
                        request.Proxy = proxy;
                    }
                }
                else
                {
                    if (ProxyName.IsNotNull() && ProxyPort.IsNotNull())
                    {
                        // Create new Proxy passing the ProxyName and ProxyPort (Example: MYISASERVER:8080
                        IWebProxy proxy = new WebProxy(string.Format("{0}:{1}", ProxyName, ProxyPort));

                        request.Proxy = proxy;

                        if (Username.IsNull() || Password.IsNull())
                        {
                            proxy.Credentials = CredentialCache.DefaultCredentials;
                        }
                        else
                        {
                            // Set the Credentials to the Proxy Client
                            proxy.Credentials = new NetworkCredential(Username, Password);
                        }
                    }
                }

                // Get Response and Write Contents to the Console
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Reads the ResponseStream and returns the results
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    Console.WriteLine(sr.ReadToEnd());
                }

                response.Close();
                request = null;

                return Result;

            }
            catch (Exception erro)
            {
                throw new Exception("Framework: Ocorreu um erro no método MakeWebRequest da classe WebCore. Detalhes: " + erro.Message);
            }
        }

        /// <summary>
        /// Fill the DropDownList with the Datasource value
        /// </summary>
        /// <param name="DropDownList">DropDownList</param>
        /// <param name="DataSource">DataSource</param>
        /// <param name="TextMember">TextMember</param>
        /// <param name="ValueMember">ValueMember</param>
        protected void FillDropDown<T>(T DropDownList, object DataSource, string TextMember, string ValueMember)
        {
            if (DropDownList is DropDownList)
            {
                var DDL = DropDownList as DropDownList;

                DDL.Items.Clear();

                DDL.DataSource     = DataSource;
                DDL.DataTextField  = TextMember;
                DDL.DataValueField = ValueMember;
                DDL.DataBind();
            }
        }

        /// <summary>
        /// Fill the DropDownList with the Datasource value
        /// </summary>
        /// <param name="DropDownList">DropDownList</param>
        /// <param name="DataSource">DataSource</param>
        /// <param name="TextMember">TextMember</param>
        /// <param name="ValueMember">ValueMember</param>
        /// <param name="InitialText">InitialText</param>
        protected void FillDropDown(DropDownList DropDownList, object DataSource, string TextMember, string ValueMember, string InitialText)
        {
            this.FillDropDown(DropDownList, DataSource, TextMember, ValueMember);

            DropDownList.Items.Insert(0, new ListItem(InitialText, "0"));
        }

        /// <summary>
        ///  Gets the System.Configuration.AppSettingsSection data for the current application's default configuration.
        /// </summary>
        /// <param name="Name">Key name of the Appsettings in the web.config/app.config</param>
        /// <returns>Value</returns>
        protected static string GetAppSettings(string Name)
        {
            return Framework.Core.Extensions.GetAppSettings("",Name);
        }

        /// <summary>
        /// Indicate if the application will add reference to the JQuery Library using the CDN technique
        /// </summary>
        protected void UseJQuery()
        {
            this.AddJavascript("//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js");
        }

        /// <summary>
        /// Disable the Cache in the page
        /// </summary>
        protected void SetNoCache()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }

        /// <summary>
        /// Indicates whether the page is running using the local web server
        /// </summary>
        /// <returns></returns>
        protected static bool IsLocalhost()
        {
            var URL = Web.Request.Url.ToString().ToLower();

            if (URL.StartsWith("http://localhost", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region| Properties |

        /// <summary>
        ///  Gets or sets the System.Web.HttpContext object for the current HTTP request
        /// </summary>
        public static HttpContext Web
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
