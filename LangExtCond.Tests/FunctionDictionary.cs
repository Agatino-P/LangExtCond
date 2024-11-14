using LanguageExt.ClassInstances;
using LanguageExt;
using LanguageExt.Common;
using Xunit.Sdk;
using ErrorOrInt = LanguageExt.Either<LanguageExt.Common.Error, int>;

namespace LangExtCond.Tests;
public class FunctionDictionaryTest
{
    readonly FunctionDictionary _sut = new();

    [Fact]
    public void ShouldApplySuccessfulFunction()
    {
        string arg = "AAA";
        int expectedOutcome = 6;

        ErrorOrInt actual = _sut.DecodeAndApply("K2", arg);

        actual.IsRight.Should().BeTrue();
        actual.Match(
            len => len.Should().Be(expectedOutcome),
            _ => false.Should().BeTrue()
            );
    }

    [Fact]
    public void ShouldApplyFailingFunction()
    {
        string arg = "AAA";

        ErrorOrInt actual = _sut.DecodeAndApply("K3", arg);

        actual.IsLeft.Should().BeTrue();
        actual.Match(
            _ => false.Should().BeTrue(),
            error => error.Message.Should().Be(FunctionDictionary.FunctionalFail)
            );
    }

    [Fact]
    public void ShouldNotFindKey()
    {
        string arg = "AAA";

        ErrorOrInt actual = _sut.DecodeAndApply("KK", arg);

        actual.IsLeft.Should().BeTrue();
        actual.Match(
            _ => false.Should().BeTrue(),
            error => error.Message.Contains("Enum").Should().BeTrue()
            );
    }

    [Fact]
    public void ShouldNotFindFunction()
    {
        string arg = "AAA";

        ErrorOrInt actual = _sut.DecodeAndApply("K4", arg);

        actual.IsLeft.Should().BeTrue();
        actual.Match(
            _ => false.Should().BeTrue(),
            error => error.Message.Contains("map").Should().BeTrue()
            );
    }

}

public enum Key { K1, K2, K3, K4 };

public class FunctionDictionary : intResult
{
    public const string FunctionalFail = "FunctionalFail";

    delegate ErrorOrInt FailableFunction(string s);

    private FailableFunction FF1 = (string s) => s.Length;
    private FailableFunction FF2 = (string s) =>
    {
        return s.Length * 2;
    };

    private static ErrorOrInt FF3(string s)
    {
        return Error.New(FunctionalFail);
    }

    public ErrorOrInt DecodeAndApplyWithoutExtensions(string keyString, string arg)
    {
        Either<Error, Key> TryParseKey(string keyString) =>
            Enum.TryParse<Key>(keyString, out Key keyResult) ? Either<Error, Key>.Right(keyResult) : Error.New("InvalidKeyString");

        Map<Key, FailableFunction> functionsMap = new([
            (Key.K1, FF1),
            (Key.K2, FF2),
            (Key.K3, FF3)
         ]);

        Either<Error, FailableFunction> TryFindFunction(Key key) =>
         functionsMap.Find(key).ToEither(Error.New("CantFindFunction"));

        ErrorOrInt maybeInt1 =
                TryParseKey(keyString)
                .Bind(TryFindFunction)
                .Bind(function => function(arg));

        ErrorOrInt maybeInt =
            from key in TryParseKey(keyString)
            from function in TryFindFunction(key)
            from result in function(arg)
            select result;

        return maybeInt;
    }

    public ErrorOrInt DecodeAndApply(string keyString, string arg)
    {
        Map<Key, FailableFunction> functionsMap = new([
            (Key.K1, FF1),
            (Key.K2, FF2),
            (Key.K3, FF3)
            ]);

        ErrorOrInt errorOrInt =
            from functionsMapKey in EnumFunctionalHelper.ErrorOrParse<Key>(keyString)
            from failableFunction in functionsMap.ErrorOrValue(functionsMapKey)
            from intResult in failableFunction(arg)
            select intResult;

        return errorOrInt;
    }
}

public static class EnumFunctionalHelper
{
    public static Either<Error, TEnum> ErrorOrParse<TEnum>(string enumString) where TEnum : struct, Enum =>
        Enum.TryParse(enumString, out TEnum enumValue)
        ? enumValue
        : Error.New($"Value: {enumString} not found in Enum type: {typeof(TEnum).Name}");
}

public static class MapExtensions
{
    public static Either<Error, TMapValue> ErrorOrValue<TMapKey, TMapValue>(
            this Map<TMapKey, TMapValue> map,
            TMapKey key,
            string? errorMessage = null) =>
        map.Find(key).ToEither(Error.New(errorMessage ?? $"Value: {key} not found in the map"));
}