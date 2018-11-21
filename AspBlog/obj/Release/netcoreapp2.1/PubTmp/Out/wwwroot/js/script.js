$(document).ready(function ($) {

    /*
     *  FLASH MESSAGES
     */

    function autoHide() {
        setTimeout(function () {
            $('.alert').slideUp();
        }, 2500);
    }

    $('.alert').hide().slideDown();
    //autoHide();

    //HIDE ALERT AFTER CLICKING IT
    $('.alert').on('click', function () {
        $(this).delay(300).slideUp();
    });


    /*
     *   ADD FORM,EDIT FORM
     */
    var focused_input = $('#add-form, #edit-form').find('input[name=title]');

    focused_input.focus().val(focused_input.val());

    $('#add-form,#edit-form')
        .find('.add-files a').on('click', function () {
            var input = $(this).prev();
            input.clone().insertAfter(input);
        })
        .end();


    /*
     *   DELETE FORM
     */

    $('#delete-form').on('submit', function () {
        return confirm("u sure 'bout that ?");
    });


    //
    // /*
    //  * ADD FORM
    //  */
    //
    // $('.add-post input:first-child').focus();
    //
    // $('.add-tag').on('click',function(){
    //
    //     var newTag = '<input type="text" name="tags[]" class="tag-form form-control">';
    //
    //     $(newTag).insertBefore($(this));
    // });

    /* AJAX COMMENTS */
    var discussion = $('#discussion');
    discussion.find('form').on('submit', function (event) {

        var form = $(this);

        var req = $.ajax({
            url: form.attr('action'),
            type: 'post',
            data: form.serialize()
        });

        req.done(function (data) {

            $.ajax({
                url: 'comment/' + data.id,
                type: 'get',
                success: function (html) {
                    var li = $(html).hide();
                    discussion.find('.comments').append(li);
                    li.fadeIn();
                }
            });

        });

        form.find('textarea').val('').focus();

        event.preventDefault();
    });

}(jQuery));