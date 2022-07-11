using JackCompiler.Core.Code_Writer;
using JackCompiler.Core.Parser.Grammar;
using JackCompiler.Core.Symbol_Table;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core.Parser;

sealed class CompilationEngine
{
    private int _index = -1;
    private int _counter = 0;
    private readonly IList<Token> _tokens;
    private readonly VMWriter _writer;
    private string _currentClassName;

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
        var token = Advance(); // class
        if (token.TokenValue != "class") throw new InvalidDataException("First token is not a class");
        _currentClassName = Advance().TokenValue;
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

        CompileSubRoutineBody($"{_currentClassName}.{subroutineName}", subroutineType);
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
            switch (Advance().TokenValue)
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
        var isArray = false;
        var varNameToken = Advance(); // varName
        
        if (NextTokenValueIs("["))
        {
            isArray = true;
            CompileArrayIndex(varNameToken.TokenValue);
        }

        Advance(); // = 
        CompileExpression();
        if (isArray)
        {
            _writer.WritePop(PopSegments.TEMP , 0);
            _writer.WritePop(PopSegments.POINTER, 1);
            _writer.WritePush(PushSegments.TEMP, 0);
            _writer.WritePop(PopSegments.THAT, 0);
        }
        else
        {
            SymbolKindToPopStatementIfExists(varNameToken.TokenValue);
        }
        Advance(); // ;
    }

    private void CompileIfStatement()
    {
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
        var callName = "";
        var nLocals = 0;
        // subroutineCall
        var classOrVarName = Advance().TokenValue; // class or var name
        if (NextTokenValueIs("."))
        {
            Advance(); // . symbol
            var subRoutineName = Advance().TokenValue; // subroutine name
            if (_symbolTable.HasSymbolInEitherScope(classOrVarName, out _))
            {
                SymbolKindToPushStatementIfExists(classOrVarName);
                callName = $"{_symbolTable.TypeOf(classOrVarName)}.{subRoutineName}";
                nLocals += 1;
            }
            else
            {
                callName = $"{classOrVarName}.{subRoutineName}";
            }
        }
        else
        {
            _writer.WritePush(PushSegments.POINTER, 0);
            callName = $"{_currentClassName}.{classOrVarName}";
            nLocals += 1;

        }

        Advance(); // (
        nLocals += CompileExpressionList();
        _writer.WriteCall(callName, nLocals);
        _writer.WritePop(PopSegments.TEMP, 0);
        Advance(); // )
        Advance(); // ;
    }

    private void CompileReturnStatement()
    {
        var shouldReturnEmpty = true;
        while (HasExpression())
        {
            shouldReturnEmpty = false;
            CompileExpression();
        }

        if (shouldReturnEmpty)
        {
            _writer.WritePush(PushSegments.CONSTANT, 0);
        }
        _writer.WriteReturn();
        Advance(); // ;
    }

    private void CompileExpression()
    {
        CompileTerm();
        
        while (_binaryOperators.Any(NextTokenValueIs))
        {
            var opSymbol = Advance().TokenValue; // op symbol
            CompileTerm();
            if (opSymbol == "+") _writer.WriteArithmetic(ArithmeticCommands.ADD);
            else if (opSymbol == "-") _writer.WriteArithmetic(ArithmeticCommands.SUB);
            else if (opSymbol == "*") _writer.WriteCall("Math.multiply", 2);
            else if (opSymbol == "/") _writer.WriteCall("Math.divide", 2);
            else if (opSymbol == "<") _writer.WriteArithmetic(ArithmeticCommands.LT);
            else if (opSymbol == ">") _writer.WriteArithmetic(ArithmeticCommands.GT);
            else if (opSymbol == "|") _writer.WriteArithmetic(ArithmeticCommands.OR);
            else if (opSymbol == "&") _writer.WriteArithmetic(ArithmeticCommands.ADD);
            else if (opSymbol == "=") _writer.WriteArithmetic(ArithmeticCommands.EQ);
        }
    }

    private void CompileTerm()
    {
        if (NextTokenTypeIs(TokenType.INTEGER_CONST))
        {
            var value = Advance().TokenValue;
            _writer.WritePush(PushSegments.CONSTANT, int.Parse(value));
        }
        else if (NextTokenTypeIs(TokenType.STRING_CONST))
        {
            var value = Advance().TokenValue;
            _writer.WritePush(PushSegments.CONSTANT, value.Length);
            _writer.WriteCall("String.new", 1);
            foreach (var chr in value)
            {
                _writer.WritePush(PushSegments.CONSTANT, chr);
                _writer.WriteCall("String.appendChar", 2);
                
            }
        }
        else if (_keyWordConstants.Any(NextTokenValueIs))
        {
            var value = Advance().TokenValue;
            if (value == "this") _writer.WritePush(PushSegments.POINTER, 0);
            else _writer.WritePush(PushSegments.CONSTANT, 0);
            if (value == "true")
            {
                _writer.WriteArithmetic(ArithmeticCommands.NOT);
            }
        }
        else if (NextTokenTypeIs(TokenType.IDENTIFIER))
        {
            var nLocals = 0;
            var isArray = false;
            var nameToken = Advance(); // class name or var name
            switch (_tokens[_index + 1].TokenValue)
            {
                case "[": // array access
                    isArray = true;
                    CompileArrayIndex(nameToken.TokenValue);
                    break;
                case "(":
                    nLocals += 1;
                    _writer.WritePush(PushSegments.POINTER, 0);
                    Advance(); // (
                    nLocals += CompileExpressionList();
                    Advance(); // )
                    _writer.WriteCall($"{_currentClassName}.{nameToken.TokenValue}", nLocals);
                    break;
                case ".":
                    Advance(); // .
                    var callName = "";
                    var subName = Advance(); // subroutine name
                    if (_symbolTable.HasSymbolInEitherScope(nameToken.TokenValue, out _))
                    {
                        SymbolKindToPushStatementIfExists(nameToken.TokenValue);
                        nLocals += 1;
                        callName = $"{_symbolTable.TypeOf(nameToken.TokenValue)}.{subName.TokenValue}";
                    }
                    else
                    {
                        callName = $"{nameToken.TokenValue}.{subName.TokenValue}";
                    }
                    Advance(); // (
                    nLocals += CompileExpressionList();
                    Advance(); // )
                    _writer.WriteCall(callName, nLocals);
                    break;
            }


            if (isArray)
            {
                _writer.WritePop(PopSegments.POINTER, 1);
                _writer.WritePush(PushSegments.THAT, 0);
                //SymbolKindToPushStatementIfExists(nameToken.TokenValue);
            }
            else
            {
                SymbolKindToPushStatementIfExists(_tokens[_index].TokenValue);
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
            var op = Advance(); // unary op symbol
            CompileTerm();
            if (op.TokenValue == "-") _writer.WriteArithmetic(ArithmeticCommands.NEG);
            else if (op.TokenValue == "~") _writer.WriteArithmetic(ArithmeticCommands.NOT);
        }
    }

    private void CompileArrayIndex(string name)
    {
        Advance(); // [
        CompileExpression();
        Advance(); // ]
        SymbolKindToPushStatementIfExists(name);
        _writer.WriteArithmetic(ArithmeticCommands.ADD);
    }

    private int CompileExpressionList()
    {
        var counter = 0;
        if (HasExpression())
        {
            CompileExpression();
            counter += 1;
        }

        while (NextTokenValueIs(","))
        {
            Advance(); // , symbol
            CompileExpression();
            counter += 1;
        }
        return counter;
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
        if (_index == _tokens.Count - 1) return false;
        var nextToken = _tokens[_index+1];
        return nextToken.TokenValue == value;
    }

    private bool NextTokenTypeIs(params TokenType[] tokenTypes)
    {
        if (_index == _tokens.Count - 1) return false;
        var nextToken = _tokens[_index+1];
        return tokenTypes.Any(x => nextToken.TokenType == x);
    }

    private Token Advance()
    {
        _index++;
        var token = _tokens[_index];
        return token;
    }

    private void SymbolKindToPopStatementIfExists(string name)
    {

        if (_symbolTable.HasSymbolInEitherScope(name, out Symbol symbol))
        {
            switch (symbol.Kind)
            {
                case SymbolKind.STATIC:
                    _writer.WritePop(PopSegments.STATIC, _symbolTable.IndexOf(name));
                    break;
                case SymbolKind.ARG:
                    _writer.WritePop(PopSegments.ARGUMENT, _symbolTable.IndexOf(name));
                    break;
                case SymbolKind.VAR:
                    _writer.WritePop(PopSegments.LOCAL, _symbolTable.IndexOf(name));
                    break;
                case SymbolKind.NONE:
                    _writer.WritePop(PopSegments.THIS, _symbolTable.IndexOf(name));
                    break;
                case SymbolKind.FIELD:
                    _writer.WritePop(PopSegments.THIS, _symbolTable.IndexOf(name));
                    break;
            }
        }
    }

    private void SymbolKindToPushStatementIfExists(string name)
    {

        if (_symbolTable.HasSymbolInEitherScope(name, out Symbol symbol))
        {
            switch (symbol.Kind)
            {
                case SymbolKind.STATIC:
                    _writer.WritePush(PushSegments.STATIC, _symbolTable.IndexOf(name));
                    break;
                case SymbolKind.ARG:
                    _writer.WritePush(PushSegments.ARGUMENT, _symbolTable.IndexOf(name));
                    break;
                case SymbolKind.VAR:
                    _writer.WritePush(PushSegments.LOCAL, _symbolTable.IndexOf(name));
                    break;
                case SymbolKind.NONE:
                    _writer.WritePush(PushSegments.THIS, _symbolTable.IndexOf(name));
                    break;
                case SymbolKind.FIELD:
                    _writer.WritePush(PushSegments.THIS, _symbolTable.IndexOf(name));
                    break;
            }
        }
    }
}