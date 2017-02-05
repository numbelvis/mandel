using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mandel.flythru.web.pages
{
    public partial class viewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ModeNumber = Convert.ToString(Page.RouteData.Values["mode"]);
        }

        public string ScriptLocation
        {
            get
            {
#if DEBUG
                return "mandel.flythru.js";
#else
                return "mandel.flythru.min.js"
#endif
            }
        }

        public string ModeNumber
        { get; set; }
    }
}