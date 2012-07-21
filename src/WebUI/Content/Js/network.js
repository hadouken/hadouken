var Network =
{
    getJson: function(url, success, async)
    {
        $.ajax(
        {
            async: async,
            type: "GET",
            url: url,
            dataType: "json",
            
            success: function(data)
            {
                switch($type(success))
                {
                    case "function":
                        success(data);
                        break;
                        
                    case "array":
                        success[0].apply(success[1], new Array(data, success[2]));
                        break;
                }
            }
        });
    },
    
    getHtml: function(url, success, async)
    {
        $.ajax(
        {
            async: async,
            type: "GET",
            url: url,
            
            success: function(data)
            {
                switch($type(success))
                {
                    case "function":
                        success(data);
                        break;
                        
                    case "array":
                        success[0].apply(success[1], new Array(data, success[2]));
                        break;
                }
            }
        });
    }
};
