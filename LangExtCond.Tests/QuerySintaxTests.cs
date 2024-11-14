using LanguageExt;
using LanguageExt.ClassInstances.Const;
using static LanguageExt.Prelude;

namespace LangExtCond.Tests;
public class QuerySintaxTests
{
    [Fact]
    public void Test1()
    {
        Option<string> optionA = Some("Hello, ");
        Option<string> optionB = Some("World");
        Option<string> optionNone = None;

        var result1 = from x in optionA
                      from y in optionB
                      select x + y;

        Option<string> result2 = from x in optionA
                      from y in optionB
                      from z in optionNone
                      select x + y + z;

        Option<string> result3 = from x in optionA
                      from y in optionB
                      from w in optionB
                      from z in optionA
                      select x + y + w+ z;



        foreach (var item in optionB)
        {
            int a = 1;
        }

        foreach (var item in optionNone)
        {
            int a = 1;
        }

        var tf = TripleForEach();
    }

    [Fact]
    public void WithEither()
    {
        Either<string, int> e1 = 1;
        Either<string, int> e2 = "e2";
        Either<string, int> e3 = 3; 
        Either<string, int> e4 = "e4";

        var result1 = from x1 in e1
                      from x2 in e2
                      from x3 in e3
                      from x4 in e4
                      select x1 + x2 + x3 + x4;

        var result2 = from x1 in e1
                      from x3 in e2
                      from y1 in f(x1,x3)
                      select x1 + + x3 + y1;

        static Either<string, int> f(int i1, int i2) => i1 * 100 + i2 * 10;

    }


    IEnumerable<string> TripleForEach()
    {
        foreach (string a in new string[] { "1", "2" })
        {
            foreach (string b in new string[] { /*"a", "b" */})
            {
                foreach (string c in new string[] { "A", "B" })
                {
                    yield return a + b + c;
                }
            }
        }
    }
}
