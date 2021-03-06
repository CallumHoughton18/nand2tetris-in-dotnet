using System.Xml;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core.Parser.Grammar;

sealed class ParserTreeToXmlDocument
{
    private readonly ParserTree _tree;

    public ParserTreeToXmlDocument(ParserTree tree)
    {
        _tree = tree;
    }

    public XmlDocument ToXml()
    {
        XmlDocument document = new();
        ToXmlNode(document, currentElement:null, _tree.Root);

        return document;
    }

    private void ToXmlNode(XmlDocument document, XmlElement? currentElement, ParserNode node)
    {
        var terminalNode = node as TerminalNode;
        var noneTerminalNode = node as NoneTerminalNode;

        if (terminalNode is not null)
        {
            var nodeLocalName = terminalNode.Token.TokenType switch
            {
                TokenType.UNDEFINED => "UNDEFINED",
                TokenType.KEYWORD => "keyword",
                TokenType.SYMBOL => "symbol",
                TokenType.STRING_CONST => "stringConstant",
                TokenType.INTEGER_CONST => "integerConstant",
                TokenType.IDENTIFIER => "identifier",
                _ => throw new ArgumentOutOfRangeException()
            };
            var terminalEle =
                currentElement.AppendChild(document.CreateXmlElement(nodeLocalName, terminalNode.Token.TokenValue));
        }
        else
        {
            var section = document.CreateElement(string.Empty, noneTerminalNode.Name, string.Empty);
            section.IsEmpty = false;
            
            if (currentElement is null)
            {
                document.AppendChild(section);
            }
            else
            {
                currentElement.AppendChild(section);
            }
            foreach (var otherNode in noneTerminalNode.Nodes)
            {
                ToXmlNode(document, section, otherNode);
            }
        }
    }
}