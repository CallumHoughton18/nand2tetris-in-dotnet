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
            var terminalEle =
                currentElement.AppendChild(document.CreateXmlElement("terminal", terminalNode.Token.TokenValue));
        }
        else
        {
            var section = document.CreateElement(string.Empty, noneTerminalNode.Name, string.Empty);
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