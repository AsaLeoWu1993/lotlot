let version = '1.0.32';
function addJS(src) {
    var js = document.createElement('script');
    js.onerror = function(e, src) {
        e.target.src = src;
    }
    js.setAttribute('src', src);
    document.body.appendChild(js);
}
addJS(`/m/static/js/chunk-vendors.${version}.js`);
addJS(`/m/static/js/app.${version}.js`);