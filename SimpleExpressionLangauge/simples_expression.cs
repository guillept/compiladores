/*
    LL(1) Grammar for the simple expression Language
    Prog    ::= Exp "EOF"
    Exp     ::= Term ("+" Term)*
    Term    ::= Fact ("*" Fact)*
    Pow     ::= Fact ("^" Fact)*
    Fact    ::= int|(Exp)
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenCategory {
	INT, PLUS, TIMES, POW, PAR_OPEN, PAR_CLOSE, EOF, BAD_TOKEN
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
        @"(\d+)|([+])|([*])|([(])|([)])|(\s)|(\^)|(.)"
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
                yield return new Token(TokenCategory.POW, m.Value);
            } else if (m.Groups[8].Success) {
                yield return new Token(TokenCategory.BAD_TOKEN, m.Value); //la cagó
            } 
        }
        yield return new Token(TokenCategory.EOF, null); //EOF
    }
}

 public class SyntaxError: Exception {}

public class Parser {
    IEnumerator<Token> tokenStream;
    public Parser(IEnumerator<Token> token){
        this.tokenStream = token;
        this.tokenStream.MoveNext();
    }

    public TokenCategory Current {
        get { return tokenStream.Current.Category; }
    }

    public Token Expect(TokenCategory category) {
        if ( Current == category ){
            Token current = tokenStream.Current; //current token
            tokenStream.MoveNext(); //move to next token
            return current;
        } else {
            //Syntax Error
            throw new SyntaxError();
        }
    }

    //Every production gets translated into a method
    public int Prog() {
        var result = Exp();
        Expect(TokenCategory.EOF);
        return result;
    }
    public int Exp() {
        var result = Term();
        while(Current == TokenCategory.PLUS) {
            Expect(TokenCategory.PLUS); // consume +
            result += Term();
        }
        return result;
    }
    public int Term() {
        var result = Pow();
        while(Current == TokenCategory.TIMES) {
            Expect(TokenCategory.TIMES);
            result *= Pow();
        }
        return result;

    }
    public int Pow() {
        var result = Fact();
        while(Current == TokenCategory.POW) {
            Expect(TokenCategory.POW);
            result = (int) Math.Pow(result, Pow()); //right recursion
        }
        return result;
        
    }    
    public int Fact() {
            switch (Current){
                case TokenCategory.INT:
                    var token = Expect(TokenCategory.INT);
                    return Convert.ToInt32(token.Lexeme);
                case TokenCategory.PAR_OPEN:
                    Expect(TokenCategory.PAR_OPEN);
                    var result = Exp();
                    Expect(TokenCategory.PAR_CLOSE);
                    return result;
                default: //syntax error
                    throw new SyntaxError();
            }
        }
}
public class Driver { 
    public static void Main() {
        Console.Write("> ");
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            var result = parser.Prog();
            Console.Write(result);
        } catch (SyntaxError) {
            Console.WriteLine("Valio verga su syntax del mameitor");
        }
        /* foreach (var token in new Scanner(line).Start()) {
            Console.WriteLine(token);
        }
        */
    }

}
