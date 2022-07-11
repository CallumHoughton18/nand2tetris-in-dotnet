namespace JackCompiler.Core.Symbol_Table;

sealed class SymbolTable
{
    // If a symbol is not present in the class level or subroutine level tables
    // then this MUST be either a class name, or a subroutine name
    // this should be handled at the compiler level
    
    private readonly Dictionary<string, Symbol> _classLevelDictionary = new();
    private Dictionary<string, Symbol>? _subRoutineSymbolDictionary;

    public void Define(string name, string type, SymbolKind kind)
    {
        if (kind == SymbolKind.FIELD || kind == SymbolKind.STATIC)
        {
            _classLevelDictionary[name] = new Symbol(name, type, kind, GetNewIndex(_classLevelDictionary, kind));
        }
        else if (_subRoutineSymbolDictionary is not null)
        {
            _subRoutineSymbolDictionary[name] = new Symbol(name, type, kind, GetNewIndex(_subRoutineSymbolDictionary, kind));
        }
    }

    private int GetNewIndex(Dictionary<string, Symbol> table, SymbolKind kind)
    {
        return table.Values.Count(x => x.Kind == kind);
    }

    public void StartSubRoutine()
    {
        if (_subRoutineSymbolDictionary is not null) _subRoutineSymbolDictionary.Clear();
        else _subRoutineSymbolDictionary = new();
    }
    
    public int VarCount(SymbolKind kind)
    {
        if (kind == SymbolKind.FIELD || kind == SymbolKind.STATIC)
        {
            return _classLevelDictionary.Values.Count(x => x.Kind == kind);
        }

        if (_subRoutineSymbolDictionary is not null)
        {
            return _subRoutineSymbolDictionary.Values.Count(x => x.Kind == kind);
        }
        throw new InvalidOperationException("Subroutine Symbol Table must be initiated first");
    }

    public SymbolKind KindOf(string name) => GetSymbolFromEitherSymbolTable(name).Kind;
    public string TypeOf(string name) => GetSymbolFromEitherSymbolTable(name).Type;
    public int IndexOf(string name) => GetSymbolFromEitherSymbolTable(name).Index;
    
    private Symbol GetSymbolFromEitherSymbolTable(string name)
    {
        if (_subRoutineSymbolDictionary is not null && _subRoutineSymbolDictionary.ContainsKey(name))
        {
            return _subRoutineSymbolDictionary[name];
        }

        return _classLevelDictionary[name];
    }

    public bool HasSymbolInEitherScope(string name, out Symbol symbol)
    {
        if (_subRoutineSymbolDictionary.ContainsKey(name))
        {
            symbol = _subRoutineSymbolDictionary[name];
            return true;
        }
        else if (_classLevelDictionary.ContainsKey(name))
        {
            symbol = _classLevelDictionary[name];
            return true;
        }

        symbol = null;
        return false;
    }
}