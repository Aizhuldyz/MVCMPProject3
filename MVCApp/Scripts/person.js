$(document)
    .ready(
    $(function() {

        $.validator.addMethod("birthdate", function (value) {
            var currentYear = new Date().getFullYear();
            var year = value.split('/');
            if (value.match(/^\d\d?\/\d\d?\/\d\d\d\d$/) && parseInt(year[2]) <= currentYear
                && parseInt(year[0]) <= 31 && parseInt(year[1]) <= 12)
                return true;
            else
                return false;
        });

        $("#person_form").validate({
            rules: {
                name: "required",
                lastname: "required",
                birthdate: {
                    required: true,
                    birthdate: true
                }
            },
            messages: {
                name: "Please enter your firstname and lastname",
                birthdate: "Please enter a valid birthdate in format dd/mm/yyyy"
            }
        });

        $("#btn_add")
            .on('click',
                function(e) {
                    e.preventDefault();
                    var isValid = $("#person_form").valid();
                    if (isValid) {
                        var name = $("#person_name").val();
                        var birthdate = $("#birthdate").val();
                        $.ajax({
                            url: "Person/Add",
                            type: "POST",
                            data: {
                                name: name,
                                birthdate: birthdate
                            },
                            datatype: "json",
                            success: function (data, status, xhr) {
                                $("#person_name").val("");
                                $("#birthdate").val("");
                                if(data.rowHtml != null)
                                    $("table#person_table tr:last").after(data.rowHtml);
                                return;
                            },
                            error: function(xhr, status, error) {
                                alert("Error occured while adding new person!");
                            }
                        });
                    }
                });

        $('button[name="delete_person"]').on("click", function (e) {
            e.preventDefault();
            var deleteId = $(this).attr("delete_id");
            $.ajax({
                url: "Person/Delete?id=" + deleteId,
                type: "POST",
                success: function (data, status, xhr) {
                    var row = "table#person_table tr#" + deleteId;
                    $(row).remove();
                },
                error: function (xhr, status, error) {
                    alert("Error occured while deleting a person!");
                }
            });
        });
    }));
