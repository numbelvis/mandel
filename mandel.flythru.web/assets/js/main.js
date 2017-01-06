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