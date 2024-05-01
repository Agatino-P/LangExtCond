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

namespace LangExtCond.Tests;
public class TryAndMore
{
    private readonly ITestOutputHelper testOutput;

    public TryAndMore(ITestOutputHelper testOutput )
    {
        this.testOutput = testOutput;
    }
    [Fact]
    public void ShouldUnderstandTry()
    {

        
        var r = new Result<string>("eee");
        Func<Result<string>> f1A()
        {
            return () => new Result<string>("WWW");
        }
        Try<string> f1B()
        {
            return () => new Result<string>("WWW");
        }

        Func<Result<string>> f2A = () => new Result<string>("WWW");
        
        Try<string> f2B() => () => new Result<string>("WWW");

        Try<Unit> f3() => () => Unit.Default;

        //var t=Try<string> FirstTry(string s);
        Try<string> a = f2B();

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

        var st1= r1.IfFail("failed...");
        var st2 = r2.IfSucc((s)=>testOutput.WriteLine($"Success {s}"));
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
