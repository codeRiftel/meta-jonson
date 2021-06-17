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
        string line;
        StringBuilder inputBuilder = new StringBuilder();
        while ((line = Console.ReadLine()) != null) {
            inputBuilder.Append(line);
            inputBuilder.Append('\n');
        }

        string input = inputBuilder.ToString();

        var res = VJP.Parse(input, 1024);
        if (res.IsErr()) {
            Console.WriteLine(res.AsErr());
        } else {
            var metaRes = Meta.GenerateToJSON(res.AsOk());
            if (metaRes.error != MetaError.None) {
                Console.WriteLine($"ERROR: {metaRes.error}");
            } else {
                Console.WriteLine(metaRes.code);
            }
        }

        return 0;
    }
}
