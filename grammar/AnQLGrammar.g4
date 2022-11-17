grammar AnQLGrammar;

// Entrypoint
query: expr;

expr:
	'(' expr ')'							# Parens
	| Not expr								# Not
	| (Quote | Stopword | Word)+			# FreeText
	| propertyPath ':' '>' value			# GreaterThan
	| propertyPath ':' '<' value			# LessThan
	| propertyPath ':' value				# Equal
	| propertyPath ':' value (',' value)+	# AnyEqual
	| left = expr And right = expr			# And
	| left = expr Or* right = expr			# Or;

propertyPath: property ('.' property)*;

property: Identifier;

value:
	Null				# Null
	| Boolean			# Bool
	| Number			# Number
	| (Word | Quote)	# String;

// Lexer
And: A N D | '&&';
Or: O R | '||';
Not: N O T | '!';
Boolean: 'true' | 'false';
Null: N U L L;
Number: '-'? Int ('.' Int)?;
Word: AlphaNum+;
Stopword: '-'+ Word;
Quote: SingleQuote | DoubleQuote;
Identifier: ([a-zA-Z_] [a-zA-Z_0-9]*) | Word;
Whitespaces: (Whitespace | NewLine)+ -> channel(HIDDEN);

// Fragments

fragment Alpha: [a-z];
fragment AlphaNum: Alpha | Digit;
fragment Digit: [0-9];
fragment Int: Digit+;
fragment SingleQuote: '\'' (~'\'' | '\\\'')* '\'';
fragment DoubleQuote: ('"' (~'"' | '\\"')* '"');

fragment NewLine:
	'\r\n'
	| '\r'
	| '\n'
	| '\u0085' // <Next Line CHARACTER (U+0085)>'
	| '\u2028' //'<Line Separator CHARACTER (U+2028)>'
	| '\u2029' ; //'<Paragraph Separator CHARACTER (U+2029)>'

fragment Whitespace:
	UnicodeClassZS //'<Any Character With Unicode Class Zs>'
	| '\u0009' //'<Horizontal Tab Character (U+0009)>'
	| '\u000B' //'<Vertical Tab Character (U+000B)>'
	| '\u000C' ; //'<Form Feed Character (U+000C)>'

fragment UnicodeClassZS:
	'\u0020' // SPACE
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
	| '\u205F' ; // MEDIUM MATHEMATICAL SPACE

fragment A: [aA];
fragment B: [bB];
fragment C: [cC];
fragment D: [dD];
fragment E: [eE];
fragment F: [fF];
fragment G: [gG];
fragment H: [hH];
fragment I: [iI];
fragment J: [jJ];
fragment K: [kK];
fragment L: [lL];
fragment M: [mM];
fragment N: [nN];
fragment O: [oO];
fragment P: [pP];
fragment Q: [qQ];
fragment R: [rR];
fragment S: [sS];
fragment T: [tT];
fragment U: [uU];
fragment V: [vV];
fragment W: [wW];
fragment X: [xX];
fragment Y: [yY];
fragment Z: [zZ];
