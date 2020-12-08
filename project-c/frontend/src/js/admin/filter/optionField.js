let addButtonClicked = false;

window.addField = function addField() {
    if(addButtonClicked === false){
        addButtonClicked = true;
        let fieldTable = document.getElementById("optionForm");
        let fieldCount = fieldTable.rows.length;

        fetch('/Admin/Filter/Field/' + fieldCount).then(function (response) {
            // Get the field successfully
            return response.text();
        }).then(function (field) {
            fieldTable.insertAdjacentHTML('beforeend', field);
            addButtonClicked = false;
        }).catch(function (err) {
            // There was an error
            console.warn('Field not added.', err);
        });
    }
}