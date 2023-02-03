$(document).ready(function () {
    $('a[name="ViewApprovals"]').click(function (e) {
        e.preventDefault();
        window.open(this.href, "cbsinvoice", "width=800,height=800,scrollbars=yes");
    });
});