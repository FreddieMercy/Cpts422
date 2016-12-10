function _uni_key(key1,key2,t_id, o_bkg, o_clr){

	var spc = t_id+"selected";
	var spc_li = "#"+t_id+"selected li";
	var sel;
	var timer;
	var kr = true;
	var kl = true;

	console.log(spc);
	console.log(key1);
	$(document).keydown(function(e){
		if((e.which === key1)||(e.which === key2))
		{
			sel = document.getElementById(spc);
			sel.removeAttribute("id");
			sel = $(sel).removeClass("spotify_selected");
			//sel.css("background-color", o_bkg).children().css("color",o_clr);
		
			switch(e.which) {
	 			   case key1:
							if(sel.prev().text().length > 0)
							{
									sel = sel.prev();

							}else{
									sel.parent().attr("id",spc);
									sel = $(spc_li).last();
									document.getElementById(spc).removeAttribute("id");
								
							};
							clearTimeout(timer);
							kl = false;
						break;
	 			   case key2:
							if(sel.next().text().length > 0){
									
									sel = sel.next();
							}else{
					
									sel.parent().attr("id",spc);
									sel = $(spc_li).first();
									document.getElementById(spc).removeAttribute("id");
								
							};
							clearTimeout(timer);
							kr = false;
						break;
			};

			sel.addClass("spotify_selected").attr("id",spc);
		};
	}).keyup(function(ev){
		
		
	switch(ev.which) {
 			   case key1:
						kl = true;
        				break;
 			   case key2:

						kr = true;
        				break;
		};
	



		if(((ev.which === key1)||(ev.which === key2))&&(kr&&kl))
		{
			timer = setTimeout(function(){
			
			//sel.children().trigger("click");

			console.log("success~!!");

			},500);
		};	
		
		});
};

function _uni_click(t_id, o_bkg, o_clr){

	var tar = "#" + t_id + " div nav ul li a";
	var spc = t_id+"selected";

	$(tar).on("click",function(e){	
				
				var sel = document.getElementById(spc);
				sel.removeAttribute("id");
				$(sel).removeClass("spotify_selected").css("background-color", o_bkg).children().css("color",o_clr);
				$(this).parent().addClass("spotify_selected").attr("id",spc);


	  });
};


//*********************************************************************************************************
var _start = false;

function _notHome(sth2){
	$("._content").removeClass("_content");
	$(sth2).addClass("_content");
	$("._content").fadeIn(500);
};


function _fades(sth){
		var sth2 = " ";
		if(sth === "Home"){

			document.getElementById("homeVideo").play();
			$("#container").fadeIn(500);
			_start = true;

		}else{
			switch(sth) {

				case "About":
					sth2 = "#About";
					break;
				case "Photo Gallary":
					sth2 = "#Photo";
					break;
				case "Video Gallary":
					sth2 = "#Video";
					break;
				case "Music I Like":
					_uni_key(38,40,"Music", "#3f3939", "#FFF");
					_uni_click("Music", "#3f3939", "#FFF");
					sth2 = "#Music";
					break;
				case "Leave Comments":
					sth2 = "#Comments";
					break;
				case "Contact me":
					sth2 = "#Contact";
					break;
				default:

					break;
				};

			_notHome(sth2);

		};
};

function _get(sth) {

	$("._content").hide();
	
	if(_start === true){
		
		$("#container").fadeOut(500, function(){document.getElementById("homeVideo").pause();_fades(sth);});
		_start = false;

	}
	else{
		_fades(sth);
	};

};

function _mark(){

	$("#sidebar1 nav ul li").each(function(){
		if($(this).children().attr("href") === location.hash)
		{
			if(document.getElementById("selected") === null){
				$(this).children().attr("id","selected").css({"color":"#000","background":"#FFF"});
			};

			$("title").text($("#selected").text()+"-Freddie's Personal Website");
			
			return false;
			
		};

	});

	if(document.getElementById("selected") === null){

			location.hash = "Home";
	}else{
		if($("._content").is(':hidden')){
			_get($("#selected").text());
		}
		else{
			$("._content").fadeOut(500,function(){
			_get($("#selected").text());
	
			});
		};
	};
};

