using LanguageExt;

using static LanguageExt.Prelude;

namespace LangExtCond.Tests;
public class Trans
{
    [Fact]
    public async Task ProcessAsync()
    {
        Either<int, string> a1 = "";
        EitherAsync<int, string> a2 = a1.BindAsync(Step1).ToAsync();
        Either<int, string> a31 = await a2.ToEither();

        Either<int, string> r1 = "";
        Either<int, string> r2 = await r1.BindAsync(Step1);
        Either<int, Unit> r3 = await (await r1.BindAsync(Step1)).BindAsync(Step2);

        Either<int, string> t1 = "";
        var t2 = t1.Map(async s => await Step1a(s));

        Task<string> q = Step1a("a");
        Option<Task<string>> ot = Some(q);
        Option<Task<string>> ot2 = ot.MapT<string, string>(s => s.ToUpper());

    }



    private async Task<Either<int, string>> Step1(string s)
    {
        string s2 = await SrvGetString(s);
        return s2;
    }
    


    private async Task<string> Step1a(string s)
    {
        await Task.CompletedTask;
        return s + "..";
    }


    private async Task<Either<int, Unit>> Step2(string s)
    {
        await Task.CompletedTask;
        if (string.IsNullOrWhiteSpace(s))
        {
            return -1;
        }

        return Unit.Default;
    }

    private async Task<EitherAsync<int, string>> SrvEA(string s)
    {
        await Task.CompletedTask;
        return s;
    }

    [Fact]
    private void TestDebug()
    {
        var ea = SrvEA("m");
    }

    private async Task<string> SrvGetString(string s)
    {
        await Task.CompletedTask;
        return s;
    }
}


