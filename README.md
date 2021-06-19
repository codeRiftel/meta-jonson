# meta-vjp
meta-vjp is a C# code generator for parsing and generating JSON from types using vjp.  
**NOTE:** this package depends on [Option](https://github.com/codeRiftel/option) and [vjp](https://github.com/codeRiftel/vjp).  

## Alternative
There is [vjp-reflect](https://github.com/codeRiftel/vjp-reflect) if you want to be slow and pure [vjp](https://github.com/codeRiftel/vjp) if you want to be verbose.

## How to use
Check out Makefile to figure out how to build and .exe. Basically you have to have mono installed on your system.  
After you've build meta.exe you can run it like this  
`mono meta.exe to < test.json > AutoGenTo.cs`  
If you want to generate FromJSON extensions instead of ToJSON just replace `to` with `from`.  
`mono meta.exe from < test.json > AutoGenFrom.cs`  
Just look at `test.json` to get an idea of how to make a description file.  
```javascript
{
    "__slaves": ["float", "double"],
    "Person": {
        "name": "string",
        "age": "int",
        "dumb": "bool",
        "job": "Job",
        "repos": "string[]",
        "signature": "List<int>",
        "foo": "Dictionary<string, int>",
        "number": "enum Number",
        "bar": "Bar?",
        "nint": "int?"
    },
    "Job": {
        "__is_ref": true,
        "name": "string",
        "position": "string",
        "salary": "decimal"
    },
    "Bar": {
        "foo": "string",
        "foobar": "int"
    }
}
```
For the most part it's pretty straightforward.  

## Notes
* `__slaves` array is not necessary, but it's there then it must contain only built-in types (string, numbers, List, array, Dictionary, etc.). You should use it only if you want to make serialization/deserialization of some basic types persistent.
* You have to put `"__is_ref": true"` if type is a reference type. Otherwise some null-related stuff wouldn't be generated and you might face some nasty errors.
* It's easy to generate `Dictionary<K, V>` if K is not a string, but to parse it is another story. Generally there is no way to fill some type from a string, because of that I would suggest you to use `Dictionary<K, V>` only if K is a string.
