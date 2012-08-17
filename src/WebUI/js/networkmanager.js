var NetworkManager =
{
    "getHtml": function(url, callback, options)
    {
        var defOptions =
        {
            async: false
        };
        
        new Request({
            method: "get",
            async: options.async,
            url: url,
            
            onComplete: function(response)
            {
                callback(response);
            }
        }).send();
    }
};