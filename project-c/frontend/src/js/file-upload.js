import FileUploadWithPreview from "@henkrehorst/file-upload-with-preview";

if (document.getElementById("plantPictureUploadBox") !== null) {
    var upload = new FileUploadWithPreview("plantPictureUpload", {
        showDeleteButtonOnImages: true,
        text: {
            chooseFile: "Upload de foto's van de plant",
            browse:
                "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"20pt\" height=\"20pt\" viewBox=\"0 0 20 20\" version=\"1.1\">" +
                "<g id=\"surface1\">" +
                "<path style=\" stroke:none;fill-rule:nonzero;fill:rgb(0%,0%,0%);fill-opacity:1;\" d=\"M 11.5625 15 L 8.4375 15 C 7.917969 15 7.5 14.582031 7.5 14.0625 L 7.5 7.5 L 4.074219 7.5 C 3.378906 7.5 3.03125 6.660156 3.523438 6.167969 L 9.464844 0.222656 C 9.757812 -0.0703125 10.238281 -0.0703125 10.53125 0.222656 L 16.476562 6.167969 C 16.96875 6.660156 16.621094 7.5 15.925781 7.5 L 12.5 7.5 L 12.5 14.0625 C 12.5 14.582031 12.082031 15 11.5625 15 Z M 20 14.6875 L 20 19.0625 C 20 19.582031 19.582031 20 19.0625 20 L 0.9375 20 C 0.417969 20 0 19.582031 0 19.0625 L 0 14.6875 C 0 14.167969 0.417969 13.75 0.9375 13.75 L 6.25 13.75 L 6.25 14.0625 C 6.25 15.269531 7.230469 16.25 8.4375 16.25 L 11.5625 16.25 C 12.769531 16.25 13.75 15.269531 13.75 14.0625 L 13.75 13.75 L 19.0625 13.75 C 19.582031 13.75 20 14.167969 20 14.6875 Z M 15.15625 18.125 C 15.15625 17.695312 14.804688 17.34375 14.375 17.34375 C 13.945312 17.34375 13.59375 17.695312 13.59375 18.125 C 13.59375 18.554688 13.945312 18.90625 14.375 18.90625 C 14.804688 18.90625 15.15625 18.554688 15.15625 18.125 Z M 17.65625 18.125 C 17.65625 17.695312 17.304688 17.34375 16.875 17.34375 C 16.445312 17.34375 16.09375 17.695312 16.09375 18.125 C 16.09375 18.554688 16.445312 18.90625 16.875 18.90625 C 17.304688 18.90625 17.65625 18.554688 17.65625 18.125 Z M 17.65625 18.125 \"/>" +
                "</g>" +
                "</svg>",
            selectedCount: "foto's zijn geupload",
        },
        sortable: true,
        maxFileCount: 9,
        images: {
            baseImage: "/Images/placeholder-picture.png",
        },
    });
}

if (document.getElementById("plantPictureEditUploadBox") !== null) {
    fetch(window.location.origin + '/api/plants/images/' + (window.location.pathname.split('/')[window.location.pathname.split('/').length - 1]))
        .then(response => {
            response.json().then(data => {
                var upload = new FileUploadWithPreview("plantPictureUpload", {
                    showDeleteButtonOnImages: true,
                    text: {
                        chooseFile: "Upload de foto's van de plant",
                        browse:
                            "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"20pt\" height=\"20pt\" viewBox=\"0 0 20 20\" version=\"1.1\">" +
                            "<g id=\"surface1\">" +
                            "<path style=\" stroke:none;fill-rule:nonzero;fill:rgb(0%,0%,0%);fill-opacity:1;\" d=\"M 11.5625 15 L 8.4375 15 C 7.917969 15 7.5 14.582031 7.5 14.0625 L 7.5 7.5 L 4.074219 7.5 C 3.378906 7.5 3.03125 6.660156 3.523438 6.167969 L 9.464844 0.222656 C 9.757812 -0.0703125 10.238281 -0.0703125 10.53125 0.222656 L 16.476562 6.167969 C 16.96875 6.660156 16.621094 7.5 15.925781 7.5 L 12.5 7.5 L 12.5 14.0625 C 12.5 14.582031 12.082031 15 11.5625 15 Z M 20 14.6875 L 20 19.0625 C 20 19.582031 19.582031 20 19.0625 20 L 0.9375 20 C 0.417969 20 0 19.582031 0 19.0625 L 0 14.6875 C 0 14.167969 0.417969 13.75 0.9375 13.75 L 6.25 13.75 L 6.25 14.0625 C 6.25 15.269531 7.230469 16.25 8.4375 16.25 L 11.5625 16.25 C 12.769531 16.25 13.75 15.269531 13.75 14.0625 L 13.75 13.75 L 19.0625 13.75 C 19.582031 13.75 20 14.167969 20 14.6875 Z M 15.15625 18.125 C 15.15625 17.695312 14.804688 17.34375 14.375 17.34375 C 13.945312 17.34375 13.59375 17.695312 13.59375 18.125 C 13.59375 18.554688 13.945312 18.90625 14.375 18.90625 C 14.804688 18.90625 15.15625 18.554688 15.15625 18.125 Z M 17.65625 18.125 C 17.65625 17.695312 17.304688 17.34375 16.875 17.34375 C 16.445312 17.34375 16.09375 17.695312 16.09375 18.125 C 16.09375 18.554688 16.445312 18.90625 16.875 18.90625 C 17.304688 18.90625 17.65625 18.554688 17.65625 18.125 Z M 17.65625 18.125 \"/>" +
                            "</g>" +
                            "</svg>",
                        selectedCount: "foto's zijn geupload",
                    },
                    maxFileCount: 9,
                    sortable: true,
                    images: {
                        baseImage: "/Images/placeholder-picture.png",
                    },
                    presetFiles: data
                });
            })
        }).catch(e => console.log(e));
}