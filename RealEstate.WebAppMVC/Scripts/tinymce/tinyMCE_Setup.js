
$(document).ready(function () {
    window.tinymce.init({
        plugins: 'link table lists advlist',
        menu: {
            edit: { title: "Редактиране", items: 'undo redo | cut copy paste pastetext | selectall' },
            format: { title: "Форматиране", items: 'bold italic underline strikethrough superscript subscript' },
            insert: { title: "Вмъкни", items: 'link table | specializedcharacter horizontalline' }
        },
        menubar: 'edit format insert',
        selector: '.full-textarea',
        branding: false,
        statusbar: false,
        language: 'bg_BG',
        height: '400',
        toolbar:
            ' styleselect ' +
            '| bold italic backcolor  ' +
            '| alignleft aligncenter alignright alignjustify ' +
            '| bullist numlist outdent indent ',
        style_formats: [
            { title: 'Заглавие', block: 'h1' },
            { title: 'Заглавие', block: 'h2' },
            { title: 'Заглавие', block: 'h3' },
            { title: 'Заглавие', block: 'h4' },
            { title: 'Заглавие', block: 'h5' },
            { title: 'Заглавие', block: 'h6' }
        ],
        setup: function (editor) {
            editor.on('change',
                function () {
                    editor.save();
                });
        }
    });

    window.tinymce.init({
        menubar: false,
        selector: '.min-textarea',
        branding: false,
        statusbar: false,
        language: 'bg_BG',
        toolbar:
            '| bold italic backcolor  ' +
            '| alignleft aligncenter alignright alignjustify ',
        setup: function (editor) {
            editor.on('change',
                function () {
                    editor.save();
                });
        }
    });

});

window.tinyHelpers = {
    fullTiny:function ($selector) {
        window.tinymce.init({
            plugins: 'link table lists advlist',
            menu: {
                edit: { title: "Редактиране", items: 'undo redo | cut copy paste pastetext | selectall' },
                format: { title: "Форматиране", items: 'bold italic underline strikethrough superscript subscript' },
                insert: { title: "Вмъкни", items: 'link table | specializedcharacter horizontalline' }
            },
            menubar: 'edit format insert',
            selector: $selector,
            branding: false,
            statusbar: false,
            language: 'bg_BG',
            height: '400',
            toolbar:
                ' styleselect ' +
                    '| bold italic backcolor  ' +
                    '| alignleft aligncenter alignright alignjustify ' +
                    '| bullist numlist outdent indent ',
            style_formats: [
                { title: 'Заглавие', block: 'h1' },
                { title: 'Заглавие', block: 'h2' },
                { title: 'Заглавие', block: 'h3' },
                { title: 'Заглавие', block: 'h4' },
                { title: 'Заглавие', block: 'h5' },
                { title: 'Заглавие', block: 'h6' }
            ],
            setup: function (editor) {
                editor.on('change',
                    function () {
                        editor.save();
                    });
            }
        });
    },
    minimalTiny: function ($selector) {
        window.tinymce.init({
            menubar: false,
            selector: $selector,
            branding: false,
            statusbar: false,
            language: 'bg_BG',
            toolbar:
                '| bold italic backcolor  ' +
                '| alignleft aligncenter alignright alignjustify ',
            setup: function (editor) {
                editor.on('change',
                    function () {
                        editor.save();
                    });
            }
        });
    }
};