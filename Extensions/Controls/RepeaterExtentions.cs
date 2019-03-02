using System.Web.UI.WebControls;

using Framework.Core;

namespace Framework.Web
{
    /// <summary>
    /// Providers Extension Methods that are a special kind of static method, 
    /// but they are called as if they were instance methods on the extended type. 
    /// </summary>
    public static class RepeaterExtentions
    {
        #region| Methods |

        /// <summary>
        /// Gets or sets a data item associated with the System.Web.UI.WebControls.RepeaterItem object in the System.Web.UI.WebControls.Repeater control.
        /// </summary>
        /// <typeparam name="T">paramtype</typeparam>
        /// <param name="e">RepeaterItemEventArgs</param>
        /// <returns></returns>
        public static T CastTo<T>(this RepeaterItemEventArgs @e) where T:class, new()
        {
            return @e.CastTo<T>(true);
        }

        /// <summary>
        ///  Searches the current naming container for a server control with the specified id parameter.
        /// </summary>
        /// <typeparam name="T">paramtype</typeparam>
        /// <param name="e"></param>
        /// <param name="id">The identifier for the control to be found.</param>
        /// <returns>WebControl</returns>
        public static WebControl GetControl<T>(this RepeaterItemEventArgs @e, string id) where T: WebControl
        {
            return @e.Item.FindControl(id) as T;
        }

        /// <summary>
        /// Indicates whether the repeater item is a datarow item
        /// </summary>
        /// <param name="e">RepeaterItemEventArgs</param>
        /// <returns>bool</returns>
        public static bool IsListItem(this RepeaterItemEventArgs @e)
        {
            return @e.Item.ItemType.In(ListItemType.Item, ListItemType.AlternatingItem);
        }

        /// <summary>
        /// Indicates whether the list item type is footer
        /// </summary>
        /// <param name="e">RepeaterItemEventArgs</param>
        /// <returns>bool</returns>
        public static bool IsFooter(this RepeaterItemEventArgs @e)
        {
            return @e.Item.ItemType.Equals(ListItemType.Footer);
        }

        /// <summary>
        /// Indicates whether the list item type is footer
        /// </summary>
        /// <param name="e">RepeaterItemEventArgs</param>
        /// <returns>bool</returns>
        public static bool IsHeader(this RepeaterItemEventArgs @e)
        {
            return @e.Item.ItemType.Equals(ListItemType.Header);
        }

        #endregion
    }
}
