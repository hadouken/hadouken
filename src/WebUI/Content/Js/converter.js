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
    toTime: function(secs)
    {
        if (secs > 63072000 || secs < 0) return "\u221E"; // secs > 2 years ~= inf. :)
        
        var div, y, w, d, h, m, s, output = "";
        
        y = Math.floor(secs / 31536000);
        div = secs % 31536000;
        w = Math.floor(div / 604800);
        div = div % 604800;
        d = Math.floor(div / 86400);
        div = div % 86400;
        h = Math.floor(div / 3600);
        div = div % 3600;
        m = Math.floor(div / 60);
        s = div % 60;
        if (y > 0) {
            output = "%dy %dw".replace(/%d/, y).replace(/%d/, w);
        } else if (w > 0) {
            output = "%dw %dd".replace(/%d/, w).replace(/%d/, d);
        } else if (d > 0) {
            output = "%dd %dh".replace(/%d/, d).replace(/%d/, h);
        } else if (h > 0) {
            output = "%dh %dm".replace(/%d/, h).replace(/%d/, m);
        } else if (m > 0) {
            output = "%dm %ds".replace(/%d/, m).replace(/%d/, s);
        } else {
            output = "%ds".replace(/%d/, s);
        }
        return output;
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
