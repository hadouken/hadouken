# core.setAuth

Sets the authentication (ie. username and password). If no auth is set, the third parameter is ignored, otherwise it should be set to the current password.

When successfully changing authentication, the event [`auth.changed`](../events/auth.changed.md) is sent.

## Parameters

An array with the elements `username`, `newPassword` and `oldPassword`.

```JSON
[ <string>, <string>, <string> ]
```

## Returns

True or false depending on if the authentication was successfully changed.

```JSON
<boolean>
```