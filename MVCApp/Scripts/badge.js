$(document)
    .ready(
    $(function () {
        $("#badge_form").validate({
            rules: {
                title: "required",
                description: "required"
            },
            messages: {
                name: "Please enter title for a badge",
                description: "Please enter description for a badge"
            }
        });

        $("#btn_add_badge")
            .on('click',
                function (e) {
                    e.preventDefault();
                    var isValid = $("#badge_form").valid();
                    if (isValid) {
                        var title = $("#badge_title").val();
                        var description = $("#description").val();
                        $.ajax({
                            url: "Badge/Add",
                            type: "POST",
                            data: {
                                title: title,
                                description: description
                            },
                            datatype: "json",
                            success: function (data, status, xhr) {
                                $("#badge_title").val("");
                                $("#description").val("");
                                if (data.rowHtml != null) {
                                    $("table#badge_table tr:last").after(data.rowHtml);
                                } else {
                                    alert("Error occured while adding new badge!");
                                }
                            },
                            error: function (xhr, status, error) {
                                alert("Error occured while adding new badge!");
                            }
                        });
                    }
                });

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
