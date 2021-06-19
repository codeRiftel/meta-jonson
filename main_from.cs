using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using vjp;
using vjp.autogen.to;
using vjp.autogen.from;
using option;
using System.Globalization;

/*
public static class FromJSONExtensions {
    public static void FromJSON(this JSONType type, ref Person val) {
        if (type.Obj.IsNone()) {
            return;
        }

        Dictionary<string, JSONType> root = type.Obj.Peel();

        if (root.ContainsKey("name")) {
            root["name"].FromJSON(ref val.name);
        }

        if (root.ContainsKey("age")) {
            root["age"].FromJSON(ref val.age);
        }

        if (root.ContainsKey("dumb")) {
            root["dumb"].FromJSON(ref val.dumb);
        }

        if (root.ContainsKey("repos")) {
            root["repos"].FromJSON(ref val.repos);
        }

        if (root.ContainsKey("foo")) {
            root["foo"].FromJSON(ref val.foo);
        }

        if (root.ContainsKey("number")) {
            root["number"].FromJSON(ref val.number);
        }
    }

    public static void FromJSON(this JSONType type, ref Number val) {
        if (type.Num.IsSome()) {
            int num = default(int);
            type.FromJSON(ref num);
            val = (Number)num;
        }
    }

    public static void FromJSON(this JSONType type, ref Dictionary<string, int> val) {
        if (type.Obj.IsSome()) {
            val = new Dictionary<string, int>();

            Dictionary<string, JSONType> obj = type.Obj.Peel();
            foreach (var pair in obj) {
                int pairVal = default(int);
                pair.Value.FromJSON(ref pairVal);
                val[pair.Key] = pairVal;
            }
        }
    }

    public static void FromJSON(this JSONType type, ref string[] val) {
        if (type.Arr.IsSome()) {
            List<JSONType> list = type.Arr.Peel();
            val = new string[list.Count];

            for (int i = 0; i < list.Count; i++) {
                list[i].FromJSON(ref val[i]);
            }
        }
    }

    public static void FromJSON(this JSONType type, ref bool val) {
        if (type.Bool.IsSome()) {
            val = type.Bool.Peel();
        }
    }

    public static void FromJSON(this JSONType type, ref int val) {
        if (type.Num.IsSome()) {
            string numStr = type.Num.Peel();

            NumberStyles style = NumberStyles.AllowDecimalPoint;
            style |= NumberStyles.AllowExponent;
            style |= NumberStyles.AllowLeadingSign;

            int.TryParse(numStr, style, CultureInfo.InvariantCulture, out val);
        }
    }

    public static void FromJSON(this JSONType type, ref string val) {
        if (type.Str.IsSome()) {
            val = type.Str.Peel();
        }
    }
}
*/

class Init {
    private static int Main(string[] args) {
        var personText = File.ReadAllText("person.json");
        var personJSONRes = VJP.Parse(personText, 1024);
        if (personJSONRes.IsErr()) {
            Console.WriteLine(personJSONRes.AsErr());
        } else {
            var personJSON = personJSONRes.AsOk();
            var person = new Person();
            personJSON.FromJSON(ref person);
            Console.WriteLine(VJP.Generate(person.ToJSON()));
        }

        return 0;
    }
}
