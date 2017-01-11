using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using mandel;

namespace mandel.flythru.lib
{
    /// <summary>
    /// Base functionality for fly thru handlers.  Includes handling all controls from the ui and storing state so the UIs don't have to learn arbitrary math.
    /// We use the context's Cache to store the user's location (state) between calls.
    /// </summary>
    public abstract class FlyThruHandlerBase : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        // Cache key for the user's location.
        const string _location_key = "FlyThruLocation";

        // Cache key for the user's current color map.
        const string _color_map_key = "FlyThruColorMap";


        /// <summary>
        /// Process the incoming request. 
        /// It should be a POST with Form Data.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            // Get the form data passed in on the request and cache
            var form = context.Request.Form;
            var cache = context.Cache;

            var shift_rate = 0.175m;

            
            // The command issued from UI.
            var cmd = form["command"];


            var location = cache.Get(_location_key) as LocationBase<MDecimal>;
            if(location == null || cmd == "reset")
            {
                location = new Location(new MDecimal(-2.5m), new MDecimal(1m), new MDecimal(-1m), new MDecimal(1m));
                location.RateOfDescent = 0.95m;
            }

            if(cmd == "colors")
            {
                // Dump the color map and a new one will replace it.
                cache.Remove(_color_map_key);
            }
            else
            {
                var y_shift = (location.yMax.value - location.y0.value) * shift_rate;
                var x_shift = (location.xMax.value - location.x0.value) * shift_rate;
                if (cmd == "north")
                {
                    location.y0.value -= y_shift;
                    location.yMax.value -= y_shift;
                }
                else if (cmd == "south")
                {
                    location.y0.value += y_shift;
                    location.yMax.value += y_shift;
                }
                else if (cmd == "east")
                {
                    location.x0.value += x_shift;
                    location.xMax.value += x_shift;
                }
                else if (cmd == "west")
                {
                    location.x0.value -= x_shift;
                    location.xMax.value -= x_shift;
                }
                else if (cmd == "slower")
                {
                    location.RateOfDescent = location.RateOfDescent * 1.1m;
                }
                else if (cmd == "faster")
                {
                    location.RateOfDescent = location.RateOfDescent * .98m;
                }
            }


            var rate = location.RateOfDescent;

            var x_zoom = (location.xMax.value - location.x0.value) * rate;
            var x_zoom_piece = ((location.xMax.value - location.x0.value) - x_zoom) / 2m;

            var y_zoom = (location.yMax.value - location.y0.value) * rate;
            var y_zoom_piece = ((location.yMax.value - location.y0.value) - y_zoom) / 2m;

            location.y0 = location.y0.value + y_zoom_piece;
            location.yMax = location.yMax.value - y_zoom_piece;
            location.x0 = location.x0.value + x_zoom_piece;
            location.xMax = location.xMax.value - x_zoom_piece;

            cache[_location_key] = location;

            var bytes = new RenderJpegBytes(400, 300)
                                .Render<RegularMathCalculator, WaveyColoring, MDecimal>(location, 250, 8, 20);

            File.WriteAllBytes(context.Server.MapPath("~/output/frame.jpg"), bytes);
        }
    }
}
