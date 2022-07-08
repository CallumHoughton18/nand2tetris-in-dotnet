using JackCompiler.Core.Code_Writer;
using JackCompiler.Core.Parser.Grammar;
using JackCompiler.Core.Symbol_Table;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core.Parser;

sealed class CompilationEngine
{
    public Token CurrentToken => _tokens[_index];
    private int _index = 0;
    private int _counter = 0;
    private readonly IList<Token> _tokens;
    private readonly VMWriter _writer;

    private readonly string[] _binaryOperators = { "+", "-", "*", "/", "|", "=", "<", ">", "&" };
    private readonly string[] _unaryOperators = { "-", "~" };
    private readonly string[] _keyWordConstants = { "true", "false", "null", "this" };
    private readonly SymbolTable _symbolTable;

    public CompilationEngine(IList<Token> tokens, VMWriter writer)
    {
        _tokens = tokens;
        _writer = writer;
        _symbolTable = new SymbolTable();
    }

    private int GetUniqueId()
    {
        _counter++;
        return _counter;
    }

    public void CompileClass()
    {
        _index = 0;
        if (CurrentToken.TokenValue != "class") throw new InvalidDataException("First token is not a class");

        var classToken = CurrentToken;
        Advance(); // {

        if (HasClassVarDec())
        {
            CompileClassVarDec();
        }

        while (HasSubRoutine())
        {
            CompileSubRoutine();
        }

        Advance(); // }
        _writer.Close();
    }

    private void CompileClassVarDec()
    {
        while (HasClassVarDec())
        {
            var kind = EnumHelpers.FromString<SymbolKind>(Advance().TokenValue); // static or field
            var type = Advance().TokenValue; // var type
            var name = Advance().TokenValue; // var name
            _symbolTable.Define(name, type, kind);

            while (NextTokenValueIs(","))
            {
                Advance(); // the ',' character
                name = Advance().TokenValue; // var name
                _symbolTable.Define(name, type, kind);
            }

            Advance(); // end of classVarDec, so ';' character
        }
    }

    
    private void CompileSubRoutine()
    {
        _symbolTable.StartSubRoutine();
        var subroutineType = Advance().TokenValue; // subroutine type
        Advance(); // subroutine return type or constructor
        var subroutineName = Advance().TokenValue; // subroutine name 
        Advance(); // '('

        // you must pass in self as the first argument to a method.
        if (subroutineType == "method") _symbolTable.Define("this", "self", SymbolKind.ARG);
        CompileParametersList();

        Advance(); // ')'

        CompileSubRoutineBody(subroutineName, subroutineType);
    }

    private void CompileParametersList()
    {
        while (!NextTokenTypeIs(TokenType.SYMBOL))
        {
            var type = Advance().TokenValue; // param type
            var name = Advance().TokenValue; // param name
            _symbolTable.Define(name, type, SymbolKind.ARG);
            if (NextTokenValueIs(","))
            {
                Advance(); // add ','
            }
        }
    }

    private void CompileSubRoutineBody(string subroutineName, string subroutineType)
    {
        Advance(); // '{'
        while (NextTokenValueIs("var"))
        {
            CompileVarDec();
        }

        var nVars = _symbolTable.VarCount(SymbolKind.VAR);
        _writer.WriteFunction(subroutineName, nVars);

        if (subroutineType == "method")
        {
            _writer.WritePush(PushSegments.ARGUMENT, 0);
            _writer.WritePop(PopSegments.POINTER, 0);
        }
        else if (subroutineType == "constructor")
        {
            var classLevelFields = _symbolTable.VarCount(SymbolKind.FIELD);
            _writer.WritePush(PushSegments.CONSTANT, classLevelFields);
            _writer.WriteCall("Memory.alloc", 1);
            _writer.WritePop(PopSegments.POINTER, 0);
        }
        
        CompileStatements();

        Advance(); // '}'
    }

    private void CompileVarDec()
    {
        var kind = EnumHelpers.FromString<SymbolKind>(Advance().TokenValue); // "var" keyword
        var type = Advance().TokenValue; // var type
        var name = Advance().TokenValue; // var name
        _symbolTable.Define(name, type, kind);

        while (NextTokenValueIs(","))
        {
            Advance(); // , symbol
            name = Advance().TokenValue; // var name
            _symbolTable.Define(name, type, kind);
        }

        Advance(); // ; symbol
    }

    private void CompileStatements()
    {
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
    }

