using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using vjp;
using vjp.meta;
using option;
using System.Globalization;

class Init {
    private static int Main(string[] args) {
        /*
        var person = new Person();
        person.name = "foo";
        person.age = 42;
        person.dumb = true;
        person.credentials = null;
        person.repos = new string[] { "bar1", "bar2" };

        string output = VJP.Generate(person.ToJSON());
        Console.WriteLine(output);
        */

        // string result = Meta.Generate(JSONType.Make());
        // Console.Write(result);

        string line;
        StringBuilder inputBuilder = new StringBuilder();
        while ((line = Console.ReadLine()) != null) {
            inputBuilder.Append(line);
            inputBuilder.Append('\n');
        }

        string input = inputBuilder.ToString();

        // Console.Write(input);

        var res = VJP.Parse(input, 1024);
        if (res.IsErr()) {
            Console.WriteLine(res.AsErr());
        } else {
            var metaRes = Meta.Generate(res.AsOk());
            if (metaRes.error != MetaError.None) {
                Console.WriteLine($"ERROR: {metaRes.error}");
            } else {
                Console.WriteLine(metaRes.code);
            }
        }

        return 0;
    }
}
