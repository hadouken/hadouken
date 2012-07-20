var Converter = 
{
    round: function(num, p)
    {
        var v = Math.floor(num * Math.pow(10, p)) / Math.pow(10, p);
        var s = v + "";
        var d = s.indexOf(".");
        var n = 0;
        
        if(d >- 1)
        {
            var ind = s.length - d;
            p++;
            
            if(ind < p)
                n = p - ind;
        }
        else
        {
            if(p > 0)
            {
                n = p;
                s = s + ".";
            }
        }
        
        for(var i = 0; i < n; i++)
            s += "0";
            
        return(s);
    },
    toTime: function(tm,noRound)
    {
        if((noRound==null) && (tm >= 2419200))
            return "\u221e";
            
        var val = tm % (604800 * 52);
        var w = iv(val / 604800);
        
        val = val % 604800;
        
        var d = iv(val / 86400);
        
        val = val % 86400;
        
        var h = iv(val / 3600);
        
        val = val % 3600;
        
        var m = iv(val / 60);
        
        val = iv(val % 60);
        
        var v = 0;
        var ret = "";
        
        if(w > 0)
        {       
            ret = w + "w";
            v++;
        }
        if(d > 0)
        {
            ret += d + "d";
            v++;
        }
        if((h > 0) && (v < 2))
        {
            ret += h + "h";
            v++;
        }
        if((m > 0) && (v < 2))
        {       
            ret += m + "m";
            v++;
        }
        if(v < 2)
            ret += val + "s";
        
        return( ret.substring(0,ret.length-1) );
    },
    toFileSize: function(bt, p)
    {
        p = (p == null) ? 1 : p;
        var a = new Array("bytes", "KB", "MB", "GB", "TB", "PB");
        var ndx = 0;
        
        if(bt == 0)
        {
            ndx = 1;
        }
        else
        {
            if(bt < 1024)
            {
                bt /= 1024;
                ndx = 1;
            }
            else
            {
                while(bt >= 1024)
                {
                    bt /= 1024;
                    ndx++;
                }
            }
        }
        return(this.round(bt, p) + " " + a[ndx]);
    },
    toSpeed: function(bt)
    {
        return((bt>0) ? this.toFileSize(bt)+ "/s" : "");
    },
    toDate: function(dt,timeOnly)
    {
        if(dt>3600*24*365)
        {
            var today = new Date();
            today.setTime(dt*1000);
            
            var month = today.getMonth()+1;
            month = (month < 10) ? ("0" + month) : month;
            
            var day = today.getDate();
            day = (day < 10) ? ("0" + day) : day;
            
            var h = today.getHours();
            var m = today.getMinutes();
            var s = today.getSeconds();
            var am = "";

            if(iv(WebUI.settings["webui.timeformat"]))
            {
                if(h>12)
                {
                    h = h-12;
                    am = " PM";
                }
                else
                {
                    am = " AM";
                }
            }
            
            h = (h < 10) ? ("0" + h) : h;
            m = (m < 10) ? ("0" + m) : m;
            s = (s < 10) ? ("0" + s) : s;
            
            var tm = h+":"+m+":"+s+am;
            var dt = '';
            if(!timeOnly)
            {
                switch(iv(WebUI.settings["webui.dateformat"]))
                {
                    case 1:
                    {
                        dt = today.getFullYear()+"-"+month+"-"+day+" ";
                        break;
                    }
                    case 2:
                    {
                        dt = month+"/"+day+"/"+today.getFullYear()+" ";
                        break;
                    }
                    default:
                    {
                        dt = day+"."+month+"."+today.getFullYear()+" ";
                        break;
                    }
                }
            }
            
            return(dt+tm);
        }
        
        return('');
    }
};
