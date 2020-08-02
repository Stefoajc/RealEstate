$(document).ready(function () {
    //Change sales/rental prices based on property type
    $('#adv-search-vert-property-status, #adv-search-horizontal-property-status').change(function () {
        let thisElem = $(this);
        if (thisElem.val() === "sale") {
            $('#adv-search-vert-sell-price-ranges, #adv-search-horizontal-sell-price-ranges').show();
            $('#adv-search-vert-rental-price-ranges, #adv-search-horizontal-rental-price-ranges').hide();
        } else {
            $('#adv-search-vert-sell-price-ranges, #adv-search-horizontal-sell-price-ranges').hide();
            $('#adv-search-vert-rental-price-ranges, #adv-search-horizontal-rental-price-ranges').show();
        }
    });

    $('#adv-search-vert-form, #adv-search-horizontal-form').submit(function (event) {
        event.preventDefault ? event.preventDefault() : (event.returnValue = false);
        var formData = new FormData();

        var searchStatus = $('#adv-search-vert-property-status, #adv-search-horizontal-property-status').val(); //rent|sale
        var propType = $('#adv-search-vert-property-types, #adv-search-horizontal-property-types').val();
        var location = $('#adv-search-vert-location, #adv-search-horizontal-location').val();
        var distanceFromLocation = $('#adv-search-vert-distancefromcity, #adv-search-horizontal-distancefromcity').val();
        var areaFrom = $('#adv-search-vert-area-from, #adv-search-horizontal-area-from').val();
        var areaTo = $('#adv-search-vert-area-to, #adv-search-horizontal-area-to').val();
        var priceFrom = searchStatus === "sale"
            ? $('#adv-search-vert-search-sell-minprice, #adv-search-horizontal-search-sell-minprice').val()
            : $('#adv-search-vert-search-rent-minprice, #adv-search-horizontal-search-rent-minprice').val();
        var priceTo = searchStatus === "sale"
            ? $('#adv-search-vert-search-sell-maxprice, #adv-search-horizontal-search-sell-maxprice').val()
            : $('#adv-search-vert-search-rent-maxprice, #adv-search-horizontal-search-rent-maxprice').val();

        if (searchStatus !== "all") {
            formData.append("isForRent", searchStatus === "sale" ? "False" : "True");
        }
        formData.append("propertyType", propType);
        formData.append("cityId", location);
        formData.append("distanceFromCity", distanceFromLocation);
        formData.append("areaFrom", areaFrom);
        formData.append("areaTo", areaTo);
        formData.append("priceFrom", priceFrom);
        formData.append("priceTo", priceTo);
        formData.append("pageCount", 1);
        formData.append("pageSize", 6);
        formData.append("SortBy", "PropertyName");
        formData.append("OrderBy", "Descending");
        formData.append("ViewType", "ListProperties");
        
        formData.submit('/Properties', 'GET');       
    });

    ////Change sales/rental prices based on property type
    //$('#adv-search-horizontal-property-status').change(function () {
    //    let thisElem = $(this);
    //    if (thisElem.val() === "sale") {
    //        $('#adv-search-horizontal-sell-price-ranges').show();
    //        $('#adv-search-horizontal-rental-price-ranges').hide();
    //    } else {
    //        $('#adv-search-horizontal-sell-price-ranges').hide();
    //        $('#adv-search-horizontal-rental-price-ranges').show();
    //    }
    //});



    //$('#adv-search-horizontal-form').submit(function (event) {
    //    event.preventDefault ? event.preventDefault() : (event.returnValue = false);
    //    var formData = new FormData();

    //    var searchStatus = $('#adv-search-horizontal-property-status').val(); //rent|sale
    //    var propType = $('#adv-search-horizontal-property-types').val();
    //    var location = $('#adv-search-horizontal-location').val();
    //    var distanceFromLocation = $('#adv-search-horizontal-distancefromcity').val();
    //    var areaFrom = $('#adv-search-horizontal-area-from').val();
    //    var areaTo = $('#adv-search-horizontal-area-to').val();
    //    var priceFrom = searchStatus === "sale"
    //        ? $('#adv-search-horizontal-search-sell-minprice').val()
    //        : $('#adv-search-horizontal-search-rent-minprice').val();
    //    var priceTo = searchStatus === "sale"
    //        ? $('#adv-search-horizontal-search-sell-maxprice').val()
    //        : $('#adv-search-horizontal-search-rent-maxprice').val();

    //    formData.append("propertyType", propType);
    //    formData.append("cityId", location);
    //    formData.append("distanceFromCity", distanceFromLocation);
    //    formData.append("areaFrom", areaFrom);
    //    formData.append("areaTo", areaTo);
    //    formData.append("priceFrom", priceFrom);
    //    formData.append("priceTo", priceTo);

    //    if (searchStatus === 'sale') {
    //        formData.submit('/Properties/ListProperties', 'GET');
    //    } else {
    //        formData.submit('/Properties/ListRentProperties', 'GET');
    //    }
    //});


});