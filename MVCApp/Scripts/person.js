$(document)
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
                        $.ajax({
                            url: "Person/Create",
                            type: "POST",
                            processData: false,
                            contentType: false,
                            data: formData,
                            datatype: "json",
                            success: function (data, status, xhr) {                                
                                    $("#person_name").val("");
                                    $("#birthdate").val("");
                                    $("#uploadedPhoto").val("");
                                    $("table#person_table tr:last").after(data);                                    
                            },
                            error: function(xhr, status, error) {
                                var errors = $.parseJSON(error);
                                validator.showErrors(errors);
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
                url: "Person/Delete?id=" + deleteId,
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
            var formData = new FormData();
            var editId = $(e.currentTarget).attr("edit_id");
            var selectorName = "tr#" + editId + " td[name=name]";
            var name = $(selectorName).html();
            formData.append("Name", name);            
            var selectorBday = "tr#" + editId + " td[name=birthDate]";
            var birthDate = $(selectorBday).html();            
            formData.append("BirthDate", birthDate);
            $.ajax({
                url: "Person/Edit",
                type: "Post",
                processData: false,
                contentType: false,
                data: formData,
                datatype: "json",
                success: function (data, status, xhr) {
                    $("#createForm").html(data);
                }
            });
        }
    })
    );
