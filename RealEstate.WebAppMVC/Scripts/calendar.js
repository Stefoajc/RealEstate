var currentOrgEvents = {};

// изграждане на интервалите в деня за органайзера
function buildDay(element, date, settings) {
  var deferred = $.Deferred();
  if(settings === undefined) {
      settings = new Object();
  }
    var year = 'year-'+date.format('YYYY');
  var dow = parseInt(date.format('E')); // day of week

  function restOfFunction(year){
    if(!settings.showEvents){

      settings.showEvents = [];
      for(let i=0; i<currentOrgEvents[year].length; i++){
        if(currentOrgEvents[year][i].Start.isSame(date, 'day') ||
        (currentOrgEvents[year][i].Start.isBefore(date, 'day') && currentOrgEvents[year][i].End.isSameOrAfter(date, 'day'))){
          settings.showEvents.push(i);
        }
        else if(currentOrgEvents[year][i].Start.isAfter(date, 'day')){
          break;
        }
      }
    }
    // тази част остава заради дневния органаизер
    if(element.hasClass('mCustomScrollbar')){
      element.mCustomScrollbar("destroy");
    }

    if(settings.day){
      // добавяне на текст за днес
      if(date.isSame(moment(), 'day')){
        $('#Organiser .prev-next > .mid > .day .today').show();
        $('#Organiser .org-corner .nav-day').removeClass('nav-visible');
      }else{
        $('#Organiser .prev-next > .mid > .day .today').hide();
        $('#Organiser .org-corner .nav-day').addClass('nav-visible');
      }
      var intDay = parseInt(date.format('D')),
      intMonth = parseInt(date.format('M')),
      year = 'year-'+date.format('YYYY');
      for(let i=0; i<currentHolidays[year].length; i++){
        if(currentHolidays[year][i].Month == intMonth && currentHolidays[year][i].Day == intDay){
          $('#org-day > .holiday').text(currentHolidays[year][i].Name);
          break;
        }
        if(i == currentHolidays[year].length-1){ $('#org-day > .holiday').text(''); }
      }
    }
    else if(settings.week){
      // външен вид на дневните контейнери
      if(date.isSame(moment(), 'day')){
        element.closest('.single').addClass('today selected');
      }
      if(date.day() == 0 || date.day() == 6){
        element.closest('.single').find('.title').addClass('text-Lred');
      }
      element.siblings('.title').text(date.format('ddd DD.MM.YYYYг.'));
    }

    element.html('');
    var singleTemplate = $(
      '<div class="single-interval btn">'+
        '<div class="event-cont"></div>'+
        '<div class="event-start"></div>'+
        '<div class="add-new-btn" title="Добавяне на Задача, Среща, Събитие">+</div>'+
      '</div>'
    );
    var workStart = $(
      '<div class="work-start">'+
        '<div class="text-Lred">неработни часове</div>'+
        '<hr/>'+
        '<div class="text-Lblue">работни часове</div>'+
      '</div>'
    );
    var workEnd = $(
      '<div class="work-start">'+
        '<div class="text-Lblue">работни часове</div>'+
        '<hr/>'+
        '<div class="text-Lred">неработни часове</div>'+
      '</div>'
    );
    var isWorkHours = false;
    var todayWorkHours = etemOrg.workDays[dow].workHours;

    // добавяне на лента за времето
    if(date.isSame(moment(), 'day')){
      element.append('<div class="current-time"></div>');
    }
    for(let i=0;i<1440;i+=etemOrg.orgInt){
      if(todayWorkHours && !isWorkHours && i >= todayWorkHours[0].start && i < todayWorkHours[0].end){
        isWorkHours = true;
        element.append(workStart.clone());
      }else if(todayWorkHours && isWorkHours && i >= todayWorkHours[0].end){
        isWorkHours = false;
        element.append(workEnd.clone());
      }
      var newTimeStamp = singleTemplate.clone();
      newTimeStamp.data('moment', date.clone().startOf('day').add(i, 'minutes'));
      if(settings.day){
        // през attr за да работи css
        newTimeStamp.attr('data-time', convertMinsToHrsMins(i));
        newTimeStamp.addClass(todayWorkHours && i >= todayWorkHours[0].start && i < todayWorkHours[0].end ? 'btn-default work':'btn-Lyellow');
        newTimeStamp.append('<div class="ruler"></div>');
        switch(etemOrg.orgInt){
          case 15:
          newTimeStamp.find('.ruler').append(
            '<div class="ruler-elem ruler-4 ruler-large"></div>'+
            '<div class="ruler-elem ruler-4 ruler-small"></div>'+
            '<div class="ruler-elem ruler-4 ruler-small"></div>'+
            '<div class="ruler-elem ruler-4 ruler-small"></div>'
          );
          break;
          case 30:
          newTimeStamp.find('.ruler').append(
            '<div class="ruler-elem ruler-6 ruler-large"></div>'+
            '<div class="ruler-elem ruler-6 ruler-small"></div>'+
            '<div class="ruler-elem ruler-6 ruler-small"></div>'+
            '<div class="ruler-elem ruler-6 ruler-med"></div>'+
            '<div class="ruler-elem ruler-6 ruler-small"></div>'+
            '<div class="ruler-elem ruler-6 ruler-small"></div>'
          );
          break;
          case 60:
          newTimeStamp.find('.ruler').append(
            '<div class="ruler-elem ruler-4 ruler-large"></div>'+
            '<div class="ruler-elem ruler-4 ruler-small"></div>'+
            '<div class="ruler-elem ruler-4 ruler-med"></div>'+
            '<div class="ruler-elem ruler-4 ruler-small"></div>'
          );
          break;
        }
      }
      else if(settings.week){
        newTimeStamp.append('<div class="time-stamp">'+convertMinsToHrsMins(i)+'</div>');
        newTimeStamp.addClass(
          (date.day() == 0 || date.day() == 6 ?'btn-Lred':(todayWorkHours && i >= todayWorkHours[0].start && i < todayWorkHours[0].end? 'btn-default': 'btn-Lyellow'))+
          (todayWorkHours && i >= todayWorkHours[0].start && i < todayWorkHours[0].end ? ' work' : '')
        );
      }
      for(let j=settings.showEvents[0], counter=0; j<=settings.showEvents[settings.showEvents.length-1]; ){
        let eventStart = currentOrgEvents[year][j].Start;
        let eventEnd = currentOrgEvents[year][j].End;
        let eventEndSys = eventEnd.clone().subtract(1, 'minute');
        let intEnd = newTimeStamp.data('moment').clone().add(etemOrg.orgInt,'minutes');
        let eventBegins = (eventStart >= newTimeStamp.data('moment') && eventStart < intEnd);
        let eventContinues = eventStart < newTimeStamp.data('moment') && eventEndSys >= intEnd;
        let eventEnds = (eventEndSys >= newTimeStamp.data('moment') && eventEndSys < intEnd);

        if(eventBegins || eventContinues || eventEnds){
          let thisEvent = $('<div class="event event-'+currentOrgEvents[year][j].Id+'"></div>');
          if(eventBegins){
            newTimeStamp.find('.event-start').append(thisEvent);
            newTimeStamp.find('.event-start').css('display', 'block');
            newTimeStamp.find('.event-start > .event').css(
              'width',
              'calc(100% / '+newTimeStamp.find('.event-start > .event').length+' - 5px)'
            );
            thisEvent.text(currentOrgEvents[year][j].Title);
            if(settings.day){
              thisEvent.append(
                '<div class="btn-delete"><span class="glyphicon glyphicon-trash"></span></div>'+
                '<div class="btn-edit"><span class="glyphicon glyphicon-edit"></span></div>'
              );
            }
          }else{
            newTimeStamp.find('.event-cont').append(thisEvent);
            newTimeStamp.find('.event-start').css(
              'padding-left',
              newTimeStamp.find('.event-cont').outerWidth()+'px'
            );
          }
          buildEventTooltip(thisEvent, currentOrgEvents[year][j], date);
          thisEvent.data('id', currentOrgEvents[year][j].Id);
          if(eventEnds){
            settings.showEvents.splice(counter, 1);
            counter--;
          }
        }
        counter++;
        j = settings.showEvents[counter];
      }
      element.append(newTimeStamp);
    }
    if(settings.day){
      element.mCustomScrollbar({
        theme: 'light-3',
        scrollInertia: 350,
        mouseWheel: { scrollAmount: 100 }
      });
    }
    else if(settings.week){
      element.mCustomScrollbar({
        scrollInertia: 350,
        mouseWheel: { scrollAmount: 100 }
      });
    }
    deferred.resolve(settings.showEvents);
  }

  if(!currentOrgEvents[year] && settings.day){
    $('#VirtualBureau .option-container').append('<div class="focused-popup">'+loadingDiv+'</div>');
    $.when(
      $.getJSON('/Organisser/OfficialHolidays?year='+date.format('YYYY')),
      $.getJSON('/Organiser/GetAllOrganiserItems?year='+date.format('YYYY'))
    ).done(function(data1, data2){
      addYearToCache(year, data1[0], data2[0]);
      restOfFunction(year);
      $('#VirtualBureau .option-container .focused-popup').remove();
    }).fail(function(jqXHR){
      const target = $('#VirtualBureau .option-container .focused-popup').html('<div class="text-danger text-center"></div>');
      showErrorResponse(jqXHR, target);
    });
  }else{
    restOfFunction(year);
  }
  return deferred.promise();
}

