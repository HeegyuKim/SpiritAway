
function ProtectTextAreaTab(e){
	if(e.keyCode === 9) {
		var start = this.selectionStart,
			end = this.selectionEnd;
		
		var $this = $(this);
		var value = $this.val();
		
		$this.val(value.substring(0, start)
				+ "\t"
				+ value.substring(end));
		
		this.selectionStart = start + 1;
		this.selectionEnd = end + 1;
		e.preventDefault();
	}
}

window.addEventListener('load', function() {
	
	var source = {}
	var editor = $('#IEditor')
	var size = {
		width: 800,
		height: 600
	}
	var buttons = {
		view: document.getElementById('IBtnView'),
		source: document.getElementById('IBtnSource')
	}
	var bg;
	
	var InitView = function()
	{
		if(source.background) {
			bg = $("<img/>")
			bg.attr("src", source.background)
			bg.click(function(e){

			      var offset = $(this).offset();
			      var relativeX = (e.pageX - offset.left);
			      var relativeY = (e.pageY - offset.top);

			      $("#IClickInfo").text(
						"X: " + relativeX + "  Y: " + relativeY
						);
			})
			
			editor.append(bg)
		}
		
		console.log(source)
		
		if(source.waypoints == null)
			return

			
		
		for(var i in source.waypoints) {
			var loc = source.waypoints[i]
			if(loc.links)
			{
				for(var j in loc.links)		
				{
					var link = loc.links[j];
					MakeLink(loc, link);
				}
				
			}
			if(loc.next)
			{
				MakeLink(loc, { id: loc.next, name: ""} );
			}
			if(loc.link1)
				MakeLink(loc, { id: loc.link1, name: ""} );
			if(loc.link2)
				MakeLink(loc, { id: loc.link2, name: ""} );
			
		}
		
		for(var i in source.waypoints)
			MakeLocations(source.waypoints[i]);
			
		
		if(source.misteries != null)
			for(var i in source.misteries)
				MakeMistery(source.misteries[i]);
		
		if(source.survivors != null)
			for(var i in source.survivors)
				ShowSurvivor(source.survivors[i]);
	}
		
	function ShowSurvivor(surv)
	{
		DrawCircle(surv.x, surv.y, surv.detect, 8, "DetectArea");
	}
	
	
	function DrawCircle(x, y, rad, border, cls)
	{
		var circle = $("<div></div>");
		circle.addClass(cls);

		circle.css({
			position: "absolute",
			left: Math.floor(x - rad - border) + "px",
			top: Math.floor(y - rad - border) + "px",
			width: rad * 2,
			height: rad * 2,
			"border-radius": rad
		});
		circle.click(function(e)
		{
	      var offset = bg.offset();
	      var relativeX = (e.pageX - offset.left);
	      var relativeY = (e.pageY - offset.top);

	      $("#IClickInfo").text(
				"X: " + relativeX + "  Y: " + relativeY
				);
		});
		
		editor.append(circle);
	}
	
	function MakeMistery(mis)
	{
		var width = 50, height = 50;
		var div = $("<div></div>")
		div.addClass("Mistery")
		if(mis.bomb)
		{
			div.addClass("Bomb");
			div.append("BOMB<br/>")
			if(mis.id)
				div.append("" + mis.id);
			if(mis.time)
				div.append("<br/>" + mis.time);
			
			var x = mis.x, y = mis.y;
			for(var i in mis.detects)
			{
				var det = mis.detects[i];
				DrawCircle(det.x, det.y, det.radius, 8, "DetectArea");
			}

			for(var i in mis.damages)
			{
				var dam = mis.damages[i];
				DrawCircle(dam.x, dam.y, dam.radius, 2, "DamageArea");
			}
		}
		
		div.css({
			left: Math.floor(mis.x - width / 2) + "px",
			top: Math.floor(mis.y - height / 2) + "px",
			width: width,
			height: height
		})
		
		editor.append(div)
	}
	
	function LinkToString(link)
	{
		if(link == null) return "";
		
		return link.id + " : " + link.name + "<br/>";
	}
	
	
	function MakeLocations(loc) {
		var width = 50, height = 50
		if(loc.type === "waypoint") {
			width = height = 35
		}
		var div = $("<div></div>")
		div.addClass("Location")
		div.addClass(loc.type);
		div.append(loc.type + "<br/>").append(loc.id)
		div.click(function(){
			
			var linkString = "";

			for(var i in loc.links)
			{
				linkString += LinkToString(loc.links[i]);
			}
			if(loc.link1)
				linkString += loc.link1 + "<br/>";
			if(loc.link2)
				linkString += loc.link2 + "<br/>";
			
			$("#IClickInfo").html (
				loc.type + " : " + loc.id + "<br/>" 
				+ linkString
			);
		});
		div.css({
			left: Math.floor(loc.x - width / 2) + "px",
			top: Math.floor(loc.y - height / 2) + "px",
			width: width,
			height: height
		})
		
		editor.append(div)
	}
	
	function DrawLine(x1, y1, x2, y2){

	    if(y1 < y2){
	        var pom = y1;
	        y1 = y2;
	        y2 = pom;
	        pom = x1;
	        x1 = x2;
	        x2 = pom;
	    }

	    var a = Math.abs(x1-x2);
	    var b = Math.abs(y1-y2);
	    var c;
	    var sx = (x1+x2)/2 ;
	    var sy = (y1+y2)/2 ;
	    var width = Math.sqrt(a*a + b*b ) ;
	    var x = sx - width/2;
	    var y = sy;

	    a = width / 2;

	    c = Math.abs(sx-x);

	    b = Math.sqrt(Math.abs(x1-x)*Math.abs(x1-x)+Math.abs(y1-y)*Math.abs(y1-y) );

	    var cosb = (b*b - a*a - c*c) / (2*a*c);
	    var rad = Math.acos(cosb);
	    var deg = (rad*180)/Math.PI

	    htmlns = "http://www.w3.org/1999/xhtml";
	    div = document.createElementNS(htmlns, "div");
	    div.setAttribute('style','border:1px solid black;width:'+width+'px;height:0px;-moz-transform:rotate('+deg+'deg);-webkit-transform:rotate('+deg+'deg);position:absolute;top:'+y+'px;left:'+x+'px;');   

	    document.getElementById("IEditor").appendChild(div);

	}
	
	function MakeLink(loc, linkId) {
		var x2 = null, y2 = null;
		/*
		if(!linkId.id)
		{
			console.log("LinkID 가 없어요..." + loc.id);
			console.log(linkId)
			return;
		}
		*/
		for(var i in source.waypoints) {
			var newLoc = source.waypoints[i];
				
			if(newLoc.id == linkId.id) {
				x2 = newLoc.x;
				y2 = newLoc.y;
			}
		}
		
		if(x2 == null || y2 == null) {
			console.log(loc.id + " 에서 " + linkId.id + "에 연결된 ID를 찾을 수 없습니다.");
			return;
		}
		
		DrawLine(loc.x, loc.y, x2, y2);
		return;
		
		var dx = x2 - loc.x;
		var dy = y2 - loc.y;
		var length = Math.sqrt(dx * dx + dy * dy);
		
		var descX = loc.x + dx / length * 100;
		var descY = loc.y + dy / length * 100 - 20;
		
		var div = $("<div></div>")
		div.addClass("Desc")
		div.text(linkId.name);
		div.css({
			position: "absolute",
			left: Math.floor(descX) + "px",
			top: Math.floor(descY) + "px",
			display: "inline-block",
			width: 100,
			height: 50
		})
		editor.append(div);
		
		/*
		var dx = x2 - loc.x,
			dy = y2 - loc.y;
		var angle = Math.atan2(dy, dx)
		var width = Math.acos(angle) * dx * 2;

		console.log(x2 + " , " + y2)
		console.log(dx + " , " + dy)
		console.log("deg: " + angle *180 / Math.PI)
		var div = $("<div></div>")
		div.addClass("Line")
		div.css({
			left: loc.x + 2,
			top: loc.y + dy / 2 + 2,
			width: width,
			height: 5,
			transform: "rotate("+angle+"rad)"
		})
		editor.append(div)
		*/
	}
	
	InitView()
	
	buttons.view.addEventListener('click', function(e){
		var input = document.getElementById("IEditorInput");
		var value = input.value;
		
		try {
			source = JSON.parse(value)
			
			editor.html("");
		}
		catch(e) {
			alert("에러가 났어요. - " + e.message)
			return;
		}
		InitView();
	})
	
	buttons.source.addEventListener('click', function(e){
		editor.html('<textarea id="IEditorInput"></textarea>');
		
		var json = JSON.stringify(source, null, "\t");
		var input = document.getElementById("IEditorInput");
		
		input.addEventListener('keydown', ProtectTextAreaTab);
		
		$(input).css({
			width: size.width + "px",
			height: size.height + "px",
		})
		input.value = json;
	})
	
	
});