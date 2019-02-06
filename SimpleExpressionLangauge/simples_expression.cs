using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenCategory {
	INT, PLUS, TIMES, PAR_OPEN, PAR_CLOSE, EOF, BAD_TOKEN
}

public class Token {
	public TokenCategory Category {get;}
	public String Lexeme {get; }
	public Token(TokenCategory category, String lexeme) {
		Category = category; //assign category
		Lexeme = lexeme;
	}
	public override String ToString() {
		return $"[{Category}, {Lexeme}]";
	}
}

public class Scanner //Lexical Analysis
{
    readonly String input;
    static readonly Regex regex = new Regex(
        @"(\d+)|([+])|([*])|([(])|([)])|(\s)|(.)"
    ); //; (int, \+, \*, \(, \) ) or espacios or anything 

    public Scanner(String input) {
        this.input = input;
    }
    public IEnumerable<Token> Start() {
        foreach (Match m in regex.Matches(input)){
             //check if it is group n of the regex. 0 is the hole expresion
            if(m.Groups[1].Success) {
                yield return new Token(TokenCategory.INT, m.Value);
            } else if (m.Groups[2].Success) {
                yield return new Token(TokenCategory.PLUS, m.Value);
            } else if (m.Groups[3].Success) {
                yield return new Token(TokenCategory.TIMES, m.Value);
            } else if (m.Groups[4].Success) {
                yield return new Token(TokenCategory.PAR_OPEN, m.Value);
            } else if (m.Groups[5].Success) {
                yield return new Token(TokenCategory.PAR_CLOSE, m.Value);
            } else if (m.Groups[6].Success) { //ignore space, represent nothing
                continue;
            } else if (m.Groups[7].Success) {
                yield return new Token(TokenCategory.BAD_TOKEN, m.Value); //la cagó
            } 
        };
    }}
public class Driver { 
    public static void Main() {
        Console.Write("> ");
        var line = Console.ReadLine();
        foreach (var token in new Scanner(line).Start()) {
            Console.WriteLine(token);
        }
    }

}
