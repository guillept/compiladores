using System;

public class Example {
    public static void Method(int x) {
        Console.WriteLine("an int");
    }

    public static void Method(String s){
        Console.WriteLine("a string");
    }
    
    public static void Main(){
        dynamic obj = 3;
    }

}