using JackCompiler.Core.Parser.Grammar;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core.Parser;

sealed class CompilationEngine
{
    public Token CurrentToken => _tokens[_index];
    private int _index = 0;
    private readonly IList<Token> _tokens;
    private readonly ParserTree _parserTree = new();

    private readonly string[] _binaryOperators = { "+", "-", "*", "/", "|", "=", "<", ">", "&" };
    private readonly string[] _unaryOperators = { "-", "~" };
    private readonly string[] _keyWordConstants = { "true", "false", "null", "this" };

    public CompilationEngine(IList<Token> tokens)
    {
        _tokens = tokens;
    }

    public ParserTree CompileClass()
    {
        _index = 0;
        if (CurrentToken.TokenValue != "class") throw new InvalidDataException("First token is not a class");

        AddNoneTerminalSection("class");
        AdvanceAndAddTerminal();
        AdvanceAndAddTerminal();
        AdvanceAndAddTerminal();

        if (HasClassVarDec())
        {
            CompileClassVarDec();
        }

        while (HasSubRoutine())
        {
            CompileSubRoutine();
        }

        AdvanceAndAddTerminal();
        return _parserTree;
    }

    private void CompileClassVarDec()
    {
        while (HasClassVarDec())
        {
            AddNoneTerminalSection("classVarDec");

            AdvanceAndAddTerminal(); // static or field
            AdvanceAndAddTerminal(); // var type
            AdvanceAndAddTerminal(); // var name

            while (NextTokenValueIs(","))
            {
                AdvanceAndAddTerminal(); // the ',' character
                AdvanceAndAddTerminal(); // var name
            }

            AdvanceAndAddTerminal(); // end of classVarDec, so ';' character
            EndOfNoneTerminalSection();
        }
    }

    private void CompileSubRoutine()
    {
        _parserTree.AddNoneTerminalNodeAndAdvance("subroutineDec");
        AdvanceAndAddTerminal(); // subroutine type
        AdvanceAndAddTerminal(); // subroutine return type or constructor
        AdvanceAndAddTerminal(); // subroutine name 
        AdvanceAndAddTerminal(); // '('

        CompileParametersList();

        AdvanceAndAddTerminal(); // ')'

        CompileSubRoutineBody();

        EndOfNoneTerminalSection();
    }

    private void CompileParametersList()
    {
        AddNoneTerminalSection("parameterList");
        while (!NextTokenTypeIs(TokenType.SYMBOL))
        {
            AdvanceAndAddTerminal(); // param type
            AdvanceAndAddTerminal(); // param name
            if (NextTokenValueIs(","))
            {
                AdvanceAndAddTerminal(); // add ','
            }
        }

        EndOfNoneTerminalSection();
    }

    private void CompileSubRoutineBody()
    {
        AddNoneTerminalSection("subroutineBody");
        AdvanceAndAddTerminal(); // '{'
        while (NextTokenValueIs("var"))
        {
            CompileVarDec();
        }

        CompileStatements();

        AdvanceAndAddTerminal(); // '}'
        EndOfNoneTerminalSection();
    }

    private void CompileVarDec()
    {
        AddNoneTerminalSection("varDec");
        AdvanceAndAddTerminal(); // "var" keyword
        AdvanceAndAddTerminal(); // var type
        AdvanceAndAddTerminal(); // var name

        while (NextTokenValueIs(","))
        {
            AdvanceAndAddTerminal(); // , symbol
            AdvanceAndAddTerminal(); // var name
        }

        AdvanceAndAddTerminal(); // ; symbol
        EndOfNoneTerminalSection();
    }

    private void CompileStatements()
    {
        AddNoneTerminalSection("statements");

        while (HasStatement())
        {
            switch (CurrentToken.TokenValue)
            {
                case "let":
                    CompileLetStatement();
                    break;
                case "if":
                    CompileIfStatement();
                    break;
                case "while":
                    CompileWhileStatement();
                    break;
                case "do":
                    CompileDoStatement();
                    break;
                case "return":
                    CompileReturnStatement();
                    break;
            }
        }

        EndOfNoneTerminalSection();
    }

    private void CompileLetStatement()
    {
        AddNoneTerminalSection("letStatement");

        AdvanceAndAddTerminal(); // let keyword
        AdvanceAndAddTerminal(); // varName

        if (NextTokenValueIs("["))
        {
            AdvanceAndAddTerminal(); // [
            CompileExpression();
            AdvanceAndAddTerminal(); // ]
        }

        AdvanceAndAddTerminal(); // = 

        CompileExpression();
        AdvanceAndAddTerminal(); // ;

        EndOfNoneTerminalSection();
    }

    private void CompileIfStatement()
    {
        AddNoneTerminalSection("ifStatement");

        AdvanceAndAddTerminal(); // if keyword

        AdvanceAndAddTerminal(); // (
        CompileExpression();
        AdvanceAndAddTerminal(); // )

        AdvanceAndAddTerminal(); // {
        CompileStatements();
        AdvanceAndAddTerminal(); // }

        if (NextTokenValueIs("else"))
        {
            AdvanceAndAddTerminal(); // else

            AdvanceAndAddTerminal(); // {
            CompileStatements();
            AdvanceAndAddTerminal(); // }
        }

        EndOfNoneTerminalSection();
    }

