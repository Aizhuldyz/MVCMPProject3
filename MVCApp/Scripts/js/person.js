﻿$(document)
    .ready(
    $(function() {
        var url = window.location.pathname;
        $('ul.nav a[href="' + url + '"]').parent().addClass("active");


        $("#person_form")
            .on("submit",
                function(e) {
                    e.preventDefault();
                    var isValid = $("#person_form").valid();
                    if (isValid) {
                        var validator = $("#person_form").data("validator");
                        var formData = new FormData();
                        var photo = $("#uploadedPhoto").get(0).files[0];
                        formData.append("Photo", photo);
                        var name = $("#person_name").val();
                        formData.append("Name", name);
                        var birthdate = $("#birthdate").val();
                        formData.append("BirthDate", birthdate);
                        var verToken = $("input[name=__RequestVerificationToken]").val();
                        formData.append("__RequestVerificationToken", verToken);
                        $.ajax({
                            url: "Person/Create",
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
                                    $("#person_name").val("");
                                    $("#birthdate").val("");
                                    $("#uploadedPhoto").val("");
                                    $("table#person_table tr:last").after(data);
                                }
                            },
                            error: function(xhr, status, error) {
                                alert("Error occured while adding a person");
                            }
                        });
                    }
                });

        $(document).on("click", 'button[name="delete_person"]', function(e) {
            deleteRow(e);
        });

        function deleteRow(e) {
            e.preventDefault();
            var deleteId = $(e.currentTarget).attr("delete_id");
            $.ajax({
                url: "user/" + deleteId + "/Delete",
                type: "POST",
                success: function (data, status, xhr) {
                    if (data.success != null) {
                        location.reload();
                    }
                    else {
                        alert("Error occured while deleting a person!");
                    }
                },
                error: function (xhr, status, error) {
                    alert("Error occured while deleting a person!");
                }
            });
        }
        $(document).on("click", 'button[name="edit_person"]', function (e) {
            editRow(e);
        });

        function editRow(e) {
            e.preventDefault();
            var editId = $(e.currentTarget).attr("edit_id");           
            $.ajax({
                url: "Person/Edit?id=" + editId,
                type: "GET",
                success: function (data, status, xhr) {
                    $("#createForm").html(data);
                },
                error: function(data, status, err) {
                    alert("error while rendering edit form");
                }
            });
        }

        $(document).on("click", 'button[name="add_new_badge"]', function (e) {
            addNewBadge(e);
        });

        function addNewBadge(e) {
            e.preventDefault();
            var personId = $(e.currentTarget).attr("person_id");
            var personName = $(e.currentTarget).attr("person_name");
            $.ajax({
                url: "Person/AddRecognition",
                type: "GET",
                data: {
                    personId: personId,
                    personName: personName
                    
                },
                success: function (data, status, xhr) {
                    $("#createForm").html(data);
                },
                error: function (data, status, err) {
                    alert("error while rendering add badge form");
                }
            });
        }

        $(document).on("click", "img", function(e) {
            showBadgeModal(e);
        });

        function showBadgeModal(e) {
            e.preventDefault();
            var badgeId = $(e.currentTarget).attr("img_id");
            $.ajax({
                url: "/MVCApp/Badge/GetBadgeInfo?id=" + badgeId,
                type: "GET",
                success: function (data, status, xhr) {
                    $("#badgeInfo").html(data);
                    $("#badgeModal").css("display", "block");
                },
                error: function (data, status, err) {
                    alert("error while rendering badge info modal");
                }
            });
        }

        $(document).on("click", "#close_badge_modal", function (e) {
            $("#badgeModal").css("display", "none");
        });

    }));

$(document).on("submit", "#person_form_edit", function (e) {
    e.preventDefault();
    $('#person_form_edit').removeData("validator");
    $.validator.unobtrusive.parse($('#person_form_edit'));
        var isValid = $("#person_form_edit").valid();
        if (isValid) {
            var validator = $("#person_form_edit").data("validator");
            var formData = new FormData();
            var id = $("#person_name").attr("edit-id");
            formData.append("Id", id);
            var photo = $("#uploadedPhoto").get(0).files[0];
            formData.append("Photo", photo);
            var name = $("#person_name").val();
            formData.append("Name", name);
            var birthdate = $("#birthdate").val();
            formData.append("BirthDate", birthdate);
            var verToken = $("input[name=__RequestVerificationToken]").val();
            formData.append("__RequestVerificationToken", verToken);
            var deletePhoto = $("#delete_check").prop("checked");
            if (deletePhoto == undefined)
                deletePhoto = false;
            formData.append("DeletePhoto", deletePhoto);
            var apiUri = "http://localhost/MVCApp/api";
            $.ajax({
                url: apiUri + "/user/" + id,
                type: "PUT",
                processData: false,
                contentType: false,
                data: formData,
                datatype: "json",
                success: function (data, status, xhr) {
                    if (xhr.status === 201) {
                        location.reload();
                    } else {
                        alert("Error Occured while updating person");
                    }
                },
                error: function(xhr, status, error) {
                    alert("Error Occured while updating person");
                }
            });
        }
});

$(document).on("submit", "#add_badge_form", function (e) {
    e.preventDefault();
    $('#add_badge_form').removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($('#add_badge_form'));
    $('#add_badge_form').validate();
    var validator = $("#add_badge_form").data("validator");
    var personId = $("#person_name").attr("person-id");
    var selectedBadgeId = $("#badge_dropdown").val();
    var verToken = $("input[name=__RequestVerificationToken]").val();
    $.ajax({
        url: "http://localhost/MVCApp/api/user/" + personId + "/award/" + selectedBadgeId,
        type: "POST", 
        data: {
            userId : personId,
            awardId: selectedBadgeId,
            __RequestVerificationToken: verToken
        },
        datatype: "json",
        success: function (data, status, xhr) {
            if (xhr.status === 200) {
                location.reload();
            }
        },
        error: function (xhr, status, error) {
            if (xhr.status === 400) {
                var errorData = $.parseJSON(xhr.responseText);
                var errors ={
                    "Badges": errorData.Message
                }
                validator.showErrors(errors);
            } else {
                alert("Error Occured while adding a badge");
            }
        }
        });
});
