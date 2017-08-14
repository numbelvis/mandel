/*

    The processing loop.  Facilitates getting the next image from the server and continuing to do so
    until told otherwise.

*/

import { modes as communicationModes } from './comms';
import { modes as processorModes } from './processors';
import { takeNextCommand } from './controls.js';

export { toggle };


// Loop state information

        // Is the loop currently running?
let     running = false,

        framecount = 0,

        generator = null,

        communication = null,

        processors = null
;



/*
    Toggle the loop's running status:  If started, pause.  If paused, start.
*/
export default function toggle() {

    return running ? stop() : start();
}

export function initialize(communicationModeNumber, processingModeNumber) {

    communication = communicationModes.get(communicationModeNumber);
    processors = processorModes.get(processingModeNumber);
}



function start() {

    running = true;
    generator = nextImageGenerator();
    var initial = generator.next();
    return running;
}


function stop() {

    running = false;
    return running;
}


function *nextImageGenerator() {

    // As long as the loop is still set to run, keep going!
    while(running === true) 
    {
        // First ask the server for another image.
        var result1 = yield nextImageRequest();
        
        // Then increase the framecount and return it.
        framecount++;
    }
}


function nextImageRequest() {

    communication.nextImageMethod(generator, processors, takeNextCommand());
}