    private void CompileWhileStatement()
    {
        AddNoneTerminalSection("whileStatement");
        
        AdvanceAndAddTerminal(); // while
        AdvanceAndAddTerminal(); // (
        CompileExpression();
        AdvanceAndAddTerminal(); // )
        
        AdvanceAndAddTerminal(); // {
        CompileStatements(); 
        AdvanceAndAddTerminal(); // }
        
        EndOfNoneTerminalSection();
    }

    private void CompileDoStatement()
    {
        AddNoneTerminalSection("doStatement");
        AdvanceAndAddTerminal(); // do
        // subroutineCall
        AdvanceAndAddTerminal(); // class or var name
        if (NextTokenValueIs("."))
        {
            AdvanceAndAddTerminal(); // . symbol
            AdvanceAndAddTerminal(); // subroutine name
        }
        
        AdvanceAndAddTerminal(); // (
        CompileExpressionList();
        AdvanceAndAddTerminal(); // )
        
        AdvanceAndAddTerminal(); // ;
        EndOfNoneTerminalSection();
    }

    private void CompileReturnStatement()
    {
        AddNoneTerminalSection("returnStatement");
        AdvanceAndAddTerminal(); // return keyword
        if (HasExpression())
        {
            CompileExpression();
        }
        AdvanceAndAddTerminal(); // ;
        EndOfNoneTerminalSection();
    }

    private void CompileExpression()
    {
        AddNoneTerminalSection("expression"); 
        CompileTerm();
        while (_binaryOperators.Any(NextTokenValueIs))
        {
            AdvanceAndAddTerminal(); // op symbol
            CompileTerm();
        }

        EndOfNoneTerminalSection();
    }

    private void CompileTerm()
    {
        AddNoneTerminalSection("term");

        if (NextTokenTypeIs(TokenType.STRING_CONST, TokenType.INTEGER_CONST) || _keyWordConstants.Any(NextTokenValueIs))
        {
            AdvanceAndAddTerminal(); // constant val
        }
        else if (NextTokenTypeIs(TokenType.IDENTIFIER))
        {
            AdvanceAndAddTerminal(); // class name or var name
            switch (CurrentToken.TokenValue)
            {
                case "[": // array access
                    AdvanceAndAddTerminal(); // [
                    CompileExpression();
                    AdvanceAndAddTerminal(); // ]
                    break;
                case "(":
                    AdvanceAndAddTerminal(); // (
                    CompileExpressionList();
                    AdvanceAndAddTerminal(); // )
                    break;
                case ".":
                    AdvanceAndAddTerminal(); // .
                    AdvanceAndAddTerminal(); // subroutine name
                    AdvanceAndAddTerminal(); // (
                    CompileExpressionList();
                    AdvanceAndAddTerminal(); // )
                    break;
            }
        }
        else if (NextTokenValueIs("("))
        {
            AdvanceAndAddTerminal(); // (
            CompileExpression();
            AdvanceAndAddTerminal(); // )
        }
        else if (_unaryOperators.Any(NextTokenValueIs))
        {
            AdvanceAndAddTerminal(); // unary op symbol
            CompileTerm();
        }
        
        EndOfNoneTerminalSection();
    }

    private void CompileExpressionList()
    {
        AddNoneTerminalSection("expressionList");
        if (HasExpression())
        {
            CompileExpression();
        }

        while (NextTokenValueIs(","))
        {
            AdvanceAndAddTerminal(); // , symbol
            CompileExpression();
        }
        
        EndOfNoneTerminalSection();
    }

    private bool HasExpression()
    {
        return (NextTokenTypeIs(TokenType.INTEGER_CONST, TokenType.STRING_CONST, TokenType.IDENTIFIER)
                || _unaryOperators.Any(NextTokenValueIs)
                || _keyWordConstants.Any(NextTokenValueIs)
                || NextTokenValueIs("("));
    }

    private bool HasStatement()
    {
        var statmentValues = new[] { "do", "let", "if", "while", "return" };
        return statmentValues.Any(NextTokenValueIs);
    }

    public bool HasClassVarDec()
    {
        return NextTokenValueIs("static") || NextTokenValueIs("field");
    }

    public bool HasSubRoutine()
    {
        return NextTokenValueIs("constructor") || NextTokenValueIs("method") || NextTokenValueIs("function");
    }

    public bool NextTokenValueIs(string value)
    {
        var nextToken = _tokens[_index];
        return nextToken.TokenValue == value;
    }

    private bool NextTokenTypeIs(params TokenType[] tokenTypes)
    {
        var nextToken = _tokens[_index];
        return tokenTypes.Any(x => nextToken.TokenType == x);
    }

    private void AdvanceAndAddTerminal()
    {
        _parserTree.AddTerminalNode(new TerminalNode(_parserTree.CurrentNode, CurrentToken));
        _index++;
    }

    private void EndOfNoneTerminalSection()
    {
        _parserTree.SetCurrentNodeToPrevious();
    }

    private void AddNoneTerminalSection(string noneTerminalName)
    {
        _parserTree.AddNoneTerminalNodeAndAdvance(noneTerminalName);
    }
}