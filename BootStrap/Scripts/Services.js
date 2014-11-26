

function swapTiles(source, target) {

    var sourceSize = $(source).parent().parent().attr('size');
    var targetSize = $(target).parent().parent().attr('size');
    if (sourceSize == "small" && targetSize == "small") //if we swap two small tiles
    {
        swap(source, target);
        swapDivsId(source, target);
    }
    else if (sourceSize == "small" && targetSize == "large") {
        swap(source, target);       
    }
    else if (sourceSize == "large" && targetSize == "large") {
        swap(source, target);
        swapDivsId(source, target);
    }
    else if (sourceSize == "large" && targetSize == "small") {
        resizeAndSwap(source, target);
        swapDivsId(source, target);
    }   
}

function swap(source, target) {
    alert("in swap");
    var something = {
        sId: 0,
        tId: 0
    }

    swapDivs(source, target);
}

function swapDivs(source, target) {
    alert("in swapDivs"+"  source= "+source+" target= " +target);
    var resultObject = {
        SourceDivId: source,
        TargetDivId: target
    }
    $("#loadIndicator").show();
    $("body").css({ 'opacity': '0.3' })
    $.ajax({
        type: "POST",
        data: resultObject,
        async:false,
        url: '/Home/SwapDivs/',
        dataType: "json",
        // contentType: "application/json; charset=utf-8",
        success: function (result) {
            LoadDataById(result)
        },
        error: function (res) {
            alert(result.TargetDataId, result.SourceDataId);
            alert("Hello,I'm Exception!" + result);
        }
    });
}

function LoadDataById(result) {
    var heightSmall = 300;
    var widthSmall = 300;
    var heightLarge = 400;
    var widthLarge = 505;
    var res;
    var source = result.SourceDivId;
    var target = result.TargetDivId;
    var _sourceDivName = "#Div" + result.SourceDivId;
    var _targetDivName = "#Div" + result.TargetDivId;
    var sourceSize = $(_sourceDivName).parent().parent().attr('size');
    var targetSize = $(_targetDivName).parent().parent().attr('size');
    if (sourceSize == 'small' && targetSize == 'large') {
        $(_sourceDivName).load('/grf.aspx?id=' + result.TargetDataId + '&width=' + widthSmall + '&height=' + heightSmall).css({ "left": "0px", "top": "0px" });
        $(_targetDivName).load('/grf.aspx?id=' + result.SourceDataId + '&width=' + widthLarge + '&height=' + heightLarge).css({ 'left': '0px', 'top': '0px' });
        $("#Div4").attr("style", "");
        $("#Div4").css({ "left": "0px", "top": "0px" });
        alert($("#Div4").attr("style"));
        $("body").css({ 'opacity': '1' })
        $("#loadIndicator").hide();
    }
    return result;
}
function swapDivsId(source, target) {
    var tempId = $(target).attr('id');
    //swap div's id
    $(target).attr('id', $(source).attr('id'))
    $(source).attr('id', tempId)
    $(source).attr('span', "the id is:" + $(source).attr('id'))
    $(target).attr('span', $(target).attr('id'))
}

function resizeAndSwap(source, target) {
    var sourceSize = $(source).attr('size');
    var targetSize = $(target).attr('size');
    alert("source in sizes = " + $(source).attr('class'));
    alert("resize : s=" + $(source).attr('size') + "  t= " + $(target).attr('size'));
    if (sourceSize == "SmallImage" && targetSize == "LargeImage") { //small->large 250*350->400*500
        alert("small->large");
        $(source).removeClass('SmallImage').addClass('LargeImage');
        $(source).attr('size', 'LargeImage');
        $(target).removeClass('LargeImage').addClass('SmallImage');

        swap(source, target);
        $(target).children().find('img').attr('style', 'width:500px;height:350px;');
    }
    else  //large->small 400*500->250*350 
    {
        alert("large->small");
        $(source).removeClass('LargeImage').addClass('SmallImage');
        $(target).removeClass('SmallImage').addClass('LargeImage');
    }
}
function GetDivId(DivName) {
    var resultObject = {
        SourceDivName: DivName,
        SourceDivId: 0,
        SourceDataId: 0,
        SourceViewId:0
    }

    $.ajax({
        async: false,
        type: "POST",
        data: resultObject,
        url: '/Home/GetDivId/',

        dataType: "json",
        success: function (result) {
            GetDataId(result);
            resultObject.SourceDataId = result.SourceDataId;
            resultObject.SourceViewId = result.SourceViewId;
            resultObject.SourceDivId = result.SourceDivId;
           // alert("******** di=" + resultObject.SourceDivId);
        },
        error: function (res) {
            alert("Hello,I'm Exception!" + result);
        }
    });
   
    return resultObject;//.SourceDataId;
}

