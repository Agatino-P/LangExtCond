using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Xunit.Abstractions;
using FluentAssertions;

namespace LangExtCond.Tests;
public class TryAndMore
{
    private readonly ITestOutputHelper testOutput;

    public TryAndMore(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
    }
    [Fact]
    public void ShouldUnderstandTry()
    {

        Func<int, int> ThisThrowsOnOdd = i => i % 2 == 0 ? i : throw new NotImplementedException();


        int a1 = Try(() => ThisThrowsOnOdd(2)).IfFail(-1);
        a1.Should().Be(2);


        int a2 = Try(() => ThisThrowsOnOdd(1)).IfFail(-1);
        a2.Should().Be(-1);

        Func<int, int> WrapperOfThrowing = i => ThisThrowsOnOdd(i);

        int b1 = Try(() => WrapperOfThrowing(2)).IfFail(-1);
        b1.Should().Be(2);

        int b2 = Try(() => WrapperOfThrowing(1)).IfFail(
            ex =>
            {
                testOutput.WriteLine(ex.Message);
                return -1;
            }
            );
        b2.Should().Be(-1);


    }

    [Fact]
    public void ShouldUnderstandNewType()
    {
        Try<UpperCase> o1 = UpperCase.NewTry("UPPER");
        Try<UpperCase> o2 = UpperCase.NewTry("lower");
        Try<UpperCase> o3 = UpperCase.NewTry("ANOTHE_RUPPER");

        Try<string> r1 = from s1 in o1
                         from s2 in o2
                         select s1.Value + s2.Value;

        Try<string> r2 = from s1 in o1
                         from s3 in o3
                         select s1.Value + s3.Value;

        var st1 = r1.IfFail("failed...");
        var st2 = r2.IfSucc((s) => testOutput.WriteLine($"Success {s}"));
    }

    public class UpperCase : NewType<UpperCase, string>
    {
        public UpperCase(string value) : base(value)
        {
            if (value != value.ToUpper(CultureInfo.InvariantCulture))
                throw new ArgumentException(value);
        }
    }
}
