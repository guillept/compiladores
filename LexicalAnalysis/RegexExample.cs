using System;
using System.IO;
using System.Text.RegularExpressions;

public class RgexExample {
    public static void Main(String[] args) {
        var regex = new Regex(@"([/][*].*?[*][/])|(.)", RegexOptions.Singleline); //@/*any character 0 or + times */
        var text = File.ReadAllText("hello.c");
        foreach(Match m in regex.Matches(text)) {
            if(m.Groups[2].Success) {
                Console.Write(m.Value); //Group 2 = (.)
            }
        }
        // Console.WriteLine("Hello, World");
    }
}