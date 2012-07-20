/*
 *      Misc objects.
 *
 *	$Id$
 */

function $$(id)
{
	return((typeof id == 'string') ? document.getElementById(id) : id);
}

function $type(obj)
{
	return( (obj == undefined) ? false : (obj.constructor == Array) ? "array" : typeof obj );
}

function browserDetect()
{
	var ua = navigator.userAgent.toLowerCase();
	this.isGecko = (ua.indexOf("gecko") !=- 1 && ua.indexOf("safari") ==- 1);
	this.isAppleWebKit = (ua.indexOf("webkit") !=- 1);
	this.isKonqueror = (ua.indexOf("konqueror") !=- 1);
	this.isSafari = (ua.indexOf("safari") !=- 1);
	this.isOpera = (ua.indexOf("opera") !=- 1);
	this.isIE = (ua.indexOf("msie") !=- 1 && !this.isOpera && (ua.indexOf("webtv") ==- 1));
	this.isMozilla = (this.isGecko && ua.indexOf("gecko/") + 14 == ua.length);
	this.isFirefox = (ua.indexOf("firefox/") !=- 1);
	this.isChrome = (ua.indexOf("chrome/") !=- 1);
	this.isMidori = (ua.indexOf("midori/") !=- 1);
	this.versionMinor = parseFloat(navigator.appVersion);
	if(this.isGecko && !this.isMozilla && !this.isKonqueror)
		this.versionMinor = parseFloat(ua.substring(ua.indexOf("/", ua.indexOf("gecko/") + 6) + 1));
	else
	if(this.isMozilla)
		this.versionMinor = parseFloat(ua.substring(ua.indexOf("rv:") + 3));
	else
	if(this.isIE && this.versionMinor >= 4)
		this.versionMinor = parseFloat(ua.substring(ua.indexOf("msie ") + 5));
	else
	if(this.isKonqueror)
		this.versionMinor = parseFloat(ua.substring(ua.indexOf("konqueror/") + 10));
	else
	if(this.isSafari)
		this.versionMinor = parseFloat(ua.substring(ua.lastIndexOf("safari/") + 7));
	else
	if(this.isOpera)
		this.versionMinor = parseFloat(ua.substring(ua.indexOf("opera") + 6));
	if(this.isIE && document.documentMode)
		this.versionMajor = document.documentMode;
	else
		this.versionMajor = parseInt(this.versionMinor);

	this.mode = document.compatMode ? document.compatMode : "BackCompat";
	this.isIE7x = (this.isIE && this.versionMajor == 7);
	this.isIE7up = (this.isIE && this.versionMajor >= 7);
	this.isIE8up = (this.isIE && this.versionMajor >= 8);
	this.isFirefox3x = (this.isFirefox && this.versionMajor == 3);

	var h = document.getElementsByTagName("html")[0];
	var c = h.className;
	if(this.isIE)
		h.className = "ie" + " ie" + this.versionMajor + " " + c;
	else
	if(this.isOpera)
		h.className = ("opera " + c);
	else
	if(this.isKonqueror)
		h.className = ("konqueror " + c);
	else
	if(this.isChrome)
		h.className = ("webkit chrome " + c);
	else
	if(this.isAppleWebKit)
		h.className = ("webkit safari " + c);
	else
	if(this.isGecko)
		h.className = ("gecko " + c);
}
var browser = new browserDetect();

$(document).ready(function() 
{
	var i = document.createElement('p');
	i.style.width = '100%';
        i.style.height = '200px';
        var o = document.createElement('div');
	o.style.position = 'absolute';
	o.style.top = '0px';
	o.style.left = '0px';
	o.style.visibility = 'hidden';
	o.style.width = '200px';
	o.style.height = '150px';
	o.style.overflow = 'hidden';
	o.appendChild(i);
	document.body.appendChild(o);
	var w1 = i.offsetWidth;
	var h1 = i.offsetHeight;
	o.style.overflow = 'scroll';
	var w2 = i.offsetWidth;
	var h2 = i.offsetHeight;
	if (w1 == w2) w2 = o.clientWidth;
	if (h1 == h2) h2 = o.clientWidth;
	document.body.removeChild(o);
	window.scrollbarWidth = w1-w2;
	window.scrollbarHeight = h1-h2;
});