// изграждане на дните в седмицата
function buildWeek(element, date, settings){
  var deferred = $.Deferred();
  if(settings === undefined){
    var settings = new Object();
  }
  var dateIntervalEnd = date.clone().add(6, 'days');
  var month1 = parseInt(date.format('M'));
  var month2 = parseInt(dateIntervalEnd.format('M'));
  var year1 = 'year-'+date.format('YYYY');
  var year2 = 'year-'+dateIntervalEnd.format('YYYY');
  var year1events = [];
  var year2events = [];
  var includeYear2 = (year1 != year2);
  var includeMonth2 = (month1 != month2);

  function restOfFunction(){
    for(let i=0; i<currentOrgEvents[year1].length; i++){
      if(currentOrgEvents[year1][i].Start.isAfter(dateIntervalEnd, 'day')){
        break;
      }
      else if(
        currentOrgEvents[year1][i].Start.isSameOrAfter(date, 'day') ||
        (currentOrgEvents[year1][i].Start.isBefore(date, 'day') && currentOrgEvents[year1][i].End.isSameOrAfter(date, 'day'))
      ){
        year1events.push(i);
      }
    }
    if(includeYear2){
      for(let i=0; i<currentOrgEvents[year2].length; i++){
        if(currentOrgEvents[year2][i].Start.isAfter(dateIntervalEnd, 'day')){
          break;
        }
        year2events.push(i);
      }
    }
    if(element.find('.mCustomScrollbar').length > 0){
      element.find('.mCustomScrollbar').mCustomScrollbar("destroy");
    }
    element.html('');
    if(date.isSame(moment(), 'week')){
      $('#Organiser .org-corner .nav-week').removeClass('nav-visible');
    }else{
      $('#Organiser .org-corner .nav-week').addClass('nav-visible');
    }
    $('#Organiser .work-toggle.work-hide').removeClass('work-visible')
    .siblings('.work-toggle').addClass('work-visible');
    for(let i=0;i<7;i++){
      let thisDay = parseInt(date.clone().add(i, 'days').format('E'));
      let newDay = $(
        '<div class="single '+(etemOrg.workDays[thisDay].workHours?'work':'')+'">'+
          '<div class="single-container">'+
            '<div class="title"></div>'+
            '<div class="content"></div>'+
          '</div>'+
        '</div>'
      );
      element.append(newDay);
    }
    element.find('.single .content').each(function(){
      $(this).closest('.single').attr({
        'data-month': date.format('M'),
        'data-day': date.format('D')
      });
      if(year1 == 'year-'+date.format('YYYY')){
        var settings = {
          week: true,
          showEvents: year1events
        };
        buildDay($(this), date, settings).done(function(data){
          year1events = data;
        });
      }else{
        var settings = {
          week: true,
          showEvents: year2events
        };
        buildDay($(this), date, settings).done(function(data){
          year2events = data;
        });
      }
      date.add(1, 'day');
    });
    element.children('.single.work').css('width', 'calc(100% / '+$('#org-week > .single.work').length+')');
    function weekHolidays(year, month1){
      for(let i=0; i<currentHolidays[year].length; i++){
        if(currentHolidays[year][i].Month != month1){ continue; }
        let redStar = $('<span class="glyphicon glyphicon-star" data-toggle="tooltip"></span>');
        $('#org-week > .single[data-month="'+currentHolidays[year][i].Month+'"][data-day="'+currentHolidays[year][i].Day+'"]').append(redStar);
        redStar.tooltip({
          placement: 'auto right',
          container: '#Organiser',
          title: currentHolidays[year][i].Name
        });
      }
    }
    weekHolidays(year1, month1);
    if(includeYear2) weekHolidays(year2, month2);
    else if(includeMonth2) weekHolidays(year1, month2);
  }
  if(!currentOrgEvents[year1]){
    $('#VirtualBureau .option-container').append('<div class="focused-popup">'+loadingDiv+'</div>');
    $.when(
      $.getJSON('/Organiser/OfficialHolidays?year='+date.format('YYYY')),
      $.getJSON('/Organiser/GetAllOrganiserItems?year='+date.format('YYYY'))
    ).done(function(data1, data2){
      addYearToCache(year1, data1[0], data2[0]);
      restOfFunction();
      $('#VirtualBureau .option-container .focused-popup').remove();
      deferred.resolve();
    }).fail(function(jqXHR){
      const target = $('#VirtualBureau .option-container .focused-popup').html('<div class="text-danger text-center"></div>');
      showErrorResponse(jqXHR, target);
    });
  }
  else if(!currentOrgEvents[year2]){
    $('#VirtualBureau .option-container').append('<div class="focused-popup">'+loadingDiv+'</div>');
    $.when(
      $.getJSON('/Organiser/OfficialHolidays?year='+date.format('YYYY')),
      $.getJSON('/Organiser/GetAllOrganiserItems?year='+date.format('YYYY'))
    ).done(function(data1, data2){
      addYearToCache(year2, data1[0], data2[0]);
      restOfFunction();
      $('#VirtualBureau .option-container .focused-popup').remove();
      deferred.resolve();
    }).fail(function(jqXHR){
      const target = $('#VirtualBureau .option-container .focused-popup').html('<div class="text-danger text-center"></div>');
      showErrorResponse(jqXHR, target);
    });
  }
  else{
    restOfFunction();
    deferred.resolve();
  }
  return deferred.promise();
}

