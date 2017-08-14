/*

    This is the main() entry point of the Mandel.flythru application.

    The application is written so that only one can run at a time on a page.
    Only a single viewer and loop can run on the page.  This is on purpose.
*/

import "babel-polyfill";

import { wire } from './controls';
import { initialize as initViewer } from './viewer';
import { initialize as initLoop } from './loop';


// Get all of the elements and hand them off to the appropriate modules
let els = {},
    
    processors = document.querySelector('input[name="processors"]:checked').value
;


// Attach all of the controls.
['viewer',
    'start',
    'randcolors',
    'reset',
    'status',
    'northwest',
    'north',
    'northeast',
    'west',
    'east',
    'southwest',
    'south',
    'southeast']
    .map(x => els[x] = document.getElementById(x))


// Initialize the viewer.
initViewer(els.viewer);

// Initialize the Loop by letting it know how we want to talk to the server and compute on the server.
initLoop(0, processors);

// Wire up the controls.  It's all up to the user now to get the mandelbrot party started.
wire(els.status, els.start, els.randcolors, els.reset, els.northwest, els.north, els.northeast, 
        els.west, els.east, els.southwest, els.south, els.southeast);
