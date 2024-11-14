using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;
using static LanguageExt.Prelude;

namespace LangExtCond.Tests;

public class Playaround
{
    private readonly ITestOutputHelper testOutput;


    public Playaround(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
    }


    private async Task<Either<string, int>> SomeAsyncService(int i)
    {
        await Task.CompletedTask;
        Either<string, int> either = i < 0 ? Prelude.Left<string, int>("<0") : Prelude.Right(i);
        return either;
    }

    [Fact]
    public async Task FromTaskToAsync()
    {
        await Task.CompletedTask;

        Task<Either<string, int>> serviceReturn = SomeAsyncService(2);

        EitherAsync<string, int> ea = serviceReturn.ToAsync();
        string s = await ea.Match(i => i.ToString(), s => s);
    }


    [Fact]
    public void ListSelectMany()
    {
        List<int> li = [1, 2, 3];
        List<string> lism = li.SelectMany(i => new[] { i.ToString(), (i * i).ToString(), (i * i * i).ToString() }).ToList();
    }

    [Fact]
    public void CondToOption()
    {
        Option<int> o2 = 2.ToOption(i => i > 2);
        Option<int> o3 = 3.ToOption(i => i > 2);

    }

    [Fact]
    public void LinqExp()
    {
        EitherRight<int> r = (
            from x1 in Right(1)
            from x2 in Right(2)
            select x1 + x2
        );



    }

    [Fact]
    public void ShouldCollectValidations()
    {
        Func<int, Validation<Error, int>> BiggerThan1 = (n) => n > 1 ? Success<Error, int>(n) : Fail<Error, int>(Error.New("<=1"));
        Func<int, Validation<Error, int>> SmallerThan10 = (n) => n < 10 ? Success<Error, int>(n) : Fail<Error, int>(Error.New(">=10"));
        Func<int, Validation<Error, int>> Even= (n) => n%2==0 ? Success<Error, int>(n) : Fail<Error, int>(Error.New("Odd"));

        int number = 11;
        Validation<Error, int> validationResult = BiggerThan1(number) | SmallerThan10(number) | Even(number);

        testOutput.WriteLine("--1--");
        testOutput.WriteLine(validationResult.Match(
            i => i.ToString(), 
            E => string.Join(", ", E.Map(e => e.Message))
            ));

        testOutput.WriteLine("--2--");
        testOutput.WriteLine(validationResult.Match(
            i => i.ToString(),
            E => E.Map(e=>e.Message).Aggregate((a, s) => $"{a}, {s}")
            ));

        testOutput.WriteLine("--3--"); 
        testOutput.WriteLine(validationResult.Match(
            i => i.ToString(),
            E => E.Map(e => e.Message).Skip(1).Aggregate(new StringBuilder(E.First().Message),(sb, s) => sb.Append(", ").Append(s)).ToString()
            ));



    }


}

public static class FuncExt
{
    public static Option<T> ToOption<T>(this T t)
    {
        Option<T> o = t;
        return o;
    }

    public static Task<Either<TLeft, TRight1>> MapT<TLeft, TRight, TRight1>(this Task<Either<TLeft, TRight>> @this, Func<TRight, TRight1> f) =>
        @this.ToAsync().Map(f).ToEither();

    public static Option<T> ToOption<T>(this T @this, Func<T, bool> f) =>
        f(@this) ? Some(@this) : None;
}

public class Id<T>
{
    public T Value { get; set; }
    public Id(T value) => this.Value = value;
    public Id<T1> Select<T1>(Func<T, T1> f) => new Id<T1>(f(Value));
}
