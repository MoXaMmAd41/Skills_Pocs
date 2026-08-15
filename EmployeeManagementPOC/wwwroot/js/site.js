$(document).ajaxStart(function () {
    $("body").addClass("loading");
});

$(document).ajaxStop(function () {
    $("body").removeClass("loading");
});

$(document).ajaxError(function (event, jqXHR, settings, error) {

    console.log("HTTP Interceptor triggered.");
    console.log("Status:", jqXHR.status);
    console.log("URL:", settings.url);

    switch (jqXHR.status) {

        case 401:
            window.location.href = "/Account/Login";
            break;

        case 403:
            window.location.href = "/Account/AccessDenied";
            break;

        case 404:
            showHttpError(
                "The requested resource was not found."
            );
            break;

        case 500:
            showHttpError(
                "An unexpected server error occurred."
            );
            break;

        default:
            showHttpError(
                "An unexpected error occurred."
            );
            break;
    }
});

function showHttpError(message) {

    $(".global-http-error").remove();

    const alertHtml = `
        <div
            class="alert alert-danger alert-dismissible fade show
                   global-http-error position-fixed
                   bottom-0 start-50 translate-middle-x
                   mb-4 shadow"
            role="alert"
            style="
                z-index: 9999;
                min-width: 350px;
                max-width: 600px;
            ">

            <strong>Error:</strong>
            ${message}
        </div>
    `;

    $("body").prepend(alertHtml);

    setTimeout(function () {

        $(".global-http-error").fadeOut(300, function () {
            $(this).remove();
        });

    }, 5000);
}


