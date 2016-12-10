<div class="theRest">

<script type="text/javascript">
 $.ajax({
			url: "https://api.spotify.com/v1/users/12141381269/playlists/0lLHgTLf0wwJA8kPzx3mdI",
            type: "GET",
            cache: false,
            dataType: 'json',

            success: function(response){
						
					console.log("success");
					var result = "<ul>";
					var i = 0;
					$(response.tracks.items).each(function(){
	
						console.log(i + " " + this.track.uri.split(":")[2]);
						
						result= result + '<li><iframe src="https://embed.spotify.com/?uri=spotify%3Atrack%3A'+this.track.uri.split(":")[2]+'" width="300" height="80" frameborder="0" allowtransparency="true"></iframe>"' + "</li>";
						i++;
					});
					
					result+="</ul>";
					
					$("#display").html(result);
		
				},
            error: function(error) { alert(error.status); },
            beforeSend: function(xhr){xhr.setRequestHeader('Authorization','Bearer BQBirC5Z8ItW4O9BnYkK0JrH7gLr6xm7g_gvAkkSQc6HCHepITUBYrjfOg5_yMXomwOQxsNb5OAHzTZMHXK0djg33PoWQAMgSweAGtugXU4vnA2up-Ovz3RBc4dvWhFxHSiqwSQu_-R6k73DqZC3V9gkWHmM');}
        });



</script>

<div id = "display"></div>

</div>