//folder id 592546806
var site = function () {
    var siteName, folderID;
    return {
        //properties
        siteName: siteName,
        folderID: folderID
    }
}();

//self-executing anonymous functions with public and private
(function (Site, $, undefined) {
    //private properties
    var p,
        token = 'e2isqprt0i95dtlmagf7evjptyfw9amx';
    //public properties
    Site.siteName = "";
    Site.folderID = "";

    //private methods
    function second() {
    };

    //public methods
    Site.first = function () {
    };

    Site.GetLeaseFileList = function (listView) {       
        var f = "?t=" + token + "&i=" + Site.folderID;
        $.ajaxSetup({ cache: false });
        $.getJSON('Leap/Folder' + f, function (list) {
            //check for error

            //
            $.each(list.entries, function (index, data) {
                if (data.type === "file") {
                    var k = '<div class="mediumListIconTextItem" id="' + data.id + '"><div class="icon-picture mediumListIconTextItem-Image" ></div><div class="mediumListIconTextItem-Detail"><h6 style="color:darkblue;">'
                               + data.name + '</h6></div></div>';
                    listView.append(k);

                }
            });            
        });
    };

    Site.GetFileDetails = function (fileID) {
        var f = "?t=" + token + "&i=" + fileID;
        $.ajaxSetup({ cache: false });
        $.getJSON('Leap/File' + f, function (details) {

        });
    };

    Site.GetDownloadLink = function () {
        var f = "?t=" + token + "&i=" + fileID;
        $.ajaxSetup({ cache: false });
        $.getJSON('Leap/FileDownload' + f, function (details) {

        });
    };    
}(window.Site = window.Site || {}, jQuery));