function _hashChanged(){

	$(window).on('hashchange', function() {

		_mark();
	});

	if(document.getElementById("selected") === null){
		_mark();
		if(location.hash !== "#Home"){
			document.getElementById("homeVideo").pause();
		};
	};


};


function _keys(){
		
	var sel;
	var timer;
	var kr = true;
	var kl = true;
	
	$(document).keydown(function(e){
		if((e.which === 37)||(e.which === 39))
		{
			sel = document.getElementById("selected");
			sel.removeAttribute("id");
			sel = $(sel).css({"background-color":"#2A2A2A","color":"#FFF"}).parent();
		
			switch(e.which) {
	 			   case 37:
							if(sel.prev().text().length > 0)
							{
									sel = sel.prev();
									sel.children().css({"color":"#000","background":"#FFF"});
								
				
							}else{
									sel.parent().attr("id","selected");
									sel = $("#selected li").last();
									document.getElementById("selected").removeAttribute("id");
									sel.children().css({"color":"#000","background":"#FFF"});
								
							}
							clearTimeout(timer);
							kl = false;
						break;
	 			   case 39:
							if(sel.next().text().length > 0){
									
									sel = sel.next();
									sel.children().css({"color":"#000","background":"#FFF"});	
							}else{
					
									sel.parent().attr("id","selected");
									sel = $("#selected li").first();
									document.getElementById("selected").removeAttribute("id");
									sel.children().css({"color":"#000","background":"#FFF"});
								
							};
							clearTimeout(timer);
							kr = false;
						break;
			};
	
			sel.children().attr("id","selected");
		};
	}).keyup(function(ev){
		
		
	switch(ev.which) {
 			   case 37:
						kl = true;
        				break;
 			   case 39:

						kr = true;
        				break;
		};
	



		if(((ev.which === 37)||(ev.which === 39))&&(kr&&kl))
		{
			timer = setTimeout(function(){
			location.hash = $("#selected").attr("href");

			},500);
		};	
		
		});
};

function _clicks(){

	$("#sidebar1 nav ul li a").on("click",function(e){
				
				$("#selected").css({"background-color":"#2A2A2A","color":"#FFF"});
				document.getElementById("selected").removeAttribute("id");
				$(this).css({"color":"#000","background":"#FFF"}).attr("id","selected");


	  });
};

$(document).ready(function() {
	$("#container").hide();
	$("#About").hide();
	$("#Photo").hide();
	$("#Video").hide();
	$("#Music").hide();
	$("#Comments").hide();
	$("#Contact").hide();

	$(".spotifySideBar nav ul li").hover(function(){

		$(this).children().css({

			"color" : "#000",

		});

		$(this).css({

			"background-color":"#eb5d25",

		});

	}, function(){

		if($(this).hasClass("spotify_selected") === false){

			$(this).children().css({

				"color" : "#FFF",

			});

			$(this).css({

				"background-color":"#3f3939",

			});
		};

	});
	
	$("#sidebar1 nav a").hover(function(){
					$(this).css({"background-color":"#FFF","color":"#000"});
    					
				             },function(){
								if($(this).attr("id") !== "selected")
								{
								$(this).css({"background-color":"#2A2A2A","color":"#FFF"});
								};
	});
		$(".contains").css("min-height",$(window).height()-168);
		$(".spotifySideBar").height($(window).height()-168);
		$(".spotifyContainer").css("width", $(window).width()-270);
	
	$(window).resize(function() {
		$(".contains").css("min-height",$(window).height()-168);
		$(".spotifySideBar").height($(window).height()-168);
		$(".spotifyContainer").css("width", $(window).width()-270);
	});
	_hashChanged();

	_keys();

	_clicks();

});

