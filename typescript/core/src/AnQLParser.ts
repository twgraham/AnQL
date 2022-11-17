import { buildGrammarParser } from './AnQL'
import './grammar'
import IAnQLVisitor from './grammar/IAnQLVisitor'

export default class AnQLParser<T> {
    private readonly visitor: IAnQLVisitor<T>

    constructor(visitor: IAnQLVisitor<T>) {
        this.visitor = visitor
    }

    public parse(input: string): T {
        if (input == '') {
            return this.visitor.successQueryResult
        }

        const anqlParser = buildGrammarParser(input)

        return this.visitor.visit(anqlParser.query()) ?? this.visitor.successQueryResult
    }
}