    private void CompileLetStatement()
    {
        Advance(); // let keyword
        Advance(); // varName

        if (NextTokenValueIs("["))
        {
            Advance(); // [
            CompileExpression();
            Advance(); // ]
        }

        Advance(); // = 

        CompileExpression();
        Advance(); // ;
    }

    private void CompileIfStatement()
    {
        Advance(); // if keyword
        Advance(); // (
        CompileExpression();
        Advance(); // )
        
        var L1 = $"L1-{GetUniqueId()}";
        var L2 = $"L2-{GetUniqueId()}";
        
        _writer.WriteArithmetic(ArithmeticCommands.NOT);
        _writer.WriteIf(L1);
        Advance(); // {
        CompileStatements();
        _writer.WriteGoto(L2);
        Advance(); // }
        _writer.WriteLabel(L1);

        if (NextTokenValueIs("else"))
        {
            Advance(); // else
            Advance(); // {
            CompileStatements();
            Advance(); // }
        }
        _writer.WriteLabel(L2);
    }

    private void CompileWhileStatement()
    {
        var l1 = $"while-{GetUniqueId()}";
        var l2 = $"while-{GetUniqueId()}";
        _writer.WriteLabel(l1);
        Advance(); // while
        Advance(); // (
        CompileExpression();
        Advance(); // )
        
        _writer.WriteArithmetic(ArithmeticCommands.NOT);
        _writer.WriteIf(l2);
        Advance(); // {
        CompileStatements(); 
        Advance(); // }
        _writer.WriteGoto(l1);
        _writer.WriteLabel(l2);
    }

    private void CompileDoStatement()
    {
        Advance(); // do
        // subroutineCall
        var classOrVarName = Advance().TokenValue; // class or var name
        if (NextTokenValueIs("."))
        {
            Advance(); // . symbol
            var subRoutineName = Advance().TokenValue; // subroutine name
            if ()
        }
        
        Advance(); // (
        CompileExpressionList();
        Advance(); // )
        
        Advance(); // ;
    }

    private void CompileReturnStatement()
    {
        AddNoneTerminalSection("returnStatement");
        Advance(); // return keyword
        if (HasExpression())
        {
            CompileExpression();
        }
        Advance(); // ;
        EndOfNoneTerminalSection();
    }

    private void CompileExpression()
    {
        CompileTerm();
        
        while (_binaryOperators.Any(NextTokenValueIs))
        {
            var opSymbol = Advance().TokenValue; // op symbol
            if (opSymbol == "+") _writer.WriteArithmetic(ArithmeticCommands.ADD);
            else if (opSymbol == "-") _writer.WriteArithmetic(ArithmeticCommands.SUB);
            else if (opSymbol == "*") _writer.WriteCall("Math.multiply", 2);
            else if (opSymbol == "/") _writer.WriteCall("Math.divide", 2);
            else if (opSymbol == "<") _writer.WriteArithmetic(ArithmeticCommands.LT);
            else if (opSymbol == ">") _writer.WriteArithmetic(ArithmeticCommands.GT);
            else if (opSymbol == "|") _writer.WriteArithmetic(ArithmeticCommands.OR);
            else if (opSymbol == "&") _writer.WriteArithmetic(ArithmeticCommands.ADD);
            else if (opSymbol == "=") _writer.WriteArithmetic(ArithmeticCommands.EQ);
            
            CompileTerm();
        }
    }

    private void CompileTerm()
    {
        if (NextTokenTypeIs(TokenType.STRING_CONST, TokenType.INTEGER_CONST) || _keyWordConstants.Any(NextTokenValueIs))
        {
            Advance(); // constant val
        }
        else if (NextTokenTypeIs(TokenType.IDENTIFIER))
        {
            Advance(); // class name or var name
            switch (CurrentToken.TokenValue)
            {
                case "[": // array access
                    Advance(); // [
                    CompileExpression();
                    Advance(); // ]
                    break;
                case "(":
                    Advance(); // (
                    CompileExpressionList();
                    Advance(); // )
                    break;
                case ".":
                    Advance(); // .
                    Advance(); // subroutine name
                    Advance(); // (
                    CompileExpressionList();
                    Advance(); // )
                    break;
            }
        }
        else if (NextTokenValueIs("("))
        {
            Advance(); // (
            CompileExpression();
            Advance(); // )
        }
        else if (_unaryOperators.Any(NextTokenValueIs))
        {
            Advance(); // unary op symbol
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
            Advance(); // , symbol
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

    private Token Advance()
    {
        _index++;
        return CurrentToken;
    }
}