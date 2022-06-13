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
        var parserTree = new ParserTree();

        // TODO: just testing the nesting here to make sure it's right before implementing the grammar properly
        foreach (var token in _tokens)
        {
            if (token.TokenValue == "class" && parserTree.CurrentNode.Name != "class")
            {
                parserTree.AddNoneTerminalNodeAndAdvance(new NoneTerminalNode(parserTree.CurrentNode, "class"));
            }
            if (parserTree.CurrentNode?.Name == "class" && token.TokenValue == "}")
            {
                parserTree.SetCurrentNodeToPrevious();
            }

            parserTree.AddTerminalNode(new TerminalNode(parserTree.CurrentNode, token));
        }
        return parserTree;
    }
}

sealed class JackGrammarSection
{
    public JackGrammarSection(string grammarName)
    {
        GrammarName = grammarName;
        Tokens = new List<Token>();
        OtherGrammars = new List<JackGrammarSection>();
    }
    
    public string GrammarName { get; }
    public IList<Token> Tokens { get;}
    public IList<JackGrammarSection> OtherGrammars { get; }
}