// изграждане на дните в месеца
function buildMonth(element, date, settings){
  var deferred = $.Deferred();
  if(settings === undefined){
    var settings = new Object();
  }
  var year = 'year-'+date.format('YYYY');
  var month = parseInt(date.format('M'))

  function restOfFunction(year){
    intervalStart = date.clone().startOf('month');
    intervalStart = intervalStart.day() >= etemOrg.fdOfWeek
    ? intervalStart.day(etemOrg.fdOfWeek)
    : intervalStart.day(etemOrg.fdOfWeek-7);
    var count = 0;
    var isThisMonth = false;

    if(settings.month){
      element.html('');
      if(date.isSame(moment(), 'month')){
        $('#Organiser .org-corner .nav-month').removeClass('nav-visible');
      }else{
        $('#Organiser .org-corner .nav-month').addClass('nav-visible');
      }
    }
    else if(settings.year){
      // за да сме сигурни че месецът отговаря взимаме последния ден от седмицата
      var thisMonth = intervalStart.clone().add(6, 'days');
      element.html('<div class="title">'+thisMonth.format('MMMM')+'</div>');
      element.find('.title').data('month', thisMonth.clone());
      if(thisMonth.isSame(moment(), 'month')){
        element.addClass('today');
      }
    }
    for(var i = intervalStart.clone(); count < 42; i.add(1, 'day')){
      if(!isThisMonth && i.format('D') == '1'){
        isThisMonth = true;
      }else if(i.format('D') == '1'){
        isThisMonth = false;
      }
      var newDate = $('<div class="single '+(isThisMonth?'this-month':'')+'"></div>');
      newDate.data('day', i.clone());
      if(i.isSame(moment(), 'day') && isThisMonth){
        newDate.addClass('today selected');
      }
      if(settings.month){
        newDate.append(
          '<div class="title">'+i.format('D')+'</div>'+
          '<div class="inner"></div>'+
          '<div class="add-new-btn" title="Добавяне на Задача, Среща, Събитие">+</div>'
        );

        newDate.addClass('btn btn-xs btn-'+(i.day()==6 || i.day()==0 ? 'Lred':'default'));
      }
      else if(settings.year){
        newDate.append('<span>'+i.format('D')+'</span>');
        newDate.addClass((i.day()==6 || i.day()==0 ? 'text-Lred':''));
      }
      element.append(newDate);
      count++;
    }
    if(settings.month){
      for(let i=0; i<currentHolidays[year].length; i++){
        if(currentHolidays[year][i].Month != month){ continue; }
        let redStar = $('<span class="pull-left glyphicon glyphicon-star" data-toggle="tooltip"></span>');
        $('#org-month > .single.this-month:eq('+(currentHolidays[year][i].Day-1)+') .title').prepend(redStar);
        redStar.tooltip({
          placement: 'auto right',
          container: '#Organiser',
          title: currentHolidays[year][i].Name
        });
      }
      for(let i=0; i<currentOrgEvents[year].length; i++){
        let eventStart = currentOrgEvents[year][i].Start;
        if(eventStart.isSame(date, 'month')){

          let eventEnd = currentOrgEvents[year][i].End;
          let eventEndSys;
          if(eventEnd.isAfter(date, 'month')){ eventEndSys = date.clone().endOf('month'); }
          else{ eventEndSys = eventEnd.clone().subtract(1, 'minute'); }

          let eventDayStart = parseInt(eventStart.format('D'));
          let eventDayEnd = parseInt(eventEndSys.format('D'));
          for(let j=eventDayStart; j<=eventDayEnd; j++){
            let thisContainer = $('#org-month > .single.this-month:eq('+(j-1)+') > .inner').last();
            let newEvent = $('<div class="single-event"><span class="interval">'+currentOrgEvents[year][i].Start.format('HH:mm')+' - '+currentOrgEvents[year][i].End.format('HH:mm')+'</span> '+currentOrgEvents[year][i].Title+'</div>');
            thisContainer.append(newEvent);
            buildEventTooltip(newEvent, currentOrgEvents[year][i]);

            let vBureau = $('#VirtualBureau'), vBureauHidden = true,
            orgTab = $('#Organiser'), orgTabHidden = true,
            oMonth = $('#org-month'), oMonthHidden = true;
            if(vBureau.is(':visible')){ vBureauHidden = false; }
            else{ vBureau.css({'opacity': '0', 'display': 'block'}); }
            if(orgTab.is(':visible')){ orgTabHidden = false; }
            else{ orgTab.css({'opacity': '0', 'display': 'block'}); }
            if(oMonth.is(':visible')){ oMonthHidden = false; }
            else{ oMonth.css({'opacity': '0', 'display': 'block'}); }

            if(thisContainer.prop('clientHeight') < thisContainer.prop('scrollHeight') || newEvent.prev().hasClass('line')){
              thisContainer.find('.single-event').addClass('line');
            }
            if(thisContainer.prop('clientHeight') < thisContainer.prop('scrollHeight')){
              thisContainer.after(thisContainer.clone());
              thisContainer = thisContainer.next();
              let allInners = thisContainer.add(thisContainer.siblings('.inner'));
              allInners.css('width', 'calc(100% / '+allInners.length+')');
              thisContainer.html('');
              newEvent.appendTo(thisContainer);
            }
            if(vBureauHidden){ vBureau.css({'opacity': '1', 'display': 'none'}); }
            if(orgTabHidden){ orgTab.css({'opacity': '1', 'display': 'none'}); }
            if(oMonthHidden){ oMonth.css({'opacity': '1', 'display': 'none'}); }
          }
        }
      }
    }
  }

  if(!currentOrgEvents[year]){
    $('#VirtualBureau .option-container').append('<div class="focused-popup">'+loadingDiv+'</div>');
    $.when(
      $.getJSON('/Organiser/OfficialHolidays?year='+date.format('YYYY')),
      $.getJSON('/Organiser/GetAllOrganiserItems?year='+date.format('YYYY'))
    ).done(function(data1, data2){
      addYearToCache(year, data1[0], data2[0]);
      restOfFunction(year);
      $('#VirtualBureau .option-container .focused-popup').remove();
      deferred.resolve();
    }).fail(function(jqXHR){
      const target = $('#VirtualBureau .option-container .focused-popup').html('<div class="text-danger text-center"></div>');
      showErrorResponse(jqXHR, target);
    });
  }else{
    restOfFunction(year);
    deferred.resolve();
  }
  return deferred.promise();
}

