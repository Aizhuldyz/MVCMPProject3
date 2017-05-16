$(document)
    .ready(
    $(function () {
        checkSessionChanges();
        function checkSessionChanges(e) {
            var sessionUrl = location.origin + "/MVCApp/Admin/SessionHasChanges";

            $.getJSON(sessionUrl,
                function (data) {
                    if (data != null) {
                        if (data.sessionHasChanged) {
                            $("#session_changes").show();
                            $("#session_changes").attr("block_quit", true);
                        } else {
                            $("#session_changes").hide();
                            $("#session_changes").attr("block_quit", false);
                        }
                    }
                });
        }
        $("#badge_form")
            .on("submit",
                function (e) {
                    e.preventDefault();
                    var isValid = $("#badge_form").valid();
                    if (isValid) {
                        var validator = $("#badge_form").data("validator");
                        var formData = new FormData();
                        var photo = $("#uploadedImage").get(0).files[0];
                        formData.append("Image", photo);
                        var name = $("#badgeTitle").val();
                        formData.append("Title", name);
                        var description = $("#badgeDescription").val();
                        formData.append("Description", description);
                        var verToken = $("input[name=__RequestVerificationToken]").val();
                        formData.append("__RequestVerificationToken", verToken);
                        $.ajax({
                            url: "Badge/Add",
                            type: "POST",
                            processData: false,
                            contentType: false,
                            data: formData,
                            datatype: "json",
                            success: function (data, status, xhr) {
                                if (data.validationError) {
                                    var errors = $.parseJSON(data.validationError);
                                    validator.showErrors(errors);
                                } else {
                                    $("#badgeTitle").val("");
                                    $("#badgeDescription").val("");
                                    $("#uploadedImage").val("");
                                    $("table#badge_table tr:last").after(data);
                                    checkSessionChanges();
                                }
                            },
                            error: function (xhr, status, error) {
                                alert("Error occured while adding a badge");
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
