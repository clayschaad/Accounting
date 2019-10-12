var helper = {
    // debug flag, if set to true, we log to the console...
    debug: false,

    log: function () {
        // logs to the console if debug is enabled and a console object is available
        if (!helper.debug) {
            return;
        }
        if (!window.console) {
            //alert(arguments)
        } else {
            console.log(arguments);
        }
    },

    init: function() {
        Highcharts.setOptions({
            lang: {
                decimalPoint: '.',
                thousandsSep: '\'',
            }
        });
    },

    setScrollbar: function(selector) {
        $(selector).css('max-height', function () {
            return $(window).height() - 260 + 'px';
        }).css('overflow-y', 'auto');
    },

    createDatepicker: function(selector) {
        
        $(selector).datepicker(
        {
            dateFormat: "dd.mm.yy"
        });
        $(selector).datepicker($.datepicker.regional["de"]);
    },

    createDataTable: function (selector) {
        // https://datatables.net/
        $.fn.dataTable.moment('D.MM.YYYY');
        $(selector).DataTable({
            paging: false,
            language: {
                decimal: ".",
                url: 'https://cdn.datatables.net/plug-ins/1.10.10/i18n/German.json'
            }
        });
    },

    createAccountList: function (selector) {
        helper.get(
             '/Account/GetAccountList',
             function (data) {
                 $(selector).select2({
                     data: data,
                     language: "de",
                 });
                 
                $(selector).on("select2:select", function (e) 
                { 
                    var accountId = e.params.data.id;
                    helper.get("/Account/IsFxAccount/" + accountId, function (ret)
                    {
                        var fxRateRow = $(e.target.form).find(".fxrate");
                        if (ret)
                        {
                            fxRateRow.show();
                        }
                        else 
                        {
                            fxRateRow.hide();
                        }
                    });
                });

             }
       );
    },

    createBookingTextList: function (selector) {
        helper.get(
            '/BookingText/GetBookingTextList',
            function (data) {
                $(selector).autocomplete({
                    source: data
                });
            }
      );
    },

    validateTransaction: function(form) {
        // validate
        if (!$(form).find('input[name="Text"]').val()) {
            alert("Bitte Text erfassen!");
            return false;
        }
        if (!$(form).find('input[name="OriginAccountId"]').val()) {
            alert("Bitte Konto erfassen!");
            return false;
        }
        if (!$(form).find('input[name="TargetAccountId"]').val()) {
            alert("Bitte Konto erfassen!");
            return false;
        }
        return true;
    },

    setEditTransactionClick: function (selector) {
        $(selector)
          .button()
          .click(function (event) {
              event.preventDefault();
              var id = $(this).data('id');
              if (id == undefined) id = "";
              helper.get(
                  "/Transaction/Edit/" + id,
                  function (res) { helper.showTransactionDialog(res); }
                  );
          });
    },
    
    setEditClick: function (selector, controller) {
        $(selector)
          .button()
          .click(function (event) {
              event.preventDefault();
              var id = $(this).data('id');
              if (id == undefined) id = "";
              helper.get(
                  "/" +  controller + "/Edit/" + id,
                  function (res) { helper.showEditDialog(res, controller); }
                  );
          });
    },
    
    showEditDialog: function (content, controller) {
        $('#dialog').html(content);
        $('#dialog').dialog({
            width: 700
        });

        helper.createAccountList(".accountList");
        helper.createBookingTextList(".textList");
        helper.createDatepicker(".datepicker");

        $('form').on('submit', function (e) {
            e.preventDefault();
            
            if (helper.validate($(this)) == false) {
                return;
            };
            
            var url = $(this).attr('action') || window.location.pathname;
            var data = JSON.stringify(helper.serializeSimpleForm(this));
            var type = $(this).attr('method');

            helper.ajax(
                url,
                data,
                {
                    type: type,
                    dataType: 'json'
                },
                function()
                {
                    window.location = '/' + controller;
                }
            );
        });
    },
    
    validate: function(form) {
        // validate
        var ret = true;
        $(form).find('input').each(function () {
            var name = $(this).attr('name');
            var doValidate = $(this).data('validate');
            if (doValidate && !$(this).val()) {
                alert("Bitte Feld '" + name + "' erfassen!");
                ret = false;
                return;
            }
        });
        return ret;
    },

    addPartialHtml: function(url, target, callback){
        helper.get(
            url,
            function (content) {
                $(target).html(content);
                if (callback) callback();
            }
        );
    },

    showOpenBankTransactionDialog: function (content) {
        $('#dialog').html(content);
        $('#dialog').dialog({
            width: 1000,
            height: 600,
            close: function (event, ui)
            {
                location.reload();
            }
        });

        $('#dialog').tooltip({
            position: {
                my: "center bottom-5",
                at: "center top",
            }
        });

        helper.createAccountList(".accountList");
        helper.createBookingTextList(".textList");
        
        $('form').on('submit', function (e) {
            e.preventDefault();
   
            if (helper.validateTransaction($(this)) == false) return;

            var form = $(this);
            var url = $(this).attr('action') || window.location.pathname;
            //var data = JSON.stringify(helper.serializeForm(this)[0]);
            var data = JSON.stringify(helper.serializeSimpleForm(this));
            var type = $(this).attr('method');

            helper.ajax(
                url,
                data,
                {
                    type: type,
                    dataType: 'json'
                },
                function () {
                    $(form).remove();
                }
            );         
        });
    },

    showTransactionDialog: function (content) {
        $('#dialog').html(content);
        $('#dialog').dialog({
            width: 700
        });

        helper.createAccountList(".accountList");
        helper.createBookingTextList(".textList");
        helper.createDatepicker(".datepicker");
        
        $('.getFxRate')
          .button()
          .click(function (event) {
              event.preventDefault();   
              alert("Aktuell nicht verfügbar da kostenpflichtig!");
              return;
              
              var originAcountId = $('input[name="OriginAccountId"]').val();
              var targetAcountId = $('input[name="TargetAccountId"]').val();
              var date = helper.formatStringDate($('input[name="ValueDate"]').val());
       
              var url = "/Transaction/GetFxRate?originAccountId=" + originAcountId + "&targetAccountId=" + targetAcountId + "&date=" + date;
              helper.get(
                  url,
                  function (res) { 
                           $('input[name="FxRate"]').val(res);
                  }
              );
          });

        $('form').on('submit', function (e) {
            e.preventDefault();
            
            if (helper.validateTransaction($(this)) == false) {
                return;
            };
            
            var url = $(this).attr('action') || window.location.pathname;
            var data = JSON.stringify(helper.serializeSimpleForm(this));
            var type = $(this).attr('method');

            helper.ajax(
                url,
                data,
                {
                    type: type,
                    dataType: 'json'
                },
                function()
                {
                    window.location = '/Transaction';
                }
            );
        });
    },

    showSplitBankTransactionDialog: function (content) {
        $('#dialog').html(content);
        $('#dialog').dialog({
            width: 1000,
            height: 600
        });

        $('#dialog').tooltip({
            position: {
                my: "center bottom-5",
                at: "center top",
            }
        });

        helper.createAccountList(".accountList");
        helper.createBookingTextList(".textList");
        helper.createDatepicker(".datepicker");

        $(document).on('change', 'input[name="Value"]', function () {
            helper.calculateSplitForm();
        });

        $('#addEmptyTransaction')
            .button()
            .click(function (event) {
                var id = $(this).data('id');
                helper.get(
                    "/Home/GetEmptyTransaction/" + encodeURIComponent(id),
                    function (content) {
                        $('#saveSplit').before(content);
                        helper.createAccountList(".accountList");
                        helper.createBookingTextList(".textList");
                        helper.createDatepicker(".datepicker");
                        helper.calculateSplitForm();
                    }
                );
            });

        $('#addSplitPredefinition')
            .button()
            .click(function (event) {
                var id = $(this).data('id');
                helper.get(
                    "/Home/GetSplitPredefiniton/" + encodeURIComponent(id),
                    function (content) {
                        $('#saveSplit').before(content);
                        helper.createAccountList(".accountList");
                        helper.createBookingTextList(".textList");
                        helper.createDatepicker(".datepicker");
                        helper.calculateSplitForm();
                    }
                );
            });

        $('#importCreditCardStatement')
            .button()
            .click(function (event) {
                var id = $(this).data('id');
                helper.get(
                    "/Home/GetCreditCardStatement/" + encodeURIComponent(id),
                    function (content) {
                        $('#saveSplit').before(content);
                        helper.createAccountList(".accountList");
                        helper.createBookingTextList(".textList");
                        helper.createDatepicker(".datepicker");
                        helper.calculateSplitForm();
                    }
                );
            });

        $('#formSplit').on('submit', function (e) {
            e.preventDefault();

            var url = $(this).attr('action') || window.location.pathname;
            var data = JSON.stringify(helper.serializeForm(this));
            var type = $(this).attr('method');

            helper.ajax(
                url,
                data,
                {
                    type: type,
                    dataType: 'json'
                },
                function()
                {
                    window.location = '/';
                }
            );
        });
    },

    calculateSplitForm: function ()
    {
        // calc sum of transaction
        var sum = 0;
        $("input[name='Value']").each(function () {
            sum += Number($(this).val());
        });
        // show rest to total balance
        var rest = Math.round(100 * (Number($('#totalBalance').text().replace('\'', '')) - sum)) / 100;
        $('#restBalance').html(rest);
        // enable submit button if no rest
        if (rest == 0) $('#saveSplit').removeAttr('disabled');
        else $('#saveSplit').attr('disabled');
    },

    serializeForm: function(form)
    {
        var array = new Array();
        $(form).find('.row').each(function () {
           var element = helper.serializeSimpleForm(this);
            array.push(element);
        });
        return array;
    },
    
    serializeSimpleForm: function(form)
    {
        console.log($(form));
        var element = {};
        $(form).find('input').each(function () {
            console.log($(this));
            if ($(this).attr('name') == 'ValueDate' || $(this).attr('name') == 'BookingDate') {
                element[$(this).attr('name')] = helper.stringToDate( $(this).val());
            }
            else if($(this).attr('name') != undefined) {
                element[$(this).attr('name')] = $(this).val();
            }
        });
        return element;
    },

    stringToDate: function(stringDate)
    {
        var pattern = /(\d{2})\.(\d{2})\.(\d{4})/;
        stringDate = new Date(stringDate.replace(pattern, '$3-$2-$1'));
        return stringDate;
    },
    
    formatStringDate: function(stringDate)
    {
        var pattern = /(\d{2})\.(\d{2})\.(\d{4})/;
        var date = new Date(stringDate.replace(pattern, '$3-$2-$1'));
        return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
    },

    numberToString: function(value)
    {
        return value.toLocaleString('de-CH', {minimumFractionDigits: 2}); // 10'000.00
    },

    get: function (url, successCallback) {
        helper.ajax(
             url,
             {},
             {},
             successCallback
       );
    },

    ajax: function (url, data, options, successCallback, errorCallback) {
        // default options. 
        var defaultOptions = {
            async: true,
            cache: false,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            crossDomain: true,
        };


        $.extend(defaultOptions, options);
        defaultOptions.url = url;
        defaultOptions.data = data;
        // call the webservice
        var request = $.ajax(defaultOptions)
        .done(function (result) {

            if (successCallback != null) {
                successCallback(result);
            }

            // logging
            helper.log("success", url, result);
        })
        .fail(function (xhr, textStatus, errorThrown) {
            // log
            alert("Request: " + xhr.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
            helper.log(textStatus, url, errorThrown, xhr.responseText, defaultOptions);

            if (errorCallback != null) {
                errorCallback();
            }
        });
    },
    
    showLineChart: function(selector, title, subtitle, yAxis, data) 
    {   
        $(selector).highcharts({
            chart: {
                height: 700,
                type: 'line'
            },
            title: {
                text: title,
                x: -20 //center
            },
            subtitle: {
                text: subtitle,
                x: -20
            },
            xAxis: {
                categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
            },
            yAxis: {
                title: {
                    text: yAxis
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                valueSuffix: yAxis
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                borderWidth: 0
            },
            series: data
        });
    },

    showColumnChart: function(selector, title, subtitle, yAxis, data) 
    {   
        $(selector).highcharts({
            chart: {
                height: 700,
                type: 'column'
            },
            title: {
                text: title,
                useHTML: true,
                x: -20 //center
            },
            subtitle: {
                text: subtitle,
                x: -20
            },
            xAxis: {
                categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
            },
            yAxis: {
                title: {
                    text: yAxis
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                },
                {
                    value: data.median,
                    width: 1,
                    zIndex: 5,
                    color: '#ff0000'
                }]
            },
            tooltip: {
                valueSuffix: yAxis
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                borderWidth: 0
            },
            series: data.series
        });
    },

    showPieChart: function(selector, title, data, getDetailCallback)
    {
        // Build the chart
        $(selector).highcharts({
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
            },
            title: {
                text: title
            },
            tooltip: {
                //pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                pointFormat: '<b>{point.y:,.2f} CHF</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        format: '<b>{point.name}</b>: {point.y:,.2f} CHF',
                        style: {
                            color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                        }
                    },
                    showInLegend: false,
                    point: {
                        events: {
                            click: function () {
                                if (getDetailCallback != null) {
                                    getDetailCallback(this.options.id, this.options.name);
                                }
                            }
                        }
                    }
                }
            },
            series: [{
                //name: 'Brands',
                colorByPoint: true,
                data: data
            }]
        });

    },
}