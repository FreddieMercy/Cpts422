function timer(second){
	if(second > 1){
		$("#timer").text(second);
		second--;	
		setTimeout(function(){timer(second)},1000);
					
	}else{
		$("#timer").text(second);
		second--;
		$("html").fadeOut(1000,function(){
		$("#timer").text("0");
		window.open("/myWeb/posts/Home.php").document.getElementById('LC').click();
		window.open
		});	


	}
};

$(document).ready(function() {
	
	$("#sidebar1 div").hide().parent().attr("align","center");
	
	$("#sidebar1 nav ul li").each(function(){
			
			$(this).children().attr("href","#");

	 });
	 
	var second = 5;
		
	timer(second);
	 
	 
	
});