if(browser.isKonqueror)
{
	$.fn.inheritedval = $.fn.val;
	$.fn.val = function( value )
	{
		if( this.length && $.nodeName( this[0], "textarea" ) && (value !== undefined))
		{
			var tarea = this[0];
			if(tarea.lastChild)
				tarea.removeChild(tarea.lastChild);
			tarea.appendChild(document.createTextNode(value)); 
			return(this);
		}
		else
			return($.fn.inheritedval.call(this,value));
	};
	$.fn.show = function(speed,callback)
	{
		return(this.each(function()
		{
			this.style.display = "block";
		}));
	};
	$.fn.hide = function(speed,callback)
	{
		return(this.each(function()
		{
			this.style.display = "none";
		}).end());
	};
}

$.event.inheritedfix = $.event.fix;
$.event.fix = function(e)
{
	e = $.event.inheritedfix(e);
	e.fromTextCtrl = (e.target && e.target.tagName && (/^input|textarea|a$/i).test(e.target.tagName));
	e.metaKey = e.ctrlKey;
	return(e);
}
$.fn.extend({
	mouseclick: function( handler )
	{
		var contextMenuPresent = ("oncontextmenu" in document.createElement("foo")) || browser.isFirefox || $.support.touchable;
	        return( this.each( function()
	        {
	        	if($type(handler)=="function")
	        	{
				if(contextMenuPresent)
				{
					$(this).bind( "contextmenu", function(e)
					{
						e.which = 3;
						e.button = 2;
						e.metaKey = false;	// for safari
						e.shiftKey = false;	// for safari
                                                return(handler.apply(this,arguments));
					});
                                        $(this).mousedown(function(e)
					{
						if(e.which != 3)
							return(handler.apply(this,arguments));
					});
				}
				else
				if(browser.isOpera)
				{
			        	$(this).mousedown(function(e)
					{
						if(e.which==3)
						{
							if(e.target)
							{
								var c = $(this).data("btn");
								if(c)
									c.remove();
								c = $("<input type=button>").css(
								{
									"z-index": 10000, position: "absolute",
									top: (e.clientY - 2), left: (e.clientX - 2),
									width: 5, height: 5,
									opacity: 0.01
								});
								$(document.body).append(c);
								$(this).data("btn",c);
							}
						}
						return(handler.apply(this,arguments));
					});
					$(this).mouseup(function(e)
					{
						var c = $(this).data("btn");
						if(c)
						{
							c.remove();
							$(this).data("btn",null);
							if((e.which==3) &&! (/^input|textarea|a$/i).test(e.target.tagName))
								return(false);
						}
					});
				}
				else
					$(this).mousedown( handler );
			}
			else
			{
				if(contextMenuPresent)
					$(this).unbind( "contextmenu" );
				else
				if(browser.isOpera)
					$(this).unbind( "mouseup" );
				$(this).unbind( "mousedown" );
			}
		}));            	
	},

	enableSysMenu: function()
	{
		return(this.bind("contextmenu",function(e) { e.stopImmediatePropagation(); }).
			bind("selectstart",function(e) { e.stopImmediatePropagation(); return(true); }));
	}
});

