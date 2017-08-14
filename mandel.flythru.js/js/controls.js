/*

    Control wire-up and functionality used to interface with the loop.

*/

import { toggle } from './loop';

export { wire };


////////////////////////            Locals
let 
    
    // Element representing each of the controls
    buttons = {},

    // The element to which the status is innerHTMLed.
    status = null,

    // The next command to send to the server.
    next_command = null,


    // Holds the id of the interval for the start button's initial flicker.
    flicker_id = null
;

////////////////////////            Public

export default function wire(statusEl, start, randcolors, reset, nw, n, ne, w, e, sw, s, se) {

    status = statusEl;
    buttons = { start, randcolors, reset, nw, n, ne, w, e, sw, s, se };

    // Hook up them buttons!
    addCommandClick(randcolors, 'colors');
    addCommandClick(reset, 'reset');
    addCommandClick(nw, 'northwest');
    addCommandClick(n, 'north');
    addCommandClick(ne, 'northeast');
    addCommandClick(w, 'west');
    addCommandClick(e, 'east');
    addCommandClick(sw, 'southwest');
    addCommandClick(s, 'south');
    addCommandClick(se, 'southeast');


    // Deal with the start button LAST.
    addClick(start, startpause_onclick.bind(this));
    start.style = start.style || { };
    start.style.color = 'green'
    flicker_id = setInterval(function() {

        if(start.style.color == 'green')
            start.style.color = 'black';
        else 
            start.style.color = 'green';
    }, 1000);

}

export function takeNextCommand() {

    var n = next_command;
    next_command = null;
    return n;
}



////////////////////////                Private
function addClick(el, fn) {

    el.addEventListener('click', fn);
}

function startpause_onclick(e) {

    if(flicker_id) clearInterval(flicker_id);

    var is_now_running = toggle();
    buttons.start.value = is_now_running ? 'pause' : 'Restart';
    status.innerHTML = is_now_running ? 'Running' : 'Paused';
}

function setCommand(cmd) {
    next_command = cmd;
}

function addCommandClick(el, command) {
    addClick(el, () => { setCommand(command) });
}