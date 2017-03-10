$(document)
    .ready(function() {
        $("#btn_add")
            .on('click',
                function(e) {
                    e.preventDefault();
                    var name = $("#person_name").val();
                    var birthdate = $("#birthdate").val();
                    $.ajax({
                        url: "Person/Add",
                        type: "POST",
                        data: {
                            name: name,
                            birthdate: birthdate
                        },
                        datatype:"json",
                        success: function(data, status, xhr) {
                        },
                        error: function(xhr, status, error) {
                            alert("Error occured while adding new person!");
                        }
                    });
                });

        $("#delete_person").on("click", function (e) {
            e.preventDefault(); 
            $.ajax({
                url: this.href,
                type: "POST",
                success: function (data, status, xhr) {
                },
                error: function (xhr, status, error) {
                    alert("Error occured while deleting a person!");
                }
            });
        });
    });
