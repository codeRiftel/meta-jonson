using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using jonson;
using jonson.meta;
using option;
using System.Globalization;

class Init {
    private static int Main(string[] args) {
        string line;
        StringBuilder inputBuilder = new StringBuilder();
        while ((line = Console.ReadLine()) != null) {
            inputBuilder.Append(line);
            inputBuilder.Append('\n');
        }

        string input = inputBuilder.ToString();

        var usings = new List<string>();
        var mainNamespace = "jonson.autogen";
        var genFrom = false;
        var genTo = false;
        string className = null;
        foreach (var arg in args) {
            if (arg.StartsWith("gen=")) {
                var argParts = arg.Split('=');
                if (argParts.Length == 2) {
                    if (argParts[1] == "from") {
                        genFrom = true;
                    } else if (argParts[1] == "to") {
                        genTo = true;
                    }
                }
            }
            if (arg.StartsWith("namespace=")) {
                var argParts = arg.Split('=');
                if (argParts.Length == 2) {
                    mainNamespace = argParts[1];
                }
            } else if (arg.StartsWith("using=")) {
                var argParts = arg.Split('=');
                if (argParts.Length == 2) {
                    usings.Add(argParts[1]);
                }
            } else if (arg.StartsWith("class=")) {
                var argParts = arg.Split('=');
                if (argParts.Length == 2) {
                    className = argParts[1];
                }
            }
        }

        if (!genFrom && !genTo) {
            Console.WriteLine("You must specify either gen=from or gen=to");
            return -1;
        }

        var res = Jonson.Parse(input, 1024);
        if (res.IsErr()) {
            Console.WriteLine(res.AsErr());
        } else {
            if (args.Length > 0) {
                MetaRes metaRes = default(MetaRes);
                if (genTo) {
                    if (className == null) {
                        className = "ToJSONExtensions";
                    }
                    metaRes = Meta.GenerateToJSON(res.AsOk(), className, mainNamespace, usings);
                } else if (genFrom) {
                    if (className == null) {
                        className = "FromJSONExtensions";
                    }
                    metaRes = Meta.GenerateFromJSON(res.AsOk(), className, mainNamespace, usings);
                }

                if (metaRes.error != MetaError.None) {
                    Console.WriteLine($"ERROR: {metaRes.error}");
                    return -1;
                } else {
                    Console.WriteLine(metaRes.code);
                }
            }
        }

        return 0;
    }
}
