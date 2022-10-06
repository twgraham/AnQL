grammar AnQLGrammar;

options { caseInsensitive=true; }

AND:            'and' | '&';
OR:             'or';
NOT:            'not' | '!';
BOOLEAN:        'true' | 'false';
NULL:           'null';
NUMBER:         '-'? Int ('.' Int)?;
WORD:           AlphaNum+;
STOPWORD:       '-' + WORD;
QUOTE:          SingleQuote | DoubleQuote;
WHITESPACES:    (Whitespace | NewLine)+ -> channel(HIDDEN);

DOT:            '.';
COLON:          ':';
OPEN_PARENS:    '(';
CLOSE_PARENS:   ')';
LT:             '<';
GT:             '>';
COMMA:          ',';
UNDERSCORE:     '_';

// Fragments

fragment Alpha:         [a-z];
fragment AlphaNum:      Alpha | Digit;
fragment Digit:         [0-9];
fragment Int:           Digit+;
fragment SingleQuote:   '\'' (~'\'' | '\\\'' )* '\'';
fragment DoubleQuote:   ('"' (~'"' | '\\"' )* '"');

fragment NewLine
	: '\r\n' | '\r' | '\n'
	| '\u0085' // <Next Line CHARACTER (U+0085)>'
	| '\u2028' //'<Line Separator CHARACTER (U+2028)>'
	| '\u2029' //'<Paragraph Separator CHARACTER (U+2029)>'
	;

fragment Whitespace
	: UnicodeClassZS //'<Any Character With Unicode Class Zs>'
	| '\u0009' //'<Horizontal Tab Character (U+0009)>'
	| '\u000B' //'<Vertical Tab Character (U+000B)>'
	| '\u000C' //'<Form Feed Character (U+000C)>'
	;

fragment UnicodeClassZS
	: '\u0020' // SPACE
	| '\u00A0' // NO_BREAK SPACE
	| '\u1680' // OGHAM SPACE MARK
	| '\u180E' // MONGOLIAN VOWEL SEPARATOR
	| '\u2000' // EN QUAD
	| '\u2001' // EM QUAD
	| '\u2002' // EN SPACE
	| '\u2003' // EM SPACE
	| '\u2004' // THREE_PER_EM SPACE
	| '\u2005' // FOUR_PER_EM SPACE
	| '\u2006' // SIX_PER_EM SPACE
	| '\u2008' // PUNCTUATION SPACE
	| '\u2009' // THIN SPACE
	| '\u200A' // HAIR SPACE
	| '\u202F' // NARROW NO_BREAK SPACE
	| '\u3000' // IDEOGRAPHIC SPACE
	| '\u205F' // MEDIUM MATHEMATICAL SPACE
	;

// Entrypoint
query: expr;

expr: NOT expr                      # NOT
    | expr AND expr                 # ExprAND
    | expr OR* expr                 # ExprOR
    | (text | keyOpValue)           # TextOrFilter
    | '(' expr ')'                  # Parens
    ;

text: (QUOTE | STOPWORD | WORD)+;

property_path : property ('.' property)* ;

keyOpValue  : property_path ':' '>' value                  # GreaterThan
            | property_path ':' '<' value                  # LessThan
            | property_path ':' value                      # Equal
            | property_path ':' value (',' value)+         # AnyEqual
            ;

property: (WORD ('_' WORD)* | reserved_word);

reserved_word: NULL | BOOLEAN;

value : NULL                        # null
      | BOOLEAN                     # bool
      | NUMBER                      # number
      | (QUOTE | WORD)              # string
      ;
