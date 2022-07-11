namespace JackCompiler.Core.Symbol_Table;

sealed record Symbol(string Name, string Type, SymbolKind Kind, int Index);