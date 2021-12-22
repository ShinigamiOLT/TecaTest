// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
console.log("Hola Mundo");
$(function () {

    $.ajaxSetup({ cache: false });

    $("a[data-modal]").on("click", function (e) {
        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');

        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
          
            bindForm(this);
        });
        return false;
    });
});




function mimodalfuncion() {
    $.ajaxSetup({ cache: false });
    $(".jsgrid-cell a[data-modal]").on("click", function (e) {
        // hide dropdown if any (this is used wehen invoking modal from link in bootstrap dropdown )
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');

        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                /*backdrop: 'static',*/
                keyboard: true
            }, 'show');
       
            bindForm(this);
        });
        return false;
    });
}



$('#myModal').on('hidden.bs.modal', function () {
    // do something ...
    $('#myModalContent').html("<h1>No implementado</h1>");
});

function bindForm(dialog) {
    $('form', dialog).submit(function () {
        $('#progress').show();
        var form = $(this);
        if (form.validate().checkForm()) {
            $('.progress .progress-bar').css("width",
                function () {
                    return $(this).attr("aria-valuenow") + "%";
                }
            );

            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    console.log(result);
                    if (result.success) {
                        $('#myModal').modal('hide');
                        $('#progress').hide();
                        $('#myModalContent').html("<h1>No implementado</h1>");
                        if (result.funcion) {
                            var elemento = $("#" + result.grid);
                            elemento.jsGrid("loadData");
                            return true;
                        }

                        if (result.url != undefined) {
                            var remplazar = $('#replacetarget').html();

                            if (remplazar != undefined) {
                                $('#replacetarget').load(result
                                    .url); //  Load data from the server and place the returned HTML into the matched element
                                solotable();
                            } else
                                window.location.href = result.url;
                        } else
                            location.reload(result.reload);
                    } else {
                        $('#progress').hide();
                        $('#myModalContent').html(result);
                        bindForm();
                    }
                    return true;
                }
            });
        } else {
            $('#progress').hide();

        }
        return false;
    });
}

   
