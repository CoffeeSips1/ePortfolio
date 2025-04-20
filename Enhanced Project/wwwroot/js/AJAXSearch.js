//  AJAX query counter
let AJAXcounter = 0;
let AJAXMeta = "";

function AJAXSearch(url, searchQuery, updateLocation, meta = AJAXMeta) {

    //  AJAX Instance for this function call
    let cur = ++AJAXcounter;

    //  AJAX Request
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (cur == AJAXcounter) {
                //  Update webpage element
                document.getElementById(updateLocation).innerHTML = this.responseText;
            } else {
                return;
            }
        }
    }
    xmlhttp.open("GET", url + "?searchQuery=" + searchQuery + "&meta=" + meta, true);
    xmlhttp.send();
};

function AJAXFetch(url, Id, updateLocation, meta = AJAXMeta) {

    //  AJAX Instance for this function call
    let cur = ++AJAXcounter;

    //  AJAX Request
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (cur == AJAXcounter) {
                //  Update webpage element
                document.getElementById(updateLocation).innerHTML = this.responseText;
            } else {
                return;
            }
        }
    }
    xmlhttp.open("GET", url + "?Id=" + Id + "&meta=" + meta, true);
    xmlhttp.send();
};
