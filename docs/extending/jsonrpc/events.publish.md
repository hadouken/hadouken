# events.publish

Publishes the event and event data to the event service bus.

## Parameters

An array where the first parameter is the name of the event, and the second one is any kind of JSON object representing the event data (strings, arrays, objects).

```JSON
[ <string>, <object> ]
```

## Returns

A dictionary which corresponds to the parameters, but with results instead of parameters as values.

```JSON
{
  "method1": <result>,
  "method2": <result>
}
```

## Example 1
```JSON
{
  "id": 1,
  "jsonrpc": "2.0",
  "method": "events.publish",
  "params": [ "name-of-event", { "any": "kind", "of": "data" } ]
}
```

## Example 2

```JSON
{
  "id": 1,
  "jsonrpc": "2.0",
  "method": "events.publish",
  "params": [ "name-of-event", [ 1, 2, 3 ] ]
}
```