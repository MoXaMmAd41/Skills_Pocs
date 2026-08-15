$(document).ready(function () {

    function loadEmployees(page = 1) {

        const form = $("#employeeFilterForm");

        const search = $("#searchInput").val();
        const departmentId = $("#departmentFilter").val();
        const isActive = $("#statusFilter").val();
        const sortBy = $("#sortFilter").val();

        $.ajax({
            url: "/Employees/Search",
            type: "GET",
            data: {
                Search: search,
                DepartmentId: departmentId,
                IsActive: isActive,
                SortBy: sortBy,
                Page: page,
                PageSize: 10
            },
            beforeSend: function () {
                $("#employeeTableContainer").addClass("loading");
            },
            success: function (html) {
                $("#employeeTableContainer").html(html);
            },
            error: function () {
                alert("Failed to load employees.");
            },
            complete: function () {
                $("#employeeTableContainer").removeClass("loading");
            }
        });
    }

    $("#employeeFilterForm").on("submit", function (event) {

        event.preventDefault();

        loadEmployees(1);
    });

    $("#searchInput").on("input", function () {

        clearTimeout(window.employeeSearchTimeout);

        window.employeeSearchTimeout = setTimeout(function () {
            loadEmployees(1);
        }, 400);
    });

    $("#departmentFilter, #statusFilter, #sortFilter")
        .on("change", function () {
            loadEmployees(1);
        });

    $(document).on("click", ".page-button", function () {

        const page = $(this).data("page");

        loadEmployees(page);
    });
});