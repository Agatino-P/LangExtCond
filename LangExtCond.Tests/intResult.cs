using LanguageExt.Common;

namespace LangExtCond.Tests;
public interface intResult
{
    Either<Error, int> DecodeAndApply(string keyString, string arg);
}