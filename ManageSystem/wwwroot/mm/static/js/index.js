let version = '1.0.8';
function addJS(src) {
    var js = document.createElement('script');
    js.setAttribute('src', src);
    document.body.appendChild(js);
}
addJS(`/mm/static/js/chunk-vendors.${version}.js`);
addJS(`/mm/static/js/app.${version}.js`);