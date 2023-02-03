$(document).ready(function () {
    $(".view-blob").click(function () {
        let element = $(this);
        let blobInBase64 = element[0].nextElementSibling.value;
        let contentType = element[0].nextElementSibling.nextElementSibling.value;
        window.open(createBlobUrl(blobInBase64, contentType), "View Attachment", "width=800,height=800,scrollbars=yes");
    });

    function createBlobUrl(blob, contentType) {
        const byteCharacters = atob(blob);
        const byteNumbers = new Array(byteCharacters.length);
        for (var i = 0; i < byteCharacters.length; ++i) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const realBlob = new Blob([byteArray], { type: contentType });
        return URL.createObjectURL(realBlob);
    }
});