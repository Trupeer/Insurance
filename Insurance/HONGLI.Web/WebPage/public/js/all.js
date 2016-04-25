$(function(){
    document.body.addEventListener('touchstart', function () {});
});

// 返回上一页
$("#riaGoBack").click(function(event){
    window.history.go(-1);
    event.preventDefault();
    event.stopPropagation();
});

function countDown(maxTime, fn1, fn2) {
    var timer = setInterval(function(){
        if(maxTime > 1){
            var leftTime = --maxTime;
            fn1(leftTime);
        }else{
            clearInterval(timer);
            fn2();
        }
    }, 1000);
}

$(function(){
    $(".f-list01").on("click", ".f-list01-icon-arrow-down", function(){
        var obj = $(this).data("sh");
        $(obj).hide();
        $(this).addClass("f-list01-icon-arrow-up");
        $(this).removeClass("f-list01-icon-arrow-down");
    });

    $(".f-list01").on("click", ".f-list01-icon-arrow-up", function(){
        var obj = $(this).data("sh");
        $(obj).show();
        $(this).addClass("f-list01-icon-arrow-down");
        $(this).removeClass("f-list01-icon-arrow-up");
    });
});