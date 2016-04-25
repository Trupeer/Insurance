(function(){
    function o(){document.documentElement.style.fontSize=(document.documentElement.clientWidth>1280?1280:document.documentElement.clientWidth)/12+"px"}
    var e=null;
    window.addEventListener("resize",function(){clearTimeout(e),e=setTimeout(o,300)},!1),o()
})(window);