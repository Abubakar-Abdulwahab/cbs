if (redirecting) {
    var progressDots = document.getElementById("progressDots");
    showProgress();

    var httpRequest;

    function showProgress() {
        var counter = -5;
        var id = setInterval(showProgressDots, 500);

        function showProgressDots() {
            if (counter >= 0) {
                progressDots.innerHTML = ""; counter = -5;
                redirectForInvoiceGeneration(id);
            } else {
                progressDots.innerHTML += "."; counter++;
            }
        }

        function redirectForInvoiceGeneration(){
            clearInterval(id);
            window.location.replace(url);
        }
    }
}
