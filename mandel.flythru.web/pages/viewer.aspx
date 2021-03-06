﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="viewer.aspx.cs" Inherits="mandel.flythru.web.pages.viewer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mandelbrot Flythru</title>
    <link href="/assets/style/base.css" rel="stylesheet" />
</head>
<body>
    <div id="title-section">
        <span id="title-text">mandel</span>
        <span id="area-text"></span>
    </div>
    <div id="viewer">
        <img id="main-image" data-viewer-mode="stretch" />
        <div id="status" class="nice-wrap"><div class="status-title">Status: </div><div class="status-status status-not-running">not-running</div></div>
        
        <!-- start and pause/unpause button -->
        <div id="start" onclick="$mandel.start(this)" class="btn nice-wrap">start</div>
        <div id="pause" onclick="$mandel.pause(this)" class="hidden btn nice-wrap">pause</div>

        <!-- zoom speed -->
        <div id="zoom-title" class="nice-wrap">Zoom:</div>
        <div id="faster" onclick="$mandel.faster()" class="btn nice-wrap">faster</div>
        <div id="slower" onclick="$mandel.slower()" class="btn nice-wrap">slower</div>

        <!-- location -->
        <div id="location-title" class="nice-wrap">Location:</div>
        <div id="location-capture" onclick="$mandel.capture()" class="btn nice-wrap">capture</div>

        <!-- color -->
        <div id="color-title" class="nice-wrap">Color:</div>
        <div id="color-random" class="btn nice-wrap">random</div>

        <!-- directional controls -->
        <div id="west" onclick="$mandel.west()" class="dir-btn nice-wrap">&#x2190;</div>
        <div id="east" onclick="$mandel.east()" class="dir-btn nice-wrap">&#x2192;</div>
        <div id="north" onclick="$mandel.north()" class="dir-btn nice-wrap">&#x2191;</div>
        <div id="south" onclick="$mandel.south()" class="dir-btn nice-wrap">&#x2193;</div>
        <div id="northeast" onclick="$mandel.northeast()" class="dir-btn nice-wrap">&#x2197;</div>
        <div id="northwest" onclick="$mandel.northwest()" class="dir-btn nice-wrap">&#x2196;</div>
        <div id="southeast" onclick="$mandel.southeast()" class="dir-btn nice-wrap">&#x2198;</div>
        <div id="southwest" onclick="$mandel.southwest()" class="dir-btn nice-wrap">&#x2199;</div>
    </div>


    <!-- Scripts -->
    <script src="/js/<%= ScriptLocation %>"></script>
    <script src="../js/mandeltest.js"></script>


    <form id="frmMain" runat="server">
        <script type="text/javascript"">
            $mandel('<%= ModeNumber %>', 'server-side-image', 'main-image');
        </script>
    </form>
</body>
</html>
