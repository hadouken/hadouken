define ['jquery'], ($) ->
  class RpcClient
    requestId: 0

    constructor: (@url = '/jsonrpc') ->

    call: (method, callback) ->
      @callParams(method, null, callback)

    callParams: (method, params, callback) ->
      @requestId += 1

      data =
        id: @requestId
        jsonrpc: '2.0'
        method: method,
        params: params

      json = JSON.stringify(data)
      
      $.post @url, json, (response) ->
        if response.result? then callback(response.result)

  return RpcClient