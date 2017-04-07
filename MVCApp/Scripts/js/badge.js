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
                url: "Badge/Delete?id=" + deleteId,
                type: "POST",
                success: function (data, status, xhr) {
                    if (data.success != null) {
                        var row = "table#badge_table tr#" + deleteId;
                        $(row).remove();
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