// изграждане на месеците в годината
function buildYear(element, date, settings){
  var deferred = $.Deferred();
  if(settings === undefined){
    var settings = new Object();
  }
  var year = 'year-'+date.format('YYYY');

  function restOfFunction(year){
    element.html('');
    if(date.isSame(moment(), 'year')){
      $('#Organiser .org-corner .nav-year').removeClass('nav-visible');
    }else{
      $('#Organiser .org-corner .nav-year').addClass('nav-visible');
    }
    for(var i=0;i<12;i++){
      element.append('<div class="single"></div>');
    }
    element.children('.single').each(function(){
      buildMonth($(this), date, {year:true});
      date.add(1, 'month');
    });
    for(let i=0; i<currentHolidays[year].length; i++){
      $('#org-year > .single:eq('+(currentHolidays[year][i].Month-1)+') > .single.this-month:eq('+(currentHolidays[year][i].Day-1)+')').addClass('text-Lred');
    }
    for(let i=0; i<currentOrgEvents[year].length; i++){
      let eventStart = currentOrgEvents[year][i].Start,
      eventEndSys = currentOrgEvents[year][i].End.clone().subtract(1, 'minute');
      let startMonth = parseInt(eventStart.format('M')),
      startDay = parseInt(eventStart.format('D')),
      endMonth = parseInt(eventEndSys.format('M')),
      endDay = parseInt(eventEndSys.format('D'));
      for(let j=startMonth; j<=endMonth; j++){
        let endDayTemp = endDay;
        if(j<endMonth){ endDayTemp = 31; }
        for(let k=startDay; k<=endDayTemp; k++){
          $('#org-year > .single:eq('+(j-1)+') > .single.this-month:eq('+(k-1)+')').addClass('event-day');
        }
        startDay = 1;
      }
    }
  }

  if(!currentOrgEvents[year]){
    $('#VirtualBureau .option-container').append('<div class="focused-popup">'+loadingDiv+'</div>');
    $.when(
      $.getJSON('/Organiser/OfficialHolidays?year='+date.format('YYYY')),
      $.getJSON('/Organiser/GetAllOrganiserItems?year='+date.format('YYYY'))
    ).done(function(data1, data2){
      addYearToCache(year, data1[0], data2[0]);
      restOfFunction(year);
      $('#VirtualBureau .option-container .focused-popup').remove();
      deferred.resolve();
    }).fail(function(jqXHR){
      const target = $('#VirtualBureau .option-container .focused-popup').html('<div class="text-danger text-center"></div>');
      showErrorResponse(jqXHR, target);
    });
  }else{
    restOfFunction(year);
    deferred.resolve();
  }
  return deferred.promise();
}

