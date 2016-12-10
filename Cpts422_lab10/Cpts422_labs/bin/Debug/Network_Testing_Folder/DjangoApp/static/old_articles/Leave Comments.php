<div>

<script type="text/javascript" src="/scripts/commentSection/commentSection.js"></script>


<div>
<input style = "width:100%" placeholder="Leave your comments " id = "startC">
</div>


<div class = "cs" align="center">
<form action="/scripts/commentSection/commentSection.py" method="post">


<input style="width:100%" type="text" name="_title" maxlength="100" placeholder="(optional title) 100 characters maximum">

<input type = "text" name = "_firstName" maxlength="100" placeholder="(optional) First Name">

<input type="text" name="_lastName" maxlength="100" placeholder="(optional) Last Name">

<input id = "email" type = "text" name="_email" maxlength="100" placeholder="Your email address (optional but it will be private)" >

<textarea autofocus type="text" name="_comments" maxlength="1000" placeholder="1000 characters maximum"></textarea> 

<input style="display:block" type="submit" value="Submit">


</form>
</div>


</div>