function addslashes(str) 
{
	return( (str + '').replace(/[\\"']/g, '\\$&').replace(/\u0000/g, '\\0').replace(/\u000A/g, '\\n').replace(/\u000D/g, '\\r') );
}

function iv(val) 
{
	var v = (val==null) ? 0 : parseInt(val + "");
	return(isNaN(v) ? 0 : v);
}

function ir(val) 
{
	var v = (val==null) ? 0 : parseFloat(val + "");
	return(isNaN(v) ? 0 : v);
}

function linked(obj, _33, lst) 
{
	var tn = obj.tagName.toLowerCase();
	if((tn == "input") && (obj.type == "checkbox")) 
		var d = _33 ? obj.checked : !obj.checked;
	else 
		if(tn == "select") 
		{
			var v = obj.options[obj.selectedIndex].value;
			var d = (v == _33) ? true : false;
		}
	for(var i = 0; i < lst.length; i++) 
	{
		var o = $$(lst[i]);
		if(o) 
		{
			o.disabled = d;
			o = $$("lbl_" + lst[i]);
			if(o) 
				o.className = (d) ? "disabled" : "";
		}
   	}
}

function escapeHTML(str)
{
//	return( $("<div>").text(str).html() );
	return( String(str).split('&').join('&amp;').split('<').join('&lt;').split('>').join('&gt;') );
}

function getCSSRule( selectorText )
{
	function getRulesArray(i)
	{
		var crossrule = null;
		try {
		if(document.styleSheets[i].cssRules)
			crossrule=document.styleSheets[i].cssRules;
		else 
			if(document.styleSheets[i].rules)
				crossrule=document.styleSheets[i].rules;
		} catch(e) {}
		return(crossrule);
	}

	var selectorText1 = selectorText.toLowerCase();
	var selectorText2 = selectorText1.replace('.','\\.');
	var ret = null;
	for( var j=document.styleSheets.length-1; j>=0; j-- )
	{
		var rules = getRulesArray(j);
		for( var i=0; rules && i<rules.length; i++ )
		{
			if(rules[i].selectorText)
			{
				var lo = rules[i].selectorText.toLowerCase();
				if((lo==selectorText1) || (lo==selectorText2))
				{
					ret = rules[i];
					break;
				}			
			}
		}
	}
	return(ret);
}

function RGBackground( selector )
{
        this.channels = [0,0,0];
        if(selector)
        {
		var cs;
                var rule = getCSSRule(selector);
		if(rule)
			var cs = rule.style.backgroundColor;
		else
			cs = selector;	
		if(cs.charAt(0) == '#')
       			cs = cs.substr(1);
		cs = cs.replace(/ /g,'').toLowerCase();
		var colorDefs =
		[
       			{
				re: /^rgb\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)$/,
       				process: function(bits)
	        		{
					return([iv(bits[1]),iv(bits[2]),iv(bits[3])]);
				}
			},
			{
				re: /^(\w{2})(\w{2})(\w{2})$/,
				process: function(bits)
				{
		        	        return([parseInt(bits[1], 16),parseInt(bits[2], 16),parseInt(bits[3], 16)]);
				}
			},
			{
				re: /^(\w{1})(\w{1})(\w{1})$/,
				process: function (bits)
				{
					return([parseInt(bits[1] + bits[1], 16),parseInt(bits[2] + bits[2], 16),parseInt(bits[3] + bits[3], 16)]);
				}
			}
		];
		for(var i = 0; i < colorDefs.length; i++)
		{
			var bits = colorDefs[i].re.exec(cs);
			if(bits)
			{
				this.channels = colorDefs[i].process(bits);
				break;
			}
		}
	}
	return(this);
}

RGBackground.prototype.getColor = function()
{
	var r = this.channels[0].toString(16);
        var g = this.channels[1].toString(16);
        var b = this.channels[2].toString(16);
        if(r.length == 1) r = '0' + r;
        if(g.length == 1) g = '0' + g;
        if(b.length == 1) b = '0' + b;
        return('#' + r + g + b);
	return(this);
}

RGBackground.prototype.setGradient = function(beginColor,endColor,percent)
{
	this.channels[0] = beginColor.channels[0] + iv(percent * (endColor.channels[0] - beginColor.channels[0]) / 100);
	this.channels[1] = beginColor.channels[1] + iv(percent * (endColor.channels[1] - beginColor.channels[1]) / 100);
	this.channels[2] = beginColor.channels[2] + iv(percent * (endColor.channels[2] - beginColor.channels[2]) / 100);
	return(this);
}

function getCRC( str, crc )
{
	var crc16Tab = new Array(
		0x0000,0x1021,0x2042,0x3063,0x4084,0x50A5,0x60C6,0x70E7,0x8108,0x9129,0xA14A,0xB16B,0xC18C,
		0xD1AD,0xE1CE,0xF1EF,0x1231,0x0210,0x3273,0x2252,0x52B5,0x4294,0x72F7,0x62D6,0x9339,0x8318,
		0xB37B,0xA35A,0xD3BD,0xC39C,0xF3FF,0xE3DE,0x2462,0x3443,0x0420,0x1401,0x64E6,0x74C7,0x44A4,
		0x5485,0xA56A,0xB54B,0x8528,0x9509,0xE5EE,0xF5CF,0xC5AC,0xD58D,0x3653,0x2672,0x1611,0x0630,
		0x76D7,0x66F6,0x5695,0x46B4,0xB75B,0xA77A,0x9719,0x8738,0xF7DF,0xE7FE,0xD79D,0xC7BC,0x48C4,
		0x58E5,0x6886,0x78A7,0x0840,0x1861,0x2802,0x3823,0xC9CC,0xD9ED,0xE98E,0xF9AF,0x8948,0x9969,
		0xA90A,0xB92B,0x5AF5,0x4AD4,0x7AB7,0x6A96,0x1A71,0x0A50,0x3A33,0x2A12,0xDBFD,0xCBDC,0xFBBF,
		0xEB9E,0x9B79,0x8B58,0xBB3B,0xAB1A,0x6CA6,0x7C87,0x4CE4,0x5CC5,0x2C22,0x3C03,0x0C60,0x1C41,
		0xEDAE,0xFD8F,0xCDEC,0xDDCD,0xAD2A,0xBD0B,0x8D68,0x9D49,0x7E97,0x6EB6,0x5ED5,0x4EF4,0x3E13,
		0x2E32,0x1E51,0x0E70,0xFF9F,0xEFBE,0xDFDD,0xCFFC,0xBF1B,0xAF3A,0x9F59,0x8F78,0x9188,0x81A9,
		0xB1CA,0xA1EB,0xD10C,0xC12D,0xF14E,0xE16F,0x1080,0x00A1,0x30C2,0x20E3,0x5004,0x4025,0x7046,
		0x6067,0x83B9,0x9398,0xA3FB,0xB3DA,0xC33D,0xD31C,0xE37F,0xF35E,0x02B1,0x1290,0x22F3,0x32D2,
		0x4235,0x5214,0x6277,0x7256,0xB5EA,0xA5CB,0x95A8,0x8589,0xF56E,0xE54F,0xD52C,0xC50D,0x34E2,
		0x24C3,0x14A0,0x0481,0x7466,0x6447,0x5424,0x4405,0xA7DB,0xB7FA,0x8799,0x97B8,0xE75F,0xF77E,
		0xC71D,0xD73C,0x26D3,0x36F2,0x0691,0x16B0,0x6657,0x7676,0x4615,0x5634,0xD94C,0xC96D,0xF90E,
		0xE92F,0x99C8,0x89E9,0xB98A,0xA9AB,0x5844,0x4865,0x7806,0x6827,0x18C0,0x08E1,0x3882,0x28A3,
		0xCB7D,0xDB5C,0xEB3F,0xFB1E,0x8BF9,0x9BD8,0xABBB,0xBB9A,0x4A75,0x5A54,0x6A37,0x7A16,0x0AF1,
		0x1AD0,0x2AB3,0x3A92,0xFD2E,0xED0F,0xDD6C,0xCD4D,0xBDAA,0xAD8B,0x9DE8,0x8DC9,0x7C26,0x6C07,
		0x5C64,0x4C45,0x3CA2,0x2C83,0x1CE0,0x0CC1,0xEF1F,0xFF3E,0xCF5D,0xDF7C,0xAF9B,0xBFBA,0x8FD9,
		0x9FF8,0x6E17,0x7E36,0x4E55,0x5E74,0x2E93,0x3EB2,0x0ED1,0x1EF0);

       	crc = iv(crc);
	for(var i=0; i<str.length; i++)
		crc = crc16Tab[((crc>>8)^str.charCodeAt(i))&0xFF]^((crc<<8)&0xFFFF);
	return(crc);
}

function json_encode(obj)
{
	switch($type(obj))
	{
		case "number":
			return(String(obj));
		case "boolean":
			return(obj ? "1" : "0");
		case "string":
			return('"'+addslashes(obj)+'"');
		case "array":
		{
		        var s = '';
		        $.each(obj,function(key,item)
		        {
		                if(s.length)
                			s+=",";
		        	s += json_encode(item);
		        });
			return("["+s+"]");
		}
		case "object":
		{
		        var s = '';
		        $.each(obj,function(key,item)
		        {
		                if(s.length)
                			s+=",";
		        	s += ('"'+key+'":'+json_encode(item));
		        });
			return("{"+s+"}");
		}
	}
	return("null");
}

var Timer = function()
{
        this.initial = 0;
        this.interval = 0;
};

Timer.prototype.start = function()
{
        this.initial = (new Date()).getTime();
};

Timer.prototype.stop = function()
{
        this.interval = (new Date()).getTime() - this.initial;
};

