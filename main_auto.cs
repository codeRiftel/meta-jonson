using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using vjp;
using vjp.autogen;
using option;
using System.Globalization;

public struct Person {
    public string name;
    public int age;
    public bool dumb;
    public Job job;
    public string[] repos;
    public List<int> signature;
    public Dictionary<string, int> foo;
    public Number number;
}

public enum Number {
    Zero,
    One,
    Two,
    Three,
}

public class Job {
    public string name;
    public string position;
    public decimal salary;
}

class Init {
    private static int Main(string[] args) {
        var person = new Person();
        person.name = "foo";
        person.age = 42;
        person.dumb = true;
        person.job = new Job {
            name = "janitor",
            position = "head",
            salary = 42
        };
        person.repos = new string[] { "bar1", "bar2" };
        person.signature = new List<int>() { -42, 0, 42 };
        person.foo = new Dictionary<string, int>();
        person.foo["foo1"] = 1;
        person.foo["foo2"] = 2;

        person.number = Number.Two;

        string output = VJP.Generate(person.ToJSON());
        Console.WriteLine(output);

        return 0;
    }
}