/*

    Allows the viewer to be interacted with by others.

*/

export { initialize };


// The viewer element.
let viewerEl = null;


// Initialize the viewer.
export default function initialize(vEl, initialMsg = 'Ready Freddie') {
    viewerEl = vEl;
}


export function url(l) {

    viewerEl.src = '/output/frame.jpg?dt=' + new Date().getTime();
}


export function base64(l) {

    viewerEl.innerHTML = l + 'BASS ROB BASS!!';
}
