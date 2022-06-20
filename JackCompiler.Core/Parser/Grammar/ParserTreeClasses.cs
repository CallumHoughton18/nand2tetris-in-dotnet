using System.Data;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core.Parser.Grammar;

sealed class ParserTree
{
    public NoneTerminalNode? Root { get; private set; } = null;
    public NoneTerminalNode? CurrentNode { get; private set; }

    public void AddTerminalNode(TerminalNode node)
    {
        CurrentNode?.Nodes.Add(node);
    }

    public void AddNoneTerminalNodeAndAdvance(NoneTerminalNode node)
    {
        if (Root is null)
        {
            Root = node;
            CurrentNode = node;
        }
        else
        {
            CurrentNode?.Nodes.Add(node);
            CurrentNode = node;
        }
    }
    
    public void AddNoneTerminalNodeAndAdvance(string nodeValue)
    {
        var node = new NoneTerminalNode(CurrentNode, nodeValue);
        AddNoneTerminalNodeAndAdvance(node);
    }

    public void SetCurrentNodeToPrevious()
    {
        if (CurrentNode == Root) return;
        
        var parentNoneTerminal = CurrentNode?.Parent as NoneTerminalNode;
        CurrentNode = parentNoneTerminal 
                      ?? throw new NoNullAllowedException("Parent node cannot be null when navigating back");
    }
}

abstract class ParserNode 
{
    public ParserNode? Parent { get; }

    public ParserNode(ParserNode? parent)
    {
        Parent = parent;
    }
}

sealed class NoneTerminalNode : ParserNode
{
    public string Name { get; }
    public IList<ParserNode> Nodes { get; set; } = new List<ParserNode>();

    public NoneTerminalNode(ParserNode? parent, string name) : base(parent)
    {
        Name = name;
    }
}

sealed class TerminalNode : ParserNode
{
    public Token Token { get; }

    public TerminalNode(ParserNode? parent, Token token) : base(parent)
    {
        Token = token;
    }
}