//---------------------------------------------------------
// Type your name and student ID here.
// Guillermo Pérez Trueba
// A01377162
//---------------------------------------------------------

%{

#include <stdio.h>
#include <stdarg.h>

int yylex(void);
void yyerror(char *s, ...);
extern int yylineno;

%}

/* declare tokens */
%token INTEGER ADD MUL PAR_LEFT PAR_RIGHT EOL

/* Specify operator precedence and associativity */
%left ADD
%left MUL

%%

calclist:
    /* nothing */ { }                            /* matches at beginning of input */
    | calclist exp EOL { printf("%d\n> ", $2); } /* EOL is end of an expression */
;

exp:
    PAR_LEFT ADD PAR_RIGHT { $$ = 0; }
    | PAR_LEFT MUL PAR_RIGHT { $$ = 1; }
    | PAR_LEFT ADD exp_cont_sum PAR_RIGHT { $$ = $3; }
    | PAR_LEFT MUL exp_cont_mul PAR_RIGHT { $$ = $3; }
    | INTEGER
;

exp_cont_sum:
    exp_cont_sum exp_cont_sum {$$ = $1 + $2; }
    | exp
;

exp_cont_mul:
    exp_cont_mul exp_cont_mul {$$ = $1 * $2; }
    | exp
%%

int main(int argc, char **argv) {
    printf("> ");
    yyparse();
    return 0;
}

void yyerror(char *s, ...) {
    va_list ap;
    va_start(ap, s);
    fprintf(stderr, "Line %d: ", yylineno);
    vfprintf(stderr, s, ap);
    fprintf(stderr, "\n");
}
