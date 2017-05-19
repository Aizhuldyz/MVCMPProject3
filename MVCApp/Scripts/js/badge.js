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


                function getStatus() {
                    var sessionUrl = location.origin + "/MVCApp/Admin/SessionHasChanges";
                    return $.getJSON(sessionUrl).then(function(data) {
                            return data.sessionHasChanged;
                        });
                };
               

                $.when(getStatus()).done(function(data) {
                    if (data) {
                        $(window).on("mouseover",
                            (function() {
                                window.onbeforeunload = null;
                            }));
                        $(window).on("mouseout",
                            (function() {
                                window.onbeforeunload = confirmLeave;
                            }));

                        function confirmLeave() {                            
                            return "";
                        }
                    }
                });

            $("#badge_form")
            .on("submit",
                function (e) {
                    e.preventDefault();
                    var isValid = $("#badge_form").valid();
                    if (isValid) {
                        var validator = $("#badge_form").data("validator");
                        var formData = new FormData();
                        var image = $("#uploadedImage").get(0).files[0];
                        formData.append("Image", image);
                        var name = $("#badgeTitle").val();
                        formData.append("Title", name);
                        var description = $("#badgeDescription").val();
                        formData.append("Description", description);
                        //var verToken = $("input[name=__RequestVerificationToken]").val();
                        //formData.append("__RequestVerificationToken", verToken);
                        var apiUri = "http://localhost/MVCApp/api";
                        $.ajax({
                            url: apiUri+ "/award",
                            type: "Post",
                            processData: false,
                            contentType: false,
                            data: formData,
                            dataType: "JSON",
                            success: function (data, status, xhr) {
                                if (data.validationError) {
                                    var errors = $.parseJSON(data.validationError);
                                    validator.showErrors(errors);
                                } else {
                                    $("#badgeTitle").val("");
                                    $("#badgeDescription").val("");
                                    $("#uploadedImage").val("");
                                    $("table#badge_table tr:last").after(formatSingleBadgeRow(data));
                                    checkSessionChanges();
                                }
                            },
                            error: function (xhr, status, error) {
                                alert("Error occured while adding a badge");
                            }
                        });
                    }
                });


            function formatSingleBadgeRow(data) {
                var row = $("<tr>", { id: data.Id });
                row.append($("<td>").append($("<img>",
                    {
                        src: "/MVCApp/Badge/GetImage?badgeId=" + data.Id + "&fileName=" + data.ImageUrl,
                        class: "photoImg"
                    })));
                row.append(
                    $("<td>", { text: data.Title }));
                row.append(
                    $("<td>", { text: data.Description }));
                row.append(
                    $("<td>").append($("<a>", {href: "/MVCApp/award/" + data.Id + "/edit", text: "Edit |"})).append($("<button>",
                        {name: "delete_badge", delete_id: data.Id, class: "btn btn-link", text: "Delete"})));
                return row;
            }

            $(document).on("click", 'button[name="delete_badge"]', function (e) {
                deleteRow(e);
            });


            function deleteRow(e) {
                e.preventDefault();
                var deleteId = $(e.currentTarget).attr("delete_id");
                $.ajax({
                    url: "http://localhost/MVCApp/api/award/" + deleteId,
                    type: "Delete",
                    success: function (data, status, xhr) {
                        if (xhr.status === 200) {
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
