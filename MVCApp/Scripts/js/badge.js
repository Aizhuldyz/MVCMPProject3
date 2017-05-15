$(document)
    .ready(
    $(function () {

        $(document).on("click", 'button[name="delete_badge"]', function (e) {
            deleteRow(e);
        });

        function deleteRow(e) {
            e.preventDefault();
            var deleteId = $(e.currentTarget).attr("delete_id");
            $.ajax({
                url: "award/" + deleteId + "/delete",
                type: "POST",
                success: function (data, status, xhr) {
                    if (data.success != null) {
                        var row = "table#badge_table tr#" + deleteId;
                        $(row).remove();
                        alert("Badge was successfully deleted");
                        $.getJSON("Admin/SessionHasChanges", function (data) {
                            if (data != null) {
                                if (data.sessionHasChanged) {
                                    $("#session_changes").show();
                                } else {
                                    $("#session_changes").hide();
                                }
                            }
                        });
                    } else {
                        alert("Error occured while deleting a badge!");
                    }
                },
                error: function (xhr, status, error) {
                    alert("Error occured while deleting a badge!");
                }
            });
        }
    }));
