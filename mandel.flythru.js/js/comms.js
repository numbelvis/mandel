/*

    Communication modes:


        0 - server side image with ajax call.

        1 - base64 image data returned thru ajax call.

        2 - base64 image data returned thru web socket.

*/

import { url as setViewerUrl, base64 as setViewerData } from './viewer';

export { modes as default };



export let modes = new Map([

    [0, {

        nextImageMethod: function(generator, processor, command) {

            let xhr = new XMLHttpRequest();
            xhr.addEventListener('load', function() {
            
                // Got the image, generator can move on to the next step!
                generator.next();

                // Refresh the viewer.
                setViewerUrl('/output/frame.jpg?dt=' + __nocache());
            });
            xhr.open('GET', createUrl('/ss_jpg.ashx', processor, command));
            xhr.send();
        }
    }],
    [1, {

        nextImageMethod: function(generator, viewer, command) {

            let xhr = new XMLHttpRequest();
            xhr.addEventListener('load', function() {
            
                // Got the image, generator can move on to the next step!
                generator.next(xhr.responseText);
            });
            xhr.open('GET', createUrl('/ss_jpg.ashx', processor));
            xhr.send();
        }
    }]
]);


function createUrl(url, processor, command) {

    var url = url + '?command=' + command + '&comp=' + processor + '&dt=' + __nocache();
    return url;
}


// Cache destroyer!!
const __nocache = () => new Date().getTime()