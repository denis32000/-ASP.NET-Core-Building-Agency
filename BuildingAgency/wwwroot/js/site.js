// Write your Javascript code.


//$('#sortBy').click(function (e) {
//    e.preventDefault();
//
//    ajaxFunction('@Url.Action("SortedIndexList", "Clients", 2)'); // , new { login = "1", password = "1" }, null) TODO: login password
//})

//$("#formoid").submit(function (event) {
//
//    /* stop form from submitting normally */
//    event.preventDefault();
//
//    /* get the action attribute from the <form action=""> element */
//    var $form = $(this),
//        url = $form.attr('action');
//
//    /* Send the data using post with element id name and name2*/
//    $.post(url, $("#formoid").serialize());
//    ajaxFunction('@Url.Action("Index", "Branches")');
//
//    //$.post(url, $("#formoid").serialize()).done(function () {
//    //    ajaxFunction('@Url.Action("Index", "Branches")')
//    //});
//})

/*

                <select class="form-control" onchange="ajaxSortFunction(this.selectedIndex);">
                    @for(int i = 0; i < ViewBag.SortParams.Count; i++)
                    {
                        <option>@ViewBag.SortParams[i]</option>
                    }
                </select>
*/


//function ajaxSortFunction(selectedIndex, list) {
//    ajaxFunction('Clients/SortedClients?id=' + selectedIndex);
//}

//function ajaxSearchFunction(selectedIndex, inputValue) {
//    ajaxFunction('Clients/SearchedClients?id=' + selectedIndex + '&searchingParam=' + inputValue);
//}

function ajaxApplyFilters(controller, sortParam, searchParam, searchInput) {
    ajaxFunction(controller + '/ApplyFilters'
        + '?sortParam=' + sortParam
        + '&searchParam=' + searchParam
        + '&searchInput=' + searchInput);
}

//


function ajaxExtendedSearch(controller, sortParam, param1, param2, param3, param4, param5, param6) {
    //console.log(' ' + controller + ' ' + sortParam + ' ' + param1 + ' ' + param2 + ' ' + param3 + ' ' + param4 + ' ' + param5 + ' ' + param6);
    ajaxFunction(controller + '/ExtendedSearch'
        + '?sortParam=' + sortParam
        + '&propertyNo=' + param1
        + '&city=' + param2
        + '&type=' + param3
        + '&postcode=' + param4
        + '&ownerPassport=' + param5
        + '&staffPassport=' + param6);
};

function ajaxExtendedSearch2(controller, sortParam, param1, param2, param3, param4, param5, param6) {
    console.log(' ' + controller);
    ajaxFunction('ExtendedSearch2'
        + '?sortParam=' + sortParam
        + '&propertyNo=' + param1
        + '&city=' + param2
        + '&type=' + param3
        + '&postcode=' + param4
        + '&ownerPassport=' + param5
        + '&staffPassport=' + param6);
};

function ajaxFunction(url) {
    console.log("" + url);
        $.ajax({
            url: url,
            success: function (data) {
                //console.log("" + data);
                $(".main-block").html(data);
            }
        });
    }

