using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using jonson;
using jonson.autogen.to;
using jonson.autogen.from;
using option;
using System.Globalization;

class Init {
    private static int Main(string[] args) {
        var personText = File.ReadAllText("person.json");
        var personJSONRes = Jonson.Parse(personText, 1024);
        if (personJSONRes.IsErr()) {
            Console.WriteLine(personJSONRes.AsErr());
        } else {
            var personJSON = personJSONRes.AsOk();
            var person = new Person();
            personJSON.FromJSON(ref person);
            Console.WriteLine(Jonson.Generate(person.ToJSON()));
        }

        return 0;
    }
}
