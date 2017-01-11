window.$mandel = window.$mandel ||

{
    // Constants

    // Directory containing html templates relative to this file.
    _html_location: '/assets/html',


    // The viewer's image display mode
    /*
            'stretch': The image is stretched to the viewer size

            'fill': unsupported: The server will return an image that exactly fills the viewer, pixel per pixel
            'letterbox': unsupported: A specific image size is rendered and letterboxed in the viewer to fit completely at any viewer size
    */
    _viewer_mode: 'stretch',


    // Frames since last check by the frame counter
    __frames: 0,

    // Indicates we are paused and not generating images.
    __paused: false,

    // Last framecount
    __framecount: 0,

    // Indicates we are waiting for an image to be generate by the server.
    __waiting: false,

    // The next command to send.
    __next_cmd: null,



    // Quick reference to some elements.
    $viewer: document.getElementById('viewer'),
    $image: document.getElementById('main-image'),
    $title: document.getElementById('title-section'),


    // This is the entry point for the mandel application.
    main: function (area_title) {

        // Perform some setup.....

        // First attach a resize listener for the viewer and do an initial adjustment.
        $mandel.attachWindowResizeHandlerForViewer();
        $mandel.adjustViewertoWindow();

        // Load the menu image into the viewer.
        $mandel.loadViewerFromUrl('/assets/img/starter-image.jpg');

        // Set the area text.
        if (area_title)
            document.getElementById('area-text').innerHTML = ':' + area_title;
    },





    //  REGION:             Loading image into the viewer

    loadViewerFromUrl: function (url) {

        var image = new Image();
        image.addEventListener('load', function () {

            if ($mandel._viewer_mode == 'stretch') {

                // do nothing, the css handles it.
            }
            $mandel.$image.src = image.src;
        });
        image.src = url;
    },




    //  REGION:             Resizing of viewport and viewer

    attachWindowResizeHandlerForViewer: function () {

        window.addEventListener('resize', $mandel.adjustViewertoWindow);
    },

    adjustViewertoWindow: function() {

        var height = window.innerHeight - 81,
            height_px = height + 'px'
        ;

        $mandel.$viewer.style['max-height'] = viewer.style['height'] = height_px;
    },

    


    // REGION:              FPS Counter
    fpsCounterStart: function () {

        setInterval(function () {
            var frames = $mb.__framecount;
            $mb.__framecount = 0;

            document.getElementById('fps').innerHTML = '' + frames + ' FPS';
        }, 1000);
    },



    //  REGION:             Commands
    command: function(cmd) {

        $mandel.__next_cmd = cmd;
    },

    colors: function () {

        $mandel.command('colors');
    },
    capture: function () {

        $mandel.command('capture');
    },
    north: function () {

        $mandel.command('north');
    },
    south: function () {

        $mandel.command('south');
    },
    west: function () {

        $mandel.command('west');
    },
    east: function () {

        $mandel.command('east');
    },
    northeast: function () {

        $mandel.command('northeast');
    },
    northwest: function () {

        $mandel.command('northwest');
    },
    southeast: function () {

        $mandel.command('southeast');
    },
    southwest: function () {

        $mandel.command('southwest');
    },

    faster: function() {

        $mandel.command('faster');
    },

    slower: function () {

        $mandel.command('slower');
    },

    reset: function () {

        $mandel.command('reset');
    },




    //  REGION:             Processing loop

    start: function (btn) {
        console.log('Start');
        if (btn) {

            btn.className += ' hidden';

            var p = document.getElementById('pause');
            p.className = p.className.replace('hidden', '');
        }

        var id = setInterval(function () {

            if ($mandel.__paused === true || $mandel.__waiting === true)
                return;

            $mandel.__waiting = true;

            $mandel.send(function () {

                $mandel.__waiting = false;
                $mandel.__framecount++;
            })
        }, 100);
    },

    pause: function (btn) {

        $mandel.__paused = $mandel.__paused === true ? false : true;
        btn.innerHTML = $mandel.__paused === true ? '[unpause]' : '[pause]';
    },

    send: function (callback) {

        var cmd = $mandel.__next_cmd;
        $mandel.__next_cmd = null;

        var payload = 'command=' + cmd + '&timestamp=' + new Date().getTime();
        var request = new XMLHttpRequest();
        request.onreadystatechange = function () {

            if (request.readyState == 4 && request.status == 200) {

                var url = request.responseText;
                $mandel.$image.src = '/output/frame.png?timestamp=' + new Date();
                callback();
                $mandel.__frames++;
            }
        };

        request.open('POST', '/ss_jpg.ashx');
        request.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        request.send(payload);
    },





    //  REGION:              Html Snippet Loading, Displaying

    loadHtmlSnippetInto: function(url_inside_html_location, element_id) {

        $mandel.loadHtmlSnippet(url_inside_html_location, function (html) {

            var el = document.getElementById(element_id);
            if (el)
                el.innerHTML = html;
        });
    },

    // Loads a file from the html location and hands it to the callback.
    loadHtmlSnippet: function(url_inside_html_location, callback) {

        var url = $mandel._html_location + '/' + url_inside_html_location;
        $mandel.loadHtml(url, callback);
    },

    // Loads html from the url and hands it to the callback.
    loadHtml: function (url, callback) {

        var xhr = new XMLHttpRequest();
        xhr.open('GET', url);

        xhr.onreadystatechange = function () {

            if (xhr.readyState == 4 && xhr.status == 200) {

                callback(xhr.responseText);
            }
            else {

                callback(null);
            }
        };

        xhr.send();
    }
};

// Call with $mandel as the this object for fun and ease
window.$mandel.main('main-menu');