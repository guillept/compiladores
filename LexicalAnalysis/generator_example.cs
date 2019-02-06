using System;
using System.Collections.Generic;

public class GeneratorExample {

    public static IEnumerable<int> Start() {
        var c = 1;
        while (c < 10000){
            yield return c; //return but allows to come back
            c *= 2;
        }
    }

    public static void Main(String[] args) {
        var e = Start().GetEnumerator();
        /* e.MoveNext(); //move value to the first yield
        Console.WriteLine(e.Current); //print current iterator
        e.MoveNext();
        Console.WriteLine(e.Current);
        e.MoveNext();
        Console.WriteLine(e.Current);  */
        while(e.MoveNext()) {
            Console.WriteLine(e.Current);
        }
    }

}