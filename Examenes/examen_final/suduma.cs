/*-------------------------------------------------------------------
Guillermo Pérez Trueba
A01377162
-------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Suduma {
    
    //---------------------------------------------------------------
    class SyntaxError: Exception {}
    
    //---------------------------------------------------------------
    enum Token {
        INT, MAX, DUP, SUM_OPEN, SUM_CLOSE, COMMA, ONE, TWO, THREE, ILEGAL, EOF
    }
               
    //---------------------------------------------------------------
    class Scanner {
        readonly string input;
        static readonly Regex regex = new Regex(
            @"      
               (?<Max>         [?]     )
              | (?<Dup>         [$]     )
              | (?<SumOpen>     [{]     )
              | (?<SumClose>    [}]     )
              | (?<Comma>       [,]     )
              | (?<One>           [1]     )
              | (?<Two>         [2]     )
              | (?<Three>       [3]     )
              | (?<Espacios>    \s      )
              | (?<Otro>        .       )
            ",  RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Multiline);
        static readonly IDictionary<string, Token> regexLabels =
            new Dictionary<string, Token>() {
                {"One",  Token.ONE},
                {"Two",  Token.TWO},
                {"Three",  Token.THREE},
                {"Max",   Token.MAX},
                {"Dup", Token.DUP},
                {"SumOpen",     Token.SUM_OPEN},
                {"SumClose",    Token.SUM_CLOSE},
                {"Comma",        Token.COMMA},
            };
            
        public Scanner(string input) {
            this.input = input;
        }
        public IEnumerable<Token> Start() {
            foreach (Match m in regex.Matches(input)) {
                if (m.Groups["Espacios"].Success) {
                    // Ignorar espacios.
                } else if (m.Groups["Otro"].Success) {
                    yield return Token.ILEGAL;
                } else {
                    foreach (var name in regexLabels.Keys) {
                        if (m.Groups[name].Success) {
                            yield return regexLabels[name];
                            break;
                        }
                    }
                }
            }
            yield return Token.EOF; 
        }
    }
    
    //---------------------------------------------------------------
    class Node: IEnumerable<Node> {
        IList<Node> children = new List<Node>();
        public Node this[int index] {
            get {
                return children[index];
            }
        }

        public int ChildrenSize() {
            return children.Count;
        }
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
            return GetType().Name;                                 
        }
        public string ToStringTree() {
            var sb = new StringBuilder();
            TreeTraversal(this, "", sb);
            return sb.ToString();
        }
        static void TreeTraversal(
                Node node, 
                string indent, 
                StringBuilder sb) {
            sb.Append(indent);
            sb.Append(node);
            sb.Append('\n');
            foreach (var child in node.children) {
                TreeTraversal(child, indent + "  ", sb);
            }
        }
    }
    
    //---------------------------------------------------------------
    class Programa: Node {}
    class Max:      Node {}
    class Sum:    Node {}
    class Number:   Node {}
    class Dup:    Node {}
    class ONE:    Node {}
    class TWO:    Node {}
    class THREE:    Node {}

    //---------------------------------------------------------------
    class Parser {

        IEnumerator<Token> tokenStream;
        public Parser(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }
        public Token CurrentToken {
            get { return tokenStream.Current; }
        }
        public Token Expect(Token category) {
            if (CurrentToken == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError();                
            }
        }

        public Node Inicio() {
            var prog = new Programa() {Max()};
            Expect(Token.EOF);
            return prog;
        }

        public Node Max() {
            var simple = ExpSimple();  
            while(CurrentToken == Token.MAX){
                Expect(Token.MAX);
                var max = new Max()  {simple, ExpSimple() };
                simple = max;
            }     
            return simple;      
        }

        public Node ExpSimple() {
            switch (CurrentToken)
            {
                case Token.ONE:
                    Expect(Token.ONE);
                    return new ONE();
                case Token.TWO:
                    Expect(Token.TWO);
                    return new TWO();
                case Token.THREE:
                    Expect(Token.THREE);
                    return new THREE();
                case Token.DUP:
                    Expect(Token.DUP);
                    return new Dup() {ExpSimple()};
                case Token.SUM_OPEN:
                    Expect(Token.SUM_OPEN);
                    var maxList = MaxList();
                    Expect(Token.SUM_CLOSE);
                    return maxList;
                default:
                    throw new SyntaxError();
            }
        }

        public Node MaxList() {
            var maxList = new Sum(){ Max()};
            while (CurrentToken == Token.COMMA) {
                Expect(Token.COMMA);
                maxList.Add( Max() );
            }
            return maxList;

        }

        public Node Sum() {
            var sum = new Sum();
            while(CurrentToken == Token.INT) {
                Expect(Token.INT);
                if (CurrentToken == Token.COMMA) {
                    Expect(Token.COMMA);
                }
            }

            return sum;
        }

        public Node Dup() {
            var dup = new Dup();
            Expect(Token.DUP);
            Expect(Token.INT);    

            return dup;          
        }        
    }
    
    //---------------------------------------------------------------
    class CILGenerator {
        public string Visit(Programa node) {
            return ".assembly 'suduma' {}\n\n"
                + ".class public 'final_exam' extends ['mscorlib']'System'.'Object' {\n"
                + "\t.method public static void 'start'() {\n"
                + "\t\t.entrypoint\n"
                + Visit((dynamic) node[0])
                + "\t\tcall void ['mscorlib']'System'.'Console'::'WriteLine'(int32)\n"
                + "\t\tret\n"
                + "\t}\n"
                + "}\n";
        }

        public String Visit(Max node) {
            return VisitChildren(node) + "\t\tcall int32 ['mscorlib']'System'.'Math'::'Max'(int32, int32)\n";
        }

        public String Visit(Sum node) {
            var res = "";
            for (int i = 0; i < node.ChildrenSize(); i++)
            {   
                if (i == 0) {
                    res += Visit((dynamic)node[i]);
                } else {
                    res += Visit((dynamic) node[i]) + "\t\tadd\n";   
                }
            }
            return res;
        }

        public String Visit(Dup node) {
            return VisitChildren(node) + "\t\tdup\n" + "\t\tadd\n";
        }

        public String Visit(ONE node) {
            return "\t\tldc.i4.1\n"; 
        }

        public String Visit(TWO node) {
            return "\t\tldc.i4.2\n"; 
        }

        
        public String Visit(THREE node) {
            return "\t\tldc.i4.3\n";    
        }

        string VisitChildren(Node node) {
            var sb = new StringBuilder();
            foreach (var n in node) {
                sb.Append(Visit((dynamic) n));
            }
            return sb.ToString();
        }
    }
    
    //---------------------------------------------------------------
    class Driver {
        public static void Main(string[] args) {
            if (args.Length != 1) {
                Console.Error.WriteLine("Inserta la operacions");
                Environment.Exit(1);
            }

            try {   
                
                /*foreach (var token in new Scanner(args[0]).Start()) {
                    Console.WriteLine(String.Format("{0}", token)
                    );
                } */                       
                var p = new Parser(
                    new Scanner(args[0]).Start().GetEnumerator());                
                var ast = p.Inicio();
                Console.WriteLine(ast.ToStringTree());
                File.WriteAllText(
                    "output.il", 
                    new CILGenerator().Visit((dynamic) ast));
                    
            } catch (SyntaxError) {
                Console.Error.WriteLine("parse error");
                Environment.Exit(1);
            }
        }
    }    
}