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
using System.Text;
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
    public Node Prog() {
        /* 
        var n = Exp();
        n.Add(Exp());
        */
        var n = new Prog() {
            Exp() //implicit call to add
        };
        Expect(TokenCategory.EOF);
        return n;
    }
    public Node Exp() {
        var n = Term();
        while(Current == TokenCategory.PLUS) {
             var n2 = new Pow() {
                AnchorToken = Expect(TokenCategory.PLUS)
            };
            n2.Add(n);
            n2.Add(Term());
            n = n2;
        }
        return n;
    }

    //Left Tree
    public Node Term() {
        var n = Pow();
        while(Current == TokenCategory.TIMES) {
             var n2 = new Times() {
                AnchorToken = Expect(TokenCategory.TIMES)
            };
            n2.Add(n);
            n2.Add(Pow());
            n = n2;
        }
        return n;

    }

    //Right Tree
    public Node Pow() {
        var n = Fact();
        while(Current == TokenCategory.POW) {
            var n2 = new Pow() {
                AnchorToken = Expect(TokenCategory.POW)
            };
            n2.Add(n);
            n2.Add(Pow());
            n = n2;
        }
        return n;
    }    
    public Node Fact() {
            switch (Current){
                case TokenCategory.INT:
                    /*
                    var token = Expect(TokenCategory.INT);
                    var node = new Int();
                    node.AnchorToken = token;
                    return node;
                    */
                    return new Int(){
                        AnchorToken = Expect(TokenCategory.INT)
                    };
                    // return Convert.ToInt32(token.Lexeme);
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
class Node: IEnumerable<Node> {

    IList<Node> children = new List<Node>();

    public Node this[int index] {
        get {
            return children[index];
        }
    }

    public Token AnchorToken { get; set; }

    //Adds a children node
    public void Add(Node node) {
        children.Add(node);
    }

    public IEnumerator<Node> GetEnumerator() {
        return children.GetEnumerator();
    }

    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator() {
        throw new NotImplementedException();
    }

    public override string ToString() {
        return String.Format("{0} {1}", GetType().Name, AnchorToken);
    }

    public string ToStringTree() {
        var sb = new StringBuilder();
        TreeTraversal(this, "", sb);
        return sb.ToString();
    }

    static void TreeTraversal(Node node, string indent, StringBuilder sb) {
        sb.Append(indent);
        sb.Append(node);
        sb.Append('\n');
        foreach (var child in node.children) {
            TreeTraversal(child, indent + "  ", sb);
        }
    }
}

//Node Classes
public class Prog: Node { }
public class Plus: Node { }
public class Times: Node { }
public class Pow: Node { }
public class Int: Node { }

public class EvalVisitor {

    public int Visit (Prog node) {
        return Visit((dynamic) node[0]);
    }
    public int Visit (Plus node) {
        return Visit((dynamic) node[0]) + Visit((dynamic) node[1]);
    }
    public int Visit (Times node) {
        return Visit((dynamic) node[0]) * Visit((dynamic) node[1]);
    }
    public int Visit (Pow node) {
        return (int) Math.Pow(Visit((dynamic) node[0]), Visit((dynamic) node[1]));
    }
    public int Visit (Int node) {
        return Conver.ToInt32(node.AnchorToken.Lexeme);
    }
}

public class CVisitor {
    public String Visit(Prog node) {
        var a = Visit((dynamic) node[0]);
        return @"
            #include <stdio.h>
            #include <math.h>

            int main (void) {
                printf(""%i\n"", "+ a + @");
            }
            
            return 0;
            ";
    }
    public String Visit(Plus node) {
        var a = Visit((dynamic) node[0]);
        var b = Visit((dynamic) node[1]);
        return $"({a}+{b})";
    }
    public String Visit(Times node) {
        var a = Visit((dynamic) node[0]);
        var b = Visit((dynamic) node[1]);
        return $"({a}*{b})";
    }
    public String Visit(Pow node) {
        var a = Visit((dynamic) node[0]);
        var b = Visit((dynamic) node[1]);
        return $"({a},{b})";
    }
    public Stinrg Visit(Int node) {
        return node.AnchorToken.Lexeme;
    }
    
}

public class LispVisitor {
    public String Visit(Prog node){
        return @"
            (require '[clojure.math.numeric-tower : refer [expt]])" + Visit((dynamic) node[0]);

    }

    public String Visit(Plus node){
        var a = Visit((dynamic) node[0]);
        var b = Visit((dynamic) node[1]);
        return $"(+ {a} {b})";
    }
    public String Visit(Times node){
        var a = Visit((dynamic) node[0]);
        var b = Visit((dynamic) node[1]);
        return $"(* {a} {b})";
    }
    public String Visit(Pow node){
        var a = Visit((dynamic) node[0]);
        var b = Visit((dynamic) node[1]);
        return $"(expt {a} {b})";
    }
    public String Visit(Int node){
        return node.AnchorToken.Lexeme;
    }
}
public class Driver { 
    public static void Main() {
        Console.Write("> ");
        var line = Console.ReadLine();
        var parser = new Parser(new Scanner(line).Start().GetEnumerator());
        try {
            var ast = parser.Prog();
            // var result = new EvalVisitor().Visit((dynamic) ast);
            // programa en c var result = new CVisitor().Visit((dynamic) ast);
            //programa en lisp
            Console.WriteLine(result);
        } catch (SyntaxError) {
            Console.WriteLine("SyntaxErro");
        }
        
        /*try {
            var result = parser.Prog();
            Console.Write(result);
        } catch (SyntaxError) {
            Console.WriteLine("Valio verga su syntax del mameitor");
        }*/
        /* foreach (var token in new Scanner(line).Start()) {
            Console.WriteLine(token);
        }
        */
    }

}