function GetDataId(resultObject)
{
    var source = resultObject.SourceDivId;
    $.when(
    $.ajax({
        async: false,
        type: "POST",
        data: resultObject,
        async:false,
        url: '/Home/FindChartId/',
        dataType: "json",
        success: function (result) {
            resultObject.SourceDataId = result.SourceDataId;
            resultObject.SourceViewId = result.ViewId;
            resultObject.SourceDivId = result.SourceDivId;
        },
        error: function (result) {
            alert("Hello,I'm Exception!" + result);
        }
    }));

}
//function GetCurrentDaySupports(supportsOfTheDayQuery,supportsOfTheDayPlace)
function GetCurrentDaySupports(supportsOfTheDayPlace)
{
    alert("eeeee");
    var currentDaySupportsList = [];   
    //var query = supportsOfTheDayQuery.replace(/&#39;/g, "'");
    var DivData =
    {
        _queryString: query
      // _divId: supportsOfTheDayPlace
    }
    $.ajax({
        type: "POST",
        data:DivData,
        url: '/Home/GetCurrentDaySupports/',
        dataType: "json",
        success: function (currDaySuppoortsList) {
            $.each(currDaySuppoortsList, function (key, item) {
                var CurrentDaySupports=
                {
                    FrameName:item["FrameName"] ,
                    Supporter:item["Supporter"],
                    NumOfSupports: item["NumOfSupports"],
                    DayName: item["DayName"]
                }
                currentDaySupportsList.push(CurrentDaySupports);
            });           
           insertToCurrentDaySuppTable(currentDaySupportsList);         
        },
        error: function (currDaySuppoortsList) {
            alert("Hello,I'm Exception!" + suppList);
        }
    });  
}

function GetSuppliersList(_suppQueryString , _suppPlace)
{
    var ListSuppliers  = [];
    var Suppliers=
    {
        Area:null,
        FrameName:null,
        NumberOfSupporters: 0
    }
    var DivData=
    {
        _queryString: _suppQueryString,
        _divId: _suppPlace
    }
   
    $.ajax({
        type: "POST",
        data: DivData,
        url: '/Home/GetSuppliersList/' ,

        dataType: "json",
        success: function (suppList) {
           
            $.each(suppList, function (key, item) {
                var Suppliers=
                {
                    Area : item["Area"],
                    FrameName : item["FrameName"],
                    NumberOfSupporters:item["NumberOfSupporters"]
                }
                ListSuppliers.push(Suppliers);
            });
            insertToTable(ListSuppliers);        
        },
        error: function (suppList) {
            alert("Hello,I'm Exception!" + suppList);
        }
    });
}
function insertToCurrentDaySuppTable(currDaySupportsList) {
    alert("aaaa");
    $("#SupportsOfTheCurrentDay tbody").remove();
    $.each(currDaySupportsList, function (key, item) {
        var x = '<tr><td>' + item.FrameName + '</td><td' + item.Supporter + '</td><td>' + item.NumOfSupports + '</td>' + '</td><td>' + item.DayName + '</td></tr>';
        $("#SupportsOfTheCurrentDay").append(x);
    });
    $("#SupportsOfTheCurrentDay").parent().css('overflow-y', 'scroll');
}

