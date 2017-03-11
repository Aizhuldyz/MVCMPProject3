$(document)
    .ready(
    $(function() {
        var url = window.location.pathname;
        $('ul.nav a[href="' + url + '"]').parent().addClass("active");
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
                                if (data.rowHtml != null) {
                                    $("table#person_table tr:last").after(data.rowHtml);
                                }
                                else if (data.error != null) {
                                    alert("Error occured while adding new person!");
                                }
                            },
                            error: function(xhr, status, error) {
                                alert("Error occured while adding new person!");
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
                        var row = "table#person_table tr#" + deleteId;
                        $(row).remove();
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
    })
    );
