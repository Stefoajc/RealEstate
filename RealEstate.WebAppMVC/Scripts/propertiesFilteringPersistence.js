function updateDropDownMenues(isForRent, isShortPeriodRent,
    from, to,
    propertyType,
    cityId, distanceFromCity,
    areaFrom, areaTo,
    priceFrom, priceTo,
    extras,
    sortBy, orderBy, pageSize,
    viewType,
    isMap) {

    //DEFAULT parameters IE support
    isForRent = isForRent || '';
    isShortPeriodRent = isShortPeriodRent || '';
    from = from || '';
    to = to || '';
    propertyType = propertyType || '';
    cityId = cityId || '';
    distanceFromCity = distanceFromCity || '';
    areaFrom = areaFrom || '';
    areaTo = areaTo || '';
    priceFrom = priceFrom || '';
    priceTo = priceTo || '';
    extras = extras || [];
    sortBy = sortBy || '';
    orderBy = orderBy || '';
    pageSize = pageSize || '';
    viewType = viewType || '';
    isMap = isMap || false;

    let filterPropertyIsForRentToUrlParam = '';
    if (isForRent.length !== 0) {
        filterPropertyIsForRentToUrlParam = isForRent === ' ' || isForRent === '' ? '' : 'IsForRent=' + isForRent + '&';
    }

    let filterPropertyIsShortPeriodRentToUrlParam = '';
    if (isShortPeriodRent.length !== 0) {
        filterPropertyIsShortPeriodRentToUrlParam =
            isShortPeriodRent === ' ' || isShortPeriodRent === '' ? '' : 'IsShortPeriodRent=' + isShortPeriodRent + '&';
    }

    let filterPropertyFromDateToUrlParam = '';
    if (from.length !== 0) {
        filterPropertyFromDateToUrlParam = from === ' ' ? '' : 'From=' + from + '&';
    }

    let filterPropertyToDateToUrlParam = '';
    if (to.length !== 0) {
        filterPropertyToDateToUrlParam = to === ' ' ? '' : 'To=' + to + '&';
    }

    let filterPropertyAreaFromUrlParam = '';
    if (areaFrom.length !== 0) {
        filterPropertyAreaFromUrlParam = areaFrom === ' ' ? '' : 'AreaFrom=' + areaFrom + '&';
    }

    let filterPropertyAreaToUrlParam = '';
    if (areaTo.length !== 0) {
        filterPropertyAreaToUrlParam = areaTo === ' ' ? '' : 'AreaTo=' + areaTo + '&';
    }

    let filterPropertyPriceFromUrlParam = '';
    if (priceFrom.length !== 0) {
        filterPropertyPriceFromUrlParam = priceFrom === ' ' ? '' : 'PriceFrom=' + priceFrom + '&';
    }

    let filterPropertyPriceToUrlParam = '';
    if (priceTo.length !== 0) {
        filterPropertyPriceToUrlParam = priceTo === ' ' ? '' : 'PriceTo=' + priceTo + '&';
    }

    let filterPropertyTypeUrlParam = '';
    if (propertyType.length !== 0) {
        filterPropertyTypeUrlParam = propertyType === ' ' ? '' : 'PropertyType=' + propertyType + '&';
    }

    let filterCityUrlParam = '';
    if (cityId.length !== 0) {
        filterCityUrlParam = cityId === ' ' ? '' : 'CityId=' + cityId + '&';
    }

    let filterDistanceFromCityUrlParam = '';
    if (distanceFromCity.length !== 0) {
        filterDistanceFromCityUrlParam = distanceFromCity === ' ' ? '' : 'DistanceFromCity=' + distanceFromCity + '&';
    }

    let filterExtrasUrlParam = '';
    if (extras.length !== 0) {
        for (var i = 0; i < extras.length; i++) {
            filterExtrasUrlParam += extras[i] === ' ' ? '' : 'Extras[' + i + ']=' + extras[i] + '&';
        }
    }

    // If there is propertyType then concatenate city filter with ampersand else the filter is only by city
    let filters =
        filterPropertyIsForRentToUrlParam +
        filterPropertyIsShortPeriodRentToUrlParam +
        filterPropertyFromDateToUrlParam +
        filterPropertyToDateToUrlParam +
        filterPropertyAreaFromUrlParam +
        filterPropertyAreaToUrlParam +
        filterPropertyPriceFromUrlParam +
        filterPropertyPriceToUrlParam +
        filterPropertyTypeUrlParam +
        filterCityUrlParam +
        filterDistanceFromCityUrlParam +
        filterExtrasUrlParam;

    //console.log(filters);

    let preSelectOrder = window.location.pathname.replace(/\/$/, '') + "?" + filters;

    if (!isMap) {
        preSelectOrder += "PageCount=1&PageSize=" + pageSize + "&SortBy=" + sortBy + "&OrderBy=" + orderBy + "&ViewType=" + viewType;
    }

    //Fix the Url
    preSelectOrder = preSelectOrder.replace("&&", "&");
    //

    preSelectOrder = preSelectOrder.replace(/&+$/, "");

    //console.log(preSelectOrder);

    $('.filter-status').val(preSelectOrder).trigger("chosen:updated");
}