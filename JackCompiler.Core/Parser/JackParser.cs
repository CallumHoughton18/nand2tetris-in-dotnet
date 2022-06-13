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



        foreach (var token in _tokens)
        {
            if (token.TokenValue == "class" && parserTree.CurrentNode?.Name != "class")
            {
                parserTree.AddNoneTerminalNodeAndAdvance(new NoneTerminalNode(parserTree.CurrentNode, "class"));
            }
            if (parserTree.CurrentNode?.Name == "class" && token.TokenValue == "}")
            {
                parserTree.SetCurrentNodeToPrevious();
            }
            
            if (token.TokenValue == "function" && parserTree.CurrentNode?.Name != "subRoutineDec")
            {
                parserTree.AddNoneTerminalNodeAndAdvance(new NoneTerminalNode(parserTree.CurrentNode, "subRoutineDec"));
            }
            if (parserTree.CurrentNode?.Name == "subRoutineDec" && token.TokenValue == "}")
            {
                parserTree.SetCurrentNodeToPrevious();
            }
            
            //TODO: need to add these terminal ones to the correct current section, which is being reset above
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

