FormData.prototype.clearEmptyKeys = function() {
    var keysToRemove = [];
    // push empty keys in array for removing
    // value could only be STRING value

    //IE10/11 & Edge Compatible
    var formDataEntries = this.entries(), formDataEntry = formDataEntries.next(), pair;
    while (!formDataEntry.done) {
        pair = formDataEntry.value;
        if (pair[1] === "" || typeof pair[1] === "undefined" || pair[1] === null) {
            keysToRemove.push(pair[0]);
        }
        formDataEntry = formDataEntries.next();
    }

    //For newer browsers supporting ES6
    //for (var pair of this.entries()) {
    //    // remove if empty field
    //    if (pair[1] === "" || typeof pair[1] === "undefined" || pair[1] === null) {
    //        keysToRemove.push(pair[0]);
    //    }
    //}

    //Used to remove fields from formdata,
    //cached in keysToRemove ,cuz the iterator 
    //of entries misses properties when deleted in the loop

    for (var i = 0; i < keysToRemove.length; i++) {
        this.delete(keysToRemove[i]);
    }

    //Alternative for ES6 compatible browsers
    //for (var key of keysToRemove) {
    //    this.delete(key);
    //}
};


FormData.prototype.submit = function(uri, method) {
    var form = document.createElement("form");
    form.action = uri;
    form.method = method;
    form.enctype = 'multipart/form-data';

    this.clearEmptyKeys();


    //IE10/11 & Edge Compatible
    var formDataEntries = this.entries(), formDataEntry = formDataEntries.next(), pair;
    while (!formDataEntry.done) {
        pair = formDataEntry.value;

        var input = document.createElement("input");
        input.type = "text";
        input.name = pair[0];
        input.value = pair[1];
        form.appendChild(input);

        formDataEntry = formDataEntries.next();
    }

    //For newer browsers supporting ES6
    //This commented script should be replicated with the above
    //for (var pair of this.entries()) {
    //    var input = document.createElement("input");

    //    input.type = "text";
    //    input.name = pair[0];
    //    input.value = pair[1];

    //    form.appendChild(input);
    //}

    document.body.appendChild(form);
    form.submit();
};

function clearEmptyProperties(obj) {
    if (typeof obj !== "object") return;
    for (var propName in obj) {
        if (obj.hasOwnProperty(propName)) {
            if (typeof obj[propName] === "object") {
                clearEmptyProperties(obj[propName]);
            }
            if (obj[propName] === null ||
                typeof obj[propName] === "undefined" ||
                obj[propName] === "" ||
                obj[propName] === {}) {
                delete obj[propName];
            }
        }
    }
}

FormData.prototype.printAll = function () {

    // Display the key/value pairs
    var formDataEntries = this.entries(), formDataEntry = formDataEntries.next(), pair;
    while (!formDataEntry.done) {
        pair = formDataEntry.value;
        console.log(pair[0] + '-' + pair[1]);
        formDataEntry = formDataEntries.next();
    }

    //For newer browsers supporting ES6
    // Display the key/value pairs
    //for (var pair of this.entries()) {
    //    console.log(pair[0] + '-' + pair[1]);
    //}
};