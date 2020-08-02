ko.observableArray.fn.sortByDate = function (property, direction) {
    return this.sort(function (a, b) {
        var dateA = new Date(a[property]);
        var dateB = new Date(b[property]);
        //default is descending
        if (direction === 'asc') {
            return dateA.getTime() - dateB.getTime();
        }
        return dateB.getTime() - dateA.getTime();
    });
};