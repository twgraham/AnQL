import { CharStreams, CommonTokenStream } from 'antlr4ts'

import { AnQLGrammarParser } from '@gen/grammar/AnQLGrammarParser'
import { AnQLGrammarLexer } from '@gen/grammar/AnQLGrammarLexer'

export function buildGrammarParser(query: string): AnQLGrammarParser {
    return new AnQLGrammarParser(new CommonTokenStream(new AnQLGrammarLexer(CharStreams.fromString(query))));
}