function GetAllSupports(supporterName){
    alert(supporterName);
}
function insertToTable(ListSuppliers)
{
    $("#SupportersTable tbody").remove();
    $.each(ListSuppliers, function (key, item) {
        var x = '<tr><td>' + item.Area + '</td><td>' + item.FrameName + '</td><td>' + item.NumberOfSupporters + '</td></tr>';
        $("#SupportersTable").append(x);          
    });
    $("#SupportersTable").parent().css('overflow-y', 'scroll');
}

function DrillDown(urlString) {
   
    urlString = urlString.replace(/\s/g, "%20")+"&zoomed=1";
    var temp = '';
    var strToFormat = '';
    var i = 0;
    var j = 0;
    var urlToFormat = urlString.toString();
    var len = urlToFormat.length;
    while (i < len)
    {
        if ((urlToFormat.charCodeAt(i) >= 1488) && (urlToFormat.charCodeAt(i) <= 1514))
        {
            strToFormat = strToFormat + urlToFormat[i];
            j = j + 1; i = i + 1;
        }
            //take care of hebrew characters
        else if ((urlToFormat.charCodeAt(i) < 128) || (urlToFormat.charCodeAt(i) > 155))
        {
            if (j == 0) {
                temp = temp + urlToFormat[i];
                i = i + 1;
            }
            else {
                strToFormat = encodeURIComponent(strToFormat).replace(/'/g, "%27").replace(/"/g, "%22");
                temp = temp + strToFormat + urlToFormat[i];
                i = i + 1;
                strToFormat = '';
                j = 0;
            }
        }
    }
    $("#Zooming").load(temp);
    setTimeout(function () {
    var usemap = $("#Zooming img").attr('usemap') + "Zoomed"
    $("#Zooming img").attr('usemap', usemap);
    var mapId = $("#Zooming map").attr('id') + "Zoomed";
    $("#Zooming map").attr('id', mapId);
    }, 3000);
    var DataId = $("#Zooming").attr('sourceDataId');
    var ViewId = $("#Zooming").attr('sourceViewId');
    var sourceDivId = $("#Zooming").attr('sourceDivId');
    alert("DataId = " + DataId + " ViewId = " + ViewId + " sourceDivId = " + sourceDivId);
}

function Zoom(DivName) {
    var resultObject = {
        SourceDivName: DivName,
        SourceDivId: 0,
        SourceDataId: 0,
        SourceViewId: 0
    };
    $("body").css({ 'opacity': '0.3' })
    $('#Zooming').empty();
    resultObject = GetDivId(resultObject.SourceDivName);
    
    var ChartId = resultObject.SourceDataId;
    var ViewId = resultObject.SourceViewId;
    var DivId = resultObject.SourceDivId;
    $("#Zooming").attr('sourceDataId', ChartId);
    $("#Zooming").attr('sourceViewId', ViewId);
    $("#Zooming").attr('sourceDivId', DivId);
    //alert(" sourceDataId = " + $("#Zooming").attr('sourceDataId') + "  sourceViewId = " + $("#Zooming").attr('sourceViewId') + " sourceDivId = "+ $("#Zooming").attr('sourceDivId'));
    if (ViewId == 1) { //zoom chart
        $("#Zooming").load('/grf.aspx?id=' + ChartId + '&width=' + 818+ '&height=' + 600 + '&zoomed=1'+'&divId='+DivId).css('text-align', 'left').css('vertical-aling', 'bottom').css('border','1px solid');
        setTimeout(function () {
            var usemap = $("#Zooming img").attr('usemap') + "Zoomed"
            $("#Zooming img").attr('usemap', usemap);
            var mapId = $("#Zooming map").attr('id') + "Zoomed";
            $("#Zooming map").attr('id', mapId);
            $("#ZoomDivContainer").show();
        }, 3000);
    }
    else if (ViewId == 2) { //zoom table
        //alert(DivName);
        if (DivName=='#TableDiv1') {
            var dataList = $("#TableDiv1HiddenField").val();
            var queryString = $("#TableDiv1HiddenField").val();
           // alert("querystring is "+queryString);
            $("#hiddenQuery").val(queryString);
           
        }
        else if  (DivName=='#TableDiv2') {
            var dataList = $("#TableDiv2HiddenField").val();
            $("#hiddenQuery").val("hello");
            alert($("#hiddenQuery").val());
        }
        //var objList = $.parseJSON(dataList);
        //var jsonResultData = ConvertJsonToTable(objList, null, null, null);
        //$("#Zooming").html(jsonResultData);
        $("#Zooming").html(dataList);
        setZoomView();
            $("#ZoomDivContainer").show();
    }
  
}

function setZoomView()
{
    var width = $("#ZoomDivContainer").width();
    var height = $("#ZoomDivContainer").height();
    $("#ZoomDivContainer").css('overflow-y', 'scroll')
    $("#Zooming>table").css('background-color', '#FFFFFF').css('width', width).css('height', height).addClass("table table-responsive table-hover table-condensed table-striped");
}

function ZoomIn(){
    var DivName = "#" + $(this).attr('activeDiv');
    $(this).css('background-color', 'white');
    $(this).removeClass('glyphicon-zoom-in').addClass('glyphicon-zoom-out').attr('title', 'הקטן').css(' background', 'transparent');
    var DivId = GetDivId(DivName);
    var p = $(this).offset();
    var w = $(this).width();
    var h = $(this).height();
    $("body").css({ 'opacity': '0.3' })
    var $clone = $(this).clone().load('/grf.aspx?id=' + DivId + '&width=' + 500 + '&height=' + 400);
    $clone.css({
        position: "absolute",
        left: p.left + "px",
        top: p.top + "px",
        "z-index": 2
    }).appendTo('body');
    $clone.data("origWidth", w);
    $clone.data("origHeight", h);
    $clone.data("origTop", p.top);
    $clone.data("origLeft", p.left);
    $clone.animate({
        "border-right-width": 2 + "px",
        "border-left-width": 2 + "px",
        "border-bottom-width": 2 + "px",
        "border-top-width": 2 + "px",
        "border-top-color": "black",
        "border-top-style": "solid",
        "border-bottom-color": "black",
        "border-bottom-style": "solid",
        "border-left-color": "black",
        "border-left-style": "solid",
        "border-right-color": "black",
        "border-right-style": "solid",
        top: 30 + "%",
        left: 30 + "%",
        width: 505 + "px",
        height: 420 + "px"

    }, function () {
    });
    $clone.click(ZoomOut);
}

function ZoomOut() {
    var w = $(this).data("origWidth");
    var h = $(this).data("origHeight");
    var t = $(this).data("origTop");
    var l = $(this).data("origLeft");
    $('#ZoomDiv1').removeClass('glyphicon-zoom-out').addClass('glyphicon-zoom-in').attr('title', 'הגדל').css('background', 'transparent');
    $(this).animate({
        top: t,
        left: l,
        width: w,
        height: h,

    }, function () {

        $(this).remove();
    });
    $("body").css({ 'opacity': '1' })

}

function  ExportChartToExcel(DivId,QueryKind)
{
    var _divId = "#" + DivId;
    alert(DivId);
    if (DivId == '#ZoomDivContainer')
    {
        var DataId = $("#Zooming").attr('sourceDataId');
        var ViewId = $("#Zooming").attr('sourceViewId');
        var sourceDivId = $("#Zooming").attr('sourceDivId');
        alert("DataId = " + DataId + " ViewId = " + ViewId + " sourceDivId = " + sourceDivId);
    }
    else {
        var DataId = $(DivId).attr('sourceDataId');
        var ViewId = $(DivId).attr('sourceViewId');
        var sourceDivId = $(DivId).attr('sourceDivId');
        alert("DataId = " + DataId + " ViewId = " + ViewId + " sourceDivId = " + sourceDivId);
    }
    window.open('/home/ExportToExcel/?DivId=' + sourceDivId + '&ViewId=' + ViewId + '&DataId=' + DataId + '&QueryKind=' + QueryKind, '_blank');
}


function Print() {
    var sqlQueryString = $("#hiddenQuery").val();
    window.open('/home/Print/?temp=' + sqlQueryString, '_blank');
    //window.open('/home/Print/?temp=' + sqlQueryString, '_blank');

}

function UpdateData(graphData) {
    //alert(graphApiUrl );
    $.ajax(graphApiUrl + "/"+4 ,{
       
       
        type: "PUT",
        data:graphData,
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });

   

    //var res = APIServer.updateData(graphData);
}
function somefunction(t) {
    alert(t);
    //var tr = $(t).closest('tr');
    //var id = $('.viki', tr).text();
}


function AddParamsToTable(tableName) {
    alert('hello');
    var idPlace = 0;
    var numOfTodaysSupportsPlace = 0;
    var DayName = 0;
  //  var FrameName = $(tableName).find('table thead tr th:eq(0)').html();
    for (var i = 0; i < 4; i++) {

        var p = $(tableName).find('table thead tr th:eq(' + i + ')').html();
        alert(p);
        if (p == 'Supporter') {
                SupporterPlace = i;
            }
            if (p == 'NumOfSupports') {
                numOfTodaysSupportsPlace = i;
            }
            if (p == 'SupporterJob') {
                DayName = i;
            }
    }
    
    $(tableName).find('table tbody tr').each(function(){
        $(this).addClass('test');
        var SupporterName = $(this).find('td:eq(' + SupporterPlace + ')').html();
        var numOfSupports = $(this).find('td:eq(' + numOfTodaysSupportsPlace + ')').html();
       //alert(numOfSupports);
       // $(this).find('td:eq(' + SupporterPlace + ')').addClass('supporter-action');
        
        $(this).find('td:eq(' + numOfTodaysSupportsPlace + ')').addClass('numOfSupports-action').attr('supporterName', SupporterName);

        //Get All Supports of Chosen Supporter
        $(this).find('td:eq(' + SupporterPlace + ')').on("click", function () {
            alert("-------going to getAllSupporterClients of " + SupporterName + '  ' + numOfSupports);
            getSupporterClients(SupporterName);
        });
       
        //get Todays Supports Of Supporter 
        $(this).find('td:eq(' + numOfTodaysSupportsPlace + ')').on("click", function () {
            alert("******going to getTodaysClient of " + SupporterName + '  ' + numOfSupports);
            getTodaysClients(SupporterName);
        });
    });

}

function getSupporterClients(supporterName)
{
    var Supporter =
        {
            SupporterName: supporterName
        }
    var s = JSON.stringify(supporterName);
    $.ajax({
        type: "POST",
        data: Supporter,
        url: '/Home/GetAllSupportsOfSelectedSupporter/',
        dataType: "html",
        success: function (newTable) {
            // alert(newTable);
            $("#selectedDataDiv").html(newTable);

            $("#selectedDataDiv").find('table td ').css({
                'border-color': 'darkgrey',
                'border-width': '1px',
                'font-size': 'small', 'border-style': 'solid'
            });
            $("#selectedDataDiv").find('table th ').css({
                'font-weight': 'bold',
                'border-color': 'darkgrey',
                'border-width': '1px',
                'font-size': 'small', 'border-style': 'solid'
            });
        },
        error: function (newTable) {
            alert(newTable);
            alert("Hello,I'm Exception!" + newTable);
        }
    });
}

function getTodaysClients(supporterName)
{
    alert("**** " + supporterName);
    var Supporter=
        {
            SupporterName:supporterName
        }
    var s = JSON.stringify(supporterName);
    $.ajax({
        type: "POST",
        data: Supporter,
        url: '/Home/GetSupporterCurrentDaySupports/',
        dataType: "html",
        success: function (newTable) {
           // alert(newTable);
            $("#selectedDataDiv").html(newTable);

            $("#selectedDataDiv").find('table td ').css({
            'border-color':'darkgrey',
            'border-width':'1px',
            'font-size':'small','border-style':'solid'
            });
            $("#selectedDataDiv").find('table th ').css({
                'font-weight':'bold',
                'border-color': 'darkgrey',
                'border-width': '1px',
                'font-size': 'small', 'border-style': 'solid'
            });
        },
        error: function (newTable) {
            alert(newTable);
            alert("Hello,I'm Exception!" + newTable);
        }
    });
}








