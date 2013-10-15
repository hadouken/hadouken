# core.multiCall

Enables you to call multiple methods in a single call.

## Parameters

A dictionary of method names and their respective parameters.

```JSON
{
  "method1": <params>,
  "method2": <params>
}
```

## Returns

A dictionary which corresponds to the parameters, but with results instead of parameters as values.

```JSON
{
  "method1": <result>,
  "method2": <result>
}
```