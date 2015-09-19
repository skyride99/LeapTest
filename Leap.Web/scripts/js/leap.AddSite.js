var registration = function () {
    var token, folderID,
        CreateSite = function (siteName) {
            var folder = {
                t: 'e2isqprt0i95dtlmagf7evjptyfw9amx',
                n: siteName,
                p: '592538982' //Chicago folder
            };
            var s = "?t=" + folder.t + "&n=" + folder.n + "&p=" + folder.p;            
            $.post("Leap/Folder" + s, function (site) {
                //create discussion with initial comment
                var disc = {
                    t: folder.t,
                    i: site.id
                };
                $('#messageModal').html('created box folder');
                var p = "?t=" + disc.t + "&i=" + disc.i;
                $.post("Leap/Discussion" + p, function (dis) {
                    $('#messageModal').html('created box discussion');
                });
                //check for error

                //copy documents over to folder
                $.each($('.selected'), function (index, data) {
                    var r = "?t=" + disc.t + "&i=" + disc.i + "&f=" + data.id;
                    $.post("Leap/CopyFile" + r, function (cpy) {
                        $('#messageModal').html('copied documents into box folder');
                    });
                });
                
            });
        },

        Upload = function (e) {
            var upload = {
                t: 'j4aa2x13bgm1ccu1vv3ffatlstqqry14',
                i: "493991438", //folderID
                n: "blah.txt"
            };
            e.data = upload;
        },
        
        GetLeaseFileList = function () {
            var files = {
                t: 'e2isqprt0i95dtlmagf7evjptyfw9amx',
                n: '565154116' //folder if of files
            };
            var f = "?t=" + files.t + "&i=" + files.n;
            $.ajaxSetup({ cache: false });
            $.getJSON('Leap/Folder' + f, function (list) {
                //check for error

                //for each file object get the file metadata
                $.each(list.entries, function (index, data) {
                    //get the file metadata
                    if (data.type === "file") {
                        var d = "?t=" + files.t + "&i=" + data.id;
                        $.getJSON('Leap/Files' + d, function (fob) {
                            var k = '<div class="mediumListIconTextItem" id="' + fob.id + '"><div class="icon-picture mediumListIconTextItem-Image" ></div><div class="mediumListIconTextItem-Detail"><h6 style="color:darkblue;">'
                            + fob.name + '</h6></div></div>';
                            $("#siteDocuments").append(k);
                            //var k = fob.name + fob.description + fob.item_status + fob.etag;
                        });
                    }
                });
            });
        };

     return {
         //properties
         token: token,
         folderID: folderID,
         //methods
         CreateSite: CreateSite,
         Upload: Upload,
         GetLeaseFileList: GetLeaseFileList
        };
}();

$(document).ready(function () {
    $('#addSite').on("click", function (e) {
        //show message box
        $('#submitModal').modal();

        var opts = {
            lines: 13, // The number of lines to draw
            length: 7, // The length of each line
            width: 4, // The line thickness
            radius: 10, // The radius of the inner circle
            corners: 1, // Corner roundness (0..1)
            rotate: 0, // The rotation offset
            color: 'white', // #rgb or #rrggbb
            speed: 1, // Rounds per second
            trail: 60, // Afterglow percentage
            shadow: false, // Whether to render a shadow
            hwaccel: false, // Whether to use hardware acceleration
            className: 'spinner', // The CSS class to assign to the spinner
            zIndex: 2e9, // The z-index (defaults to 2000000000)
            top: '1em', // Top position relative to parent in px
            left: 'auto' // Left position relative to parent in px
        };
        var target = document.getElementById('spinnerModal');
        var spinner = new Spinner(opts).spin(target);
        var n = $("#siteName").val() + ' - ' + $("#siteScope").val() + ' - ' + $("#scopeDescription").val();
        $('#submitHeader').html(n);
        $('#messageModal').html('creating site');
        registration.CreateSite(n);
    });

    //$('#share').on("click", function (e) {
       
    //});

    $('#siteDocuments').on('click', '.mediumListIconTextItem', function (e) {
        e.preventDefault();
        $(this).toggleClass('selected');
    });
    $('#continue').on('click', function (e) {       
        window.location = "../site.html"
    });
    
    registration.GetLeaseFileList();
    //dynamic list view
    
});