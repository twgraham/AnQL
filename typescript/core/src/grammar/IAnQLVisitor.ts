import { AnQLGrammarVisitor } from '@gen/grammar/AnQLGrammarVisitor'

export default interface IAnQLVisitor<T> extends AnQLGrammarVisitor<T> {
    successQueryResult: T
    failedQueryResult: T
}
