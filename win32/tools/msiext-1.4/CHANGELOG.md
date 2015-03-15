###  1.4.1114.0 (11/14/2013)

  * **Features**:
    * Upgraded to Wix 3.7 - [@thcristo](https://github.com/thcristo).
  * **Bugs**: 
    * Bug: `TcpIp_ListAddresses` fails for computer names with exactly 15 characters (max length) - [@dblock](https://github.com/dblock).
  * **Dev**:
    * Project requires Visual Studio 2010 to build - [@thcristo](https://github.com/thcristo).
    * Upgraded Boost, CryptLib and CppUnit - [@thcristo](https://github.com/thcristo).

###  1.3.529.0 (5/29/2012)

  * **Features**: 
    * First release off Github.
    * Upgraded to Wix 3.5.
    * Added `Xml_DeleteNodes`.
    * Added User Privileges `GetAccountSid`.
    * Added User Privileges `TranslateNameFormatted`.
  * **Bugs**: 
    * Bug: `ODBCExecuteBinary` fails to load the binary record when the binary ID contains a . (dot) or - (dash).
    * Bug: ODBC custom actions, extensions and ODBQ miss the last character of the script output when its length equals the internal buffer size.
    * Bug: ODBC custom actions, extensions and ODBQ get confused by SQL diag messages that contain brackets.

###  1.2 (11/09/2010)

  * **Features**: 
    * Added `Type` option to `ODBCExecute` and `ODBCExecuteFile` Wix extensions. Specify `SqlServer` or `Oracle` to invoke a database-specific SQL parser.
    * Added support for SQL commands (SQL server `:On Error`, `:r`, Oracle `@@`, etc.) to `ODBCExecute` and `ODBCExecuteFile` Wix extensions and `ODBC_Execute` immediate custom action.
    * Added `BasePath` option and support for SQL file inclusions (`:r`) to `ODBCExecute` and `ODBCExecuteFile` Wix extensions.
    * Added `ODBC_SQL_DELIMITER` and `ODBC_SQL_TYPE` to input parameters of `ODBC_Execute` immediate custom action.
    * Added `ODBC_Execute_Binary` immediate custom action.
    * Added `ODBC_SQL_MESSAGES` containing out-of-band SQL status messages to output parameters of `ODBC_Execute` and `ODBC_Execute_Binary` immediate custom actions.
    * Added `--flattenonly` to `ODBQ.exe`, process input without executing.
    * Added `--rawoutput` to `ODBQ.exe`, output results in 'raw' format.
    * Added `--database` to `ODBQ.exe`, default database name.
    * Added short versions of `-f (--file)` and `-o (--outputfile)` to `ODBQ.exe`.
    * Added `XsltFilename` to `ODBCExecute`, `ODBCExecuteFile` and `ODBCExecuteBinary` that specifies an optional XSLT transform for the data.
    * Added `GenerateRandomString` custom action to Cryptography.dll.
    * Added `Win32_GenerateGuidString` and `Win32_IsValidGuid` custom actions to SystemTools.dll.
    * Added `Win32_ReadFile` and `Win32_WriteFile` custom actions to SystemTools.dll with UTF-8 support.
    * Added `Service_Exists` custom action to SystemTools.dll.
    * Added `String_LTrim` and `String_RTrim` to StringTools.dll.
    * Added UTF-8 encoded file support for ODBC custom actions, Wix extensions and ODBQ.exe.
    * Added UTF-8 encoded file support to `SystemTools::TemplateFile` Wix extension.
    * Added `Win32_GetSpecialFolderPath` custom action to SystemTols.dll.
    * `DESEDE3_Encrypt` and `DESEDE3_Decrypt` now support binary data.
    * Added `Base64EncodeA`, `Base64EncodeW`, `Base64DecodeA` and `Base64DecodeW` to Cryptography.dll.
    * Added support for `sqlvariant` in ODBC custom actions, Wix extensions and ODBQ.exe.
  * **Breaking Changes**: 
    * `DESEDE3_Encrypt` and `DESEDE3_Decrypt` custom actions now return empty results for empty inputs and use PKCS7 padding for encrypted values. Use a previous MSI Extensions 1.1 to decrypt and upgrade existing values.
    * `DESEDE3_Encrypt` and `DESEDE3_Decrypt` custom actions will fail if passed a key larger than 24 bytes. Previous versions silently truncated the key.
    * ODBC custom actions, extensions and ODBQ will no longer fail processing empty files. A warning will be produced instead.
  * **Misc**: 
    * Build and unit tests run on Windows 7, `MSIShim` adds a dummy `Property` table with a value for `ProductCode`.
    * Connection strings in custom action data passed between immediate and deferred ODBC custom actions is now encrypted so that credentials are not logged in clear in MSI logs.
  * **Bugs**: 
    * Bug: `XmlTools_XslTransform` broken for output of type text.
    * Bug: `CreateDatabases_SQLServer_Deferred` and `CreateDatabases_Access_Deferred` attempt to make a database connection even if there's nothing to do.
    * Bug: error detail lost in custom actions that rely on functions returning Win32 DWORD error codes.
    * Bug: SystemTools extensions incorrectly impersonate for file copy, move, delete, template processing and registry operations.
    * Bug: ODBC custom actions, extensions and ODBQ may fail to execute a statement at `SQLDescribeParam`.
    * Bug: ODBC custom actions, extensions and ODBQ don't work on SQL Server 2000.
    * Bug: `Win32_DirectoryCreate` works inconsistently when paths contain forward slashes depending on whether a path exists or not.
    * Bug: ODBC messages are truncated at 208 characters.
    * Bug: `SelectDbDlg` dialog mail fail when `DATABASE_SERVER` is re-defined with a custom action with "Error 2878: On the dialog SelectDbDlg the control Server has a possible value: [1]. This is an invalid or duplicate value."

###  1.1 (11/20/2009)

  * First open-source public release.

