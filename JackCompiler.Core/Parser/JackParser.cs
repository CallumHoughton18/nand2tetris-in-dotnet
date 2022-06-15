using JackCompiler.Core.Parser.Grammar;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core.Parser;

sealed class JackParser
{
    private readonly IList<Token> _tokens;

    public JackParser(IList<Token> tokens)
    {
        _tokens = tokens;
    }
 
    public ParserTree ParseTokens()
    {
        var grammarStack = new Stack<BaseGrammar>();
        BaseGrammar currentGrammar = null;
        var parserTree = new ParserTree();
        
        for (int i = 0; i < _tokens.Count;)
        {
            var token = _tokens[i];
            if (token.TokenValue == "class")
            {
                var newClass = new ClassGrammar();
                grammarStack.Push(newClass);
                currentGrammar = grammarStack.Peek();
                parserTree.AddNoneTerminalNodeAndAdvance(new NoneTerminalNode(parserTree.CurrentNode, currentGrammar.Name));
            }

            var (isNewGrammar, newGrammar, addTokenToNewGrammar) = currentGrammar.IsNewChild(token);
            if (isNewGrammar)
            {
                grammarStack.Push(newGrammar);
                var prevGrammar = currentGrammar as StatementsGrammar;
                var prev1Grammar = currentGrammar as WhileStatementGrammar;
                // If prev grammar is a statement, and new grammar is a statements, then i should also advance.
                
                currentGrammar = newGrammar;
                if (!addTokenToNewGrammar.GetValueOrDefault(true)) parserTree.AddTerminalNode(new TerminalNode(parserTree.CurrentNode, token));
                parserTree.AddNoneTerminalNodeAndAdvance(new NoneTerminalNode(parserTree.CurrentNode, currentGrammar.Name));
                if (prevGrammar is null && currentGrammar.ShouldAdvanceToNextToken) i++;
                if (prev1Grammar is not null && (currentGrammar as StatementsGrammar) != null) i++;
                continue;
            }

            var (endOfGrammar, addTokenToCurrentGrammar) = currentGrammar.IsEndOfGrammar(token);
            if (endOfGrammar)
            {

                if (addTokenToCurrentGrammar) parserTree.AddTerminalNode(new TerminalNode(parserTree.CurrentNode, token));
                if (grammarStack.Count > 0)
                {
                    grammarStack.Pop();
                    if (grammarStack.Count > 0)
                    {
                        currentGrammar = grammarStack.Peek();

                    }
                }
                parserTree.SetCurrentNodeToPrevious();
                
                string[] statementBeginnings = { "let", "if", "while", "do", "return" };
                var forceAdvance = i < _tokens.Count - 2 && statementBeginnings.Any(x => x ==_tokens[i+1].TokenValue);
                
                if (currentGrammar.ShouldAdvanceToNextToken || forceAdvance) i++;
                if (!addTokenToCurrentGrammar) parserTree.AddTerminalNode(new TerminalNode(parserTree.CurrentNode, token));
                continue;
            }
            
            parserTree.AddTerminalNode(new TerminalNode(parserTree.CurrentNode, token));
            if (currentGrammar.ShouldAdvanceToNextToken) i++;
        }
        return parserTree;
    }
}