// минаване на предишен/следващ интервал от време в органайзера
function orgPrevNext(date, direction, interval){
  var returnText;
  date.add((direction == 'next' ? 1 : -1), interval);
  if(interval == 'day'){
    buildDay($('#org-day > .single-container'), date, {day:true}).done(function(){
      updateSchedule(date.clone().startOf('day'), date.clone().endOf('day'));
      updateWorkScrolls($('#org-day .mCustomScrollbar'));
      returnText = date.format('D MMM YYYYг.');
      $('#Organiser .prev-next > .mid > .'+interval+' .date').text(returnText);
    });
  }
  else if(interval == 'week'){
    var thisDay = date.clone();
    buildWeek($('#org-week'), thisDay).done(function(){
      let showInt = $('#org-week > .single.today.selected .single-interval:eq(0)');
      if(showInt.length == 0){
        $('#Organiser .schedule > .schedule-title > .interval').html('');
        $('#Organiser .schedule .schedule-content .mCSB_container > *').hide();
        $('#Organiser .schedule .schedule-content .no-select.week').show();
      }else{
        showInt = showInt.data('moment').clone();
        updateSchedule(showInt, showInt.clone().endOf('day'));
      }
      updateWorkScrolls($('#org-week .mCustomScrollbar'));
      returnText = date.format('D MMM YYYYг.')+' - '+date.clone().add(6, 'days').format('D MMM YYYYг.');
      $('#Organiser .prev-next > .mid > .'+interval+' .date').text(returnText);
    });
  }
  else if(interval == 'month'){
    buildMonth($('#org-month'), date, {month:true}).done(function(){
      let showInt = $('#org-month > .single.selected');
      if(showInt.length == 0){
        $('#Organiser .schedule > .schedule-title > .interval').html('');
        $('#Organiser .schedule .schedule-content .mCSB_container > *').hide();
        $('#Organiser .schedule .schedule-content .no-select.month').show();
      }else{
        showInt = showInt.data('day').clone();
        updateSchedule(showInt, showInt.clone().endOf('day'));
      }
      returnText = date.format('MMMM YYYYг.');
      $('#Organiser .prev-next > .mid > .'+interval+' .date').text(returnText);
    });
  }
  else if(interval == 'year'){
    var thisMonth = date.clone();
    buildYear($('#org-year'), thisMonth).done(function(){
      let showInt = $('#org-year > .single > .single.today.selected');
      if(showInt.length == 0){
        $('#Organiser .schedule > .schedule-title > .interval').html('');
        $('#Organiser .schedule .schedule-content .mCSB_container > *').hide();
        $('#Organiser .schedule .schedule-content .no-select.year').show();
      }else{
        showInt = showInt.data('day').clone();
        updateSchedule(showInt, showInt.clone().endOf('day'));
      }
      returnText = date.format('YYYYг.');
      $('#Organiser .prev-next > .mid > .'+interval+' .date').text(returnText);
    });
  }
}

