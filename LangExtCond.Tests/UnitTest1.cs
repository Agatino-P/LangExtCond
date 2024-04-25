using LanguageExt;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace LangExtCond.Tests;

public class CondTests
{
    private readonly ITestOutputHelper testOutput;

    public record DueNumeri(int First, int Second);

    public CondTests(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
    }

    [Fact]
    public void Test1()
    {
        Either<string, Unit> SecondValidation(DueNumeri dn) => dn.Second < 5 ? $"{dn.Second}" : Unit.Default;

        Func<DueNumeri, Either<string, Unit>> ConditionalCase =
            Prelude.Cond<DueNumeri>(dn => dn.First > 5)
            .Then(SecondValidation)
            .Else(_ => Unit.Default);

        testOutput.WriteLine("Left from beginning");
        Either<string, DueNumeri>.Left("Left from beginning")
            .Bind(ConditionalCase)
            .Match(
                _ => testOutput.WriteLine("Unit"),
                s => testOutput.WriteLine(s)
            );

        testOutput.WriteLine("(1,1)");
        Either<string, DueNumeri>.Right(new(1, 1))
            .Bind(ConditionalCase)
            .Match(
                _ => testOutput.WriteLine("Unit"),
                s => testOutput.WriteLine(s)
            );

        testOutput.WriteLine("(1,6)");
        Either<string, DueNumeri>.Right(new(1, 6))
            .Bind(ConditionalCase)
            .Match(
                _ => testOutput.WriteLine("Unit"),
                s => testOutput.WriteLine(s)
            );

        testOutput.WriteLine("(6,1)");
        Either<string, DueNumeri>.Right(new(6, 1))
            .Bind(ConditionalCase)
            .Match(
                _ => testOutput.WriteLine("Unit"),
                s => testOutput.WriteLine(s)
            );


        testOutput.WriteLine("(6,6)");
        Either<string, DueNumeri>.Right(new(6, 6))
            .Bind(ConditionalCase)
            .Match(
                _ => testOutput.WriteLine("Unit"),
                s => testOutput.WriteLine(s)
            );

        testOutput.WriteLine("(6,6) Prelude in Bind");
        Either<string, DueNumeri>.Right(new(6, 6))
            .Bind(
                Prelude.Cond<DueNumeri>(dn => dn.First > 5)
                    .Then(SecondValidation)
                    .Else(_ => Unit.Default)
                )
            .Match(
                _ => testOutput.WriteLine("Unit"),
                s => testOutput.WriteLine(s)
            );

        testOutput.WriteLine("(6,6) Prelude in Bind and Second in Then");
        Either<string, DueNumeri>.Right(new(6, 6))
            .Bind(
                Cond<DueNumeri>(dn => dn.First > 5)
                    .Then<DueNumeri,Either<string,Unit>>(dn => dn.Second < 5 ? $"{dn.Second}" : Unit.Default)
                    .Else(_ => Unit.Default)
                )
            .Match(
                _ => testOutput.WriteLine("Unit"),
                s => testOutput.WriteLine(s)
                );



    }
}