$(function() {
    window.addEventListener("message", function(event) {
        var item = event.data;
        
        if (item.show) {
            if (item.show == -1) {
                $("#container").hide();
            } else {
                $("#container").show();
                $("#counter").text(item.show);
            }
        }
    });
});