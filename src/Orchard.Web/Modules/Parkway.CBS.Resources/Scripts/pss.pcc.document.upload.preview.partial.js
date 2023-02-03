$(document).ready(function () {
    $("#documentPreviewModalCloseBtn").click(function () {
        $(".document-preview-modal-container").hide();
        $("#documentPreviewImg").hide();
        $("#documentPreviewEmbed").hide();
    });

    $(".preview-document > img").click(function (event) {
        $("#documentPreviewImg").attr("src", event.currentTarget.src);
        $("#documentPreviewImg").show();
        $(".document-preview-modal-container").show();
    });

    $(".preview-document-embed > .embed-overlay").click(function () {
        $("#documentPreviewEmbed").attr("src", $("#passportDatapageThumbnail").attr("src"));
        $("#documentPreviewEmbed").show();
        $(".document-preview-modal-container").show();
    });
});