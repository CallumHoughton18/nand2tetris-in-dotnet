using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core.Parser.Grammar;


sealed class ClassGrammar : BaseGrammar
{
    public ClassGrammar() : base( "class")
    {
    }
    
    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        string[] subroutineDecTokenVals = { "constructor", "function", "method" };
        if (subroutineDecTokenVals.Any(x => x == token.TokenValue))
        {
            return (true, new SubRoutineDecGrammar(), true);
        }

        return (false, null, null);
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == "}",  true);
    }
}

sealed class SubRoutineDecGrammar : BaseGrammar
{
    public SubRoutineDecGrammar() : base("subroutineDec")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        if (token.TokenValue == "(")
        {
            return (true, new ParameterListGrammar(), false);
        }
        else if (token.TokenValue == "{")
        {
            return (true, new SubRoutineBody(), true);
        }

        return (false, null, null)!;
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == "}", true);
    }
}

sealed class SubRoutineBody : BaseGrammar
{
    public SubRoutineBody() : base("subroutineBody")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        string[] statementBeginnings = { "let", "if", "while", "do", "return" };
        if (token.TokenValue == "var")
        {
            return (true, new VarDec(), true);
        }
        else if (statementBeginnings.Any(x => x == token.TokenValue))
        {
            return (true, new StatementsGrammar(), true);
        }

        return (false, null, null);
    }

    public override (bool,bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == "}", true);
    }
}

sealed class VarDec : BaseGrammar
{
    public VarDec() : base("varDec")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        return (false, null, null);
    }

    public override (bool,bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == ";", true);
    }
}

sealed class ParameterListGrammar : BaseGrammar
{
    public ParameterListGrammar() : base("parameterList")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        return (false, null, null)!;
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == ")", false);
    }
}

sealed class StatementsGrammar: BaseGrammar
{
    string[] _statementBeginnings = { "let", "if", "while", "do", "return" };

    public StatementsGrammar() : base("statements")
    {
        ShouldAdvanceToNextToken = false;
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        return token.TokenValue switch
        {
            "let" => (true, new LetStatementGrammar(), true),
            "if" => (true, new IfStatementGrammar(), true),
            "while" => (true, new WhileStatementGrammar(), true),
            "do" => (true, new DoStatementGrammar(), true),
            "return" => (true, new ReturnStatementGrammar(), true),
            _ => (false, null, false)
        };
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (_statementBeginnings.All(x => x != token.TokenValue), true);
    }
}

sealed class LetStatementGrammar : BaseGrammar
{
    public LetStatementGrammar() : base("letStatement")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        return (false, null, null);
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == ";", true);
    }
}

sealed class IfStatementGrammar : BaseGrammar
{
    public IfStatementGrammar() : base("ifStatement")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        if (token.TokenValue == "{")
        {
            return (true, new StatementsGrammar(), false);
        }
        return (false, null, null);
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == "}", true);
    }
}

sealed class WhileStatementGrammar : BaseGrammar
{
    public WhileStatementGrammar() : base("whileStatement")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        if (token.TokenValue == "{")
        {
            return (true, new StatementsGrammar(), false);
        }
        return (false, null, null);
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == "}", true);
    }
}

sealed class DoStatementGrammar : BaseGrammar
{
    public DoStatementGrammar() : base("doStatement")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        return (false, null, null);
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == ";", true);
    }
}

sealed class ReturnStatementGrammar : BaseGrammar
{
    public ReturnStatementGrammar() : base("returnStatement")
    {
    }

    public override (bool, BaseGrammar?, bool?) IsNewChild(Token token)
    {
        return (false, null, null);
    }

    public override (bool, bool) IsEndOfGrammar(Token token)
    {
        return (token.TokenValue == ";", true);
    }
}

abstract class BaseGrammar
{
    public string Name { get; }

    public bool ShouldAdvanceToNextToken { get; protected set; } = true;

    public BaseGrammar(string name)
    {
        Name = name;
    }

    public abstract (bool, BaseGrammar?, bool?) IsNewChild(Token token);
    public abstract (bool, bool) IsEndOfGrammar(Token token);
}