// обновяване на скроловете с addon
function updateWorkScrolls(element){
  element.each(function(){
    // променливата трябва после за timeoutа
    var thisElem = $(this);
    var thisScrollArea = thisElem.closest('.mCustomScrollbar');
    var destination = thisScrollArea.find('.work').eq(0);
    if(destination.length === 0) destination = thisScrollArea.find('.single-interval').eq(0);
    if(thisElem.find('.current-time').length > 0){
      thisElem.find('.current-time').each(function(){
        var thisElem2 = $(this);
        var tempDest = 0;
        var offsetCoords = 9999;
        var lastWorkField = thisScrollArea.find('.work').last();
        var lastWorkElem = lastWorkField.length > 0 ? lastWorkField.position().top + lastWorkField.height() : thisScrollArea.height();
        var lastWorkDest = lastWorkElem - thisScrollArea.height();

        for(var i=0; i<thisElem2.siblings('.single-interval').length; i++){
          var thisSibling = thisElem2.siblings('.single-interval').eq(i);
          if(
            thisSibling.data('moment') <= moment() &&
            thisSibling.data('moment').clone().add(etemOrg.orgInt, 'minutes') > moment()
          ){
            var offsetMins = moment().diff(thisSibling.data('moment'), 'minutes');
            offsetCoords = thisSibling.position().top+(thisSibling.outerHeight()/etemOrg.orgInt*offsetMins);
            thisElem2.css('top', offsetCoords+'px');
            tempDest = offsetCoords - (thisScrollArea.height()/2);
            break;
          }
        }
        if(offsetCoords < destination.position().top){
          destination = offsetCoords;
        }else if(tempDest > destination.position().top && tempDest <= lastWorkDest){
          destination = tempDest;
        }else if(offsetCoords >= lastWorkElem){
          destination = tempDest;
        }else if(tempDest > lastWorkDest){
          destination = lastWorkDest;
        }
      });
      let toNextMin = 60000-(moment().get('second')*1000 + moment().get('millisecond'));
      setTimeout(function(){
        updateWorkScrolls(thisElem);
      }, toNextMin);
    }
    if(thisElem.find('.single-interval.selected').length > 0){
      return true;
    }
    thisElem.mCustomScrollbar(
      'scrollTo',
      destination,
      {scrollInertia: 1000}
    );
  });
}

// генерира системни напомняния със формата за събитие
// не генерира напомняния за миналото
function buildNotificationsLocal(thisForm){
  var startDate = thisForm.find('#event-start').data("DateTimePicker").date();
  thisForm.find('input[name^="NotificationsLocal"]').remove();
  thisForm.find('.before-container').each(function(index){
    if(index > 10){ return false; }
    var thisDate = startDate.clone().subtract(
      $(this).find('.before-amount').val(),
      'milliseconds'
    );
    if(thisDate <= moment()){ return true; }
    thisForm.append('<input type="hidden" name="NotificationsLocal['+index+']" value="'+thisDate.format('YYYY-MM-DD HH:mm')+'" />');
  });
}

// добавя събитията от годината в кеша и ги конвертира
function addYearToCache(year, dataHol, dataOrg){
  // сортиране по начална дата
  dataOrg.sort(function(a,b){
    if(a.Start < b.Start) return -1;
    if(a.Start > b.Start) return 1;
    return 0;
  });
  var lastElem;
  for(let i=0; i<dataOrg.length; i++){
    dataOrg[i].Start = parseMoment(dataOrg[i].Start);
    dataOrg[i].End = parseMoment(dataOrg[i].End);
    if(!lastElem){
      $('#Organiser .schedule .schedule-content .mCSB_container > .single-event').each(function(){
        if(dataOrg[i].Start.isBefore($(this).data('start'))){
          lastElem = addEventToSchedule(dataOrg[i], $(this).prev(), true);
          return false;
        }
        else if(dataOrg[i].Start.isAfter($(this).data('start'))){
          lastElem = addEventToSchedule(dataOrg[i], $(this), true);
          return false;
        }
      });
      if(!lastElem){ lastElem = addEventToSchedule(dataOrg[i], $('#Organiser .schedule .schedule-content .mCSB_container > div:last-child'), true); }
    }else{
      lastElem = addEventToSchedule(dataOrg[i], lastElem, true);
    }
  }
  currentHolidays[year] = dataHol;
  currentOrgEvents[year] = dataOrg;
  orderKeys(currentHolidays);
  orderKeys(currentOrgEvents);
}

// подрежда ключовете в обекта по азбучен ред
function orderKeys(obj) {
    var keys = Object.keys(obj).sort(function keyOrder(k1, k2) {
        if (k1 < k2) return -1;
        else if (k1 > k2) return +1;
        else return 0;
    });

    var i, after = {};
    for (i = 0; i < keys.length; i++) {
        after[keys[i]] = obj[keys[i]];
        delete obj[keys[i]];
    }

    for (i = 0; i < keys.length; i++) {
        obj[keys[i]] = after[keys[i]];
    }
    return obj;
}

function addEventToSchedule(dataOrgEvent, afterTarget, returnElem){
  let dataType = [];
  switch(dataOrgEvent.Type){
    case 'Job':
      dataType = ['Задача', 'Планирана', 'Продължаваща'];
      break;
    case 'Meeting':
      dataType = ['Среща', 'Планирана', 'Продължаваща'];
      break;
    case 'Event':
      dataType = ['Събитие', 'Планиранo', 'Продължаващo'];
  }
  let thisEvent = $(
  '<div class="single-event btn btn-default arrow">'+
    '<div class="event-menu collapse text-right">'+
      '<span class="repeat'+(dataOrgEvent.IsRepeatable?' show-inline':'')+'">Повтарящо се</span>'+
      '<span class="glyphicon glyphicon-edit"></span>'+
      '<span class="glyphicon glyphicon-trash"></span>'+
    '</div>'+
    '<div class="collapse in">'+
      '<div class="col col-xs-3">'+
        '<div class="time">'+dataOrgEvent.Start.format('HH:mm')+'ч.</div>'+
        '<div class="time cont">'+dataOrgEvent.Start.format('DD.MM.YYYY')+'г.</div>'+
      '</div>'+
      '<div class="col col-xs-6">'+
        '<div class="title repeat'+(dataOrgEvent.IsRepeatable?' show':'')+'"><span>Повтарящо се</span></div>'+
        '<div class="title"><span>'+dataOrgEvent.Title+'</span></div>'+
        '<div class="title cont">'+dataType[2]+' '+dataType[0]+'</div>'+
      '</div>'+
      '<div class="col col-xs-3">'+
        '<div class="time">'+dataOrgEvent.End.format('HH:mm')+'ч.</div>'+
        '<div class="time cont">'+dataOrgEvent.End.format('DD.MM.YYYY')+'г.</div>'+
      '</div>'+
    '</div>'+
    '<div class="collapse">'+
      '<div class="top-info">'+
        '<div>'+dataType[1]+' '+dataType[0]+'</div>'+
        '<div class="row">'+
          '<div class="col col-xs-6">за '+dataOrgEvent.Start.format('HH:mm')+'ч.</div>'+
          '<div class="col col-xs-6">до '+dataOrgEvent.End.format('HH:mm')+'ч.</div>'+
          '<div class="col col-xs-12 nocont">на '+dataOrgEvent.Start.format('DD MMMM YYYY')+'г.</div>'+
          '<div class="col col-xs-12 cont">за периода<br/>'+dataOrgEvent.Start.format('DD MMMM YYYY')+'г. - '+dataOrgEvent.End.format('DD MMMM YYYY')+'г.</div>'+
        '</div>'+
      '</div>'+
      '<div class="text-left">'+
        '<div><b>Заглавие:</b> '+dataOrgEvent.Title+'</div>'+
        '<div class="description"><b>Описание:</b> '+dataOrgEvent.Body+'</div>'+
      '</div>'+
    '</div>'+
  '</div>'
  );
  thisEvent.data('start', dataOrgEvent.Start);
  thisEvent.data('end', dataOrgEvent.End);
  thisEvent.data('id', dataOrgEvent.Id);
  if(dataOrgEvent.LinkToItem){
    let linkPath = dataOrgEvent.LinkToItem.slice(0, -1).split(';');
    for(let i=0; i<linkPath.length; i++){
      linkPath[i] = linkPath[i].split(':');
    }
    thisEvent.data('link', linkPath);
    thisEvent.find('.description').after('<span class="event-link-shortcut" data-toggle="tooltip" title="">към връзката</span>');
  }
  if(!dataOrgEvent.Start.isSame(dataOrgEvent.End.clone().subtract(1, 'minute'), 'day')){ thisEvent.addClass('cont'); }
  afterTarget.after(thisEvent);
  let eventTitle = thisEvent.find('.title:not(.repeat):not(.cont)');

  let vBureau = $('#VirtualBureau'), vBureauHidden = true,
  orgTab = $('#Organiser'), orgTabHidden = true;
  if(vBureau.is(':visible')){ vBureauHidden = false; }
  else{ vBureau.css({'opacity': '0', 'display': 'block'}); }
  if(orgTab.is(':visible')){ orgTabHidden = false; }
  else{ orgTab.css({'opacity': '0', 'display': 'block'}); }

  if(eventTitle.prop('clientHeight') < eventTitle.prop('scrollHeight')){
    eventTitle.addClass('smallfont');
  }
  thisEvent.hide();
  if(vBureauHidden){ vBureau.css({'opacity': '', 'display': ''}); }
  if(orgTabHidden){ orgTab.css({'opacity': '', 'display': ''}); }
  if(returnElem) return thisEvent;
}

function buildEventTooltip(element, eventData){
  const eventType = (function(){
    if(eventData.End.diff(eventData.Start, 'minutes') > etemOrg.orgInt){
      if(eventData.Type == 'Job') return 'Продължаваща задача';
      if(eventData.Type == 'Meeting') return 'Продължаваща среща';
      if(eventData.Type == 'Event') return 'Продължаващо събитие';
    }else{
      if(eventData.Type == 'Job') return 'Задача';
      if(eventData.Type == 'Meeting') return 'Среща';
      if(eventData.Type == 'Event') return 'Събитие';
    }
    return 'Събитие';
  })();
  element.attr('data-toggle', 'tooltip');
  element.tooltip({
    placement: 'auto right',
    delay: {show: 500, hide: 0},
    html: true,
    container: '#Organiser',
    template:
    '<div class="tooltip event-tooltip" role="tooltip">'+
      '<div class="tooltip-arrow"></div>'+
      '<div class="tooltip-inner"></div>'+
    '</div>',
    title:
    '<div class="event-tooltip-title row">'+
      '<div class="col col-xs-12">'+eventType+'</div>'+
      '<div class="col col-xs-3">Относно:</div><div class="col col-xs-9">'+eventData.Title+'</div>'+
    '</div>'+
    '<div class="event-tooltip-body row">'+
      '<div class="col col-xs-3">Начало:</div><div class="col col-xs-9">'+eventData.Start.format(dateDisplayFormat)+'</div>'+
      '<div class="col col-xs-3">Край:</div><div class="col col-xs-9">'+eventData.End.format(dateDisplayFormat)+'</div>'+
    '</div>'
  });
}

// обръща минути в часове и минути
function convertMinsToHrsMins(mins) {
  let h = Math.floor(mins / 60);
  let m = mins % 60;
  h = h < 10 ? '0' + h : h;
  m = m < 10 ? '0' + m : m;
  return `${h}:${m}`;
}

function updateSchedule(intervalStart, intervalEnd){
  if(!intervalStart || !intervalEnd){
    intervalStart = $('#Organiser .schedule').data('start');
    intervalEnd = $('#Organiser .schedule').data('end');
  }else{
    $('#Organiser .schedule').data('start', intervalStart);
    $('#Organiser .schedule').data('end', intervalEnd);
  }
  if(intervalEnd.isSame(intervalStart, 'day')){
    $('#Organiser .schedule > .schedule-title > .interval').html(intervalStart.format('D MMM YYYYг.'));
    if(intervalStart.isSame(moment(), 'day')){ $('#Organiser .schedule > .schedule-title > .interval').prepend('<span class="today">Днес</span> '); }
  }
  else if(intervalEnd.isSame(intervalStart, 'month')){
    $('#Organiser .schedule > .schedule-title > .interval').html(intervalStart.format('MMMM YYYYг.'));
  }
  var eventsList = $('#Organiser .schedule .schedule-content .single-event'),
  shownEvents = [];
  $('#Organiser .schedule .schedule-content .no-select').hide();
  eventsList.hide();
  for(let i=0; i<eventsList.length; i++){
    if(eventsList.eq(i).data('start').isAfter(intervalEnd)){
      break;
    }
    // else if(shownEvents.indexOf(eventsList.eq(i).data('id')) != -1){
    //   eventsList.eq(shownEvents.indexOf(eventsList.eq(i).data('id'))).addClass('repeat');
    // }
    else if(
      eventsList.eq(i).data('start').isSameOrAfter(intervalStart) ||
      (eventsList.eq(i).data('start').isBefore(intervalStart) && eventsList.eq(i).data('end').isAfter(intervalStart))
    ){
      eventsList.eq(i).show();
      shownEvents[i] = eventsList.eq(i).data('id');
    }
  }
  if(!shownEvents[shownEvents.length-1]){
    $('#Organiser .schedule .schedule-content .no-select.results').show();
  }
}

function parseMoment(string){
  return moment(parseInt(string.match(/\d/g).join("")));
}

function enableEventFormDateFields(){
  var eventStartVal = moment($('#event-start input[type="text"]').val(), dateDisplayFormat);
  var eventEndVal = moment($('#event-end input[type="text"]').val(), dateDisplayFormat);
  $('#event-start').datetimepicker({
    locale: 'bg',
    format: dateDisplayFormat,
    defaultDate: eventStartVal
  });
  $('#event-end').datetimepicker({
    locale: 'bg',
    format: dateDisplayFormat,
    defaultDate: eventEndVal
  });
  $('#event-start').data("DateTimePicker").maxDate(eventEndVal.clone().subtract(15, 'minutes'));
  $('#event-end').data("DateTimePicker").minDate(eventStartVal.clone().add(15, 'minutes'));
  $('#event-start-format').val(eventStartVal.format('YYYY-MM-DD HH:mm'));
  $('#event-end-format').val(eventEndVal.format('YYYY-MM-DD HH:mm'));

  // събитията са тук защото дава грешка при показването на прозореца
  $('#event-start').on('dp.change', function(e){
    $('#event-end').data("DateTimePicker").minDate(e.date.clone().add(15, 'minutes'));
    $('#event-start-format').val(e.date.format('YYYY-MM-DD HH:mm'));
    buildNotificationsLocal($('#notes-form form'));
  });
  $('#event-end').on('dp.change', function(e){
    $('#event-start').data("DateTimePicker").maxDate(e.date.clone().subtract(15, 'minutes'));
    $('#event-end-format').val(e.date.format('YYYY-MM-DD HH:mm'));
    buildNotificationsLocal($('#notes-form form'));
  });
}

function changeLocalNotifyUnits(elem){
  elem.closest('.before-container').find('.before-amount > option').addClass('hidden').filter(elem.children(':selected').data('target')).removeClass('hidden').first().prop('selected', true);
}

function toggleExternalNotify(elem){
  var thisForm = elem.closest("form");
  if(elem.is(':checked')){
    thisForm.find('#date-interval select').change();
    thisForm.find('.checked-collapse, #date-start input').prop('disabled', false);
    thisForm.find('.checked-collapse:checked').change();
    thisForm.find('[data-target^=".add-note"]').prop('disabled', false);
    thisForm.find('[data-target^=".add-note"]').change();
    thisForm.find('.remind-body').removeClass('disabled');
  }else{
    thisForm.find('input[name^="NotificationsExternal"]').remove();
    thisForm.find('.remind-body select, .remind-body :not(legend) input').prop('disabled', true);
    thisForm.find('.input-group-btn').addClass('disabled');
    thisForm.find('.remind-body').addClass('disabled');
  }
}

function editExternalNotifyIntervalDropdown(){
  var difference = moment($('#date-end-format').val()).diff($('#date-start-format').val());
  if(difference <= 3600000){
    $('#date-interval select > option.stage-1').removeClass('hidden')
      .siblings(':not(.stage-1)').addClass('hidden');
    if($('#date-interval select > option:selected').hasClass('hidden')){
      $('#date-interval select > option.stage-1').each(function(){
        if(
          (difference >= $(this).val() && difference < $(this).nextAll('.stage-1').first().val()) ||
          $(this).val() == 900000
        ){
          $(this).prop('selected', true);
        }
      });
    }
  }
  else if(difference <= 86400000){
    $('#date-interval select > option.stage-2').removeClass('hidden')
      .siblings(':not(.stage-2)').addClass('hidden');
    if($('#date-interval select > option:selected').hasClass('hidden')){
      $('#date-interval select > option[value="3600000"]').prop('selected', true);
    }
  }
  else{
    $('#date-interval select > option.stage-3').removeClass('hidden')
      .siblings(':not(.stage-3)').addClass('hidden');
    if($('#date-interval select > option:selected').hasClass('hidden')){
      $('#date-interval select > option[value="86400000"]').prop('selected', true);
    }
  }
}


function refreshOrganiser(){
  buildDay($('#org-day > .single-container'), $('#Organiser .prev-next .day').data('current'), {day:true});
  buildWeek($('#org-week'), $('#Organiser .prev-next .week').data('current').clone());
  buildMonth($('#org-month'), $('#Organiser .prev-next .month').data('current').clone(), {month:true});
  buildYear($('#org-year'), $('#Organiser .prev-next .year').data('current').clone());
}
