# events.publish

Publishes the event and event data to the event service bus.

## Parameters

An array where the first parameter is the name of the event, and the second one is any kind of object representing the event data.

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

## Example
```JSON
{
  "id": 1,
  "jsonrpc": "2.0",
  "method": "events.publish",
  "params": [ "name-of-event", { "any": "kind", "of": "data" } ]
}
```