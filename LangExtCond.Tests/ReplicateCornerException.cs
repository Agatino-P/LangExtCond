using LanguageExt;
using System.Reflection.Metadata.Ecma335;

namespace LangExtCond.Tests;
public class ReplicateCornerException
{
    [Fact]
    public void ShouldNotExplode()
    {
        Explosive e1 = new Explosive("Nums", "Two");

        Numbers n = e1.MayExplode();
        n.Should().Be(Numbers.Two);
    }

    [Fact]
    public void ShouldExplodeByKey()
    {
        Explosive e1 = new Explosive("NoNums", "Two");

        Func<Numbers> thisExplodes = () => e1.MayExplode();
        thisExplodes.Should().Throw<Exception>();
    }

    [Fact]
    public void ShouldExplodeByParse()
    {
        Explosive e1 = new Explosive("Nums", "NA");

        Func<Numbers> thisExplodes = () => e1.MayExplode();
        thisExplodes.Should().Throw<Exception>();
    }

    [Fact]
    public void ShouldRight()
    {
        Explosive e1 = new Explosive("Nums", "Two");

        Func<Either<string, Numbers>> thisShouldNotExplode = () => e1.WillNotExplode();
        thisShouldNotExplode().Match(
            n => n.Should().Be(Numbers.Two),
            _ => false.Should().BeTrue()
            );
    }

    [Fact]
    public void ShouldLeftByKey()
    {
        Explosive e1 = new Explosive("NoNums", "Two");

        Func<Either<string, Numbers>> thisShouldNotExplode = () => e1.WillNotExplode();
        thisShouldNotExplode.Should().NotThrow<Exception>();
    }

    [Fact]
    public void ShouldLeftByParse()
    {
        Explosive e1 = new Explosive("Nums", "NA");

        Func<Either<string, Numbers>> thisShouldNotExplode = () => e1.WillNotExplode();
        thisShouldNotExplode.Should().NotThrow<Exception>();
    }

    [Fact]
    public void ShouldLeftByKeyBis()
    {
        Explosive e1 = new Explosive("NoNums", "Two");

        Func<Either<string, Numbers>> thisShouldNotExplodeBis = () => e1.WillNotExplodeBis();
        thisShouldNotExplodeBis.Should().NotThrow<Exception>();
    }

    [Fact]
    public void ShouldLeftByParseBis()
    {
        Explosive e1 = new Explosive("Nums", "NA");

        Func<Either<string, Numbers>> thisShouldNotExplodeBis = () => e1.WillNotExplodeBis();
        thisShouldNotExplodeBis.Should().NotThrow<Exception>();
    }

    [Fact]
    public void ShouldLeftByKeyTer()
    {
        Explosive e1 = new Explosive("NotNums", "Two");

        Either<string, Numbers> actual = e1.WillNotExplodeTer();
        
        actual.Match(
            _=>false.Should().BeTrue(),
            s=> s.Should().Be("Key failed")
            );
    }

    [Fact]
    public void ShouldLeftByParseTer()
    {
        Explosive e1 = new Explosive("Nums", "NA");

        Either<string, Numbers> actual = e1.WillNotExplodeTer();

        actual.Match(
            _ => false.Should().BeTrue(),
            s => s.Should().Be("Value failed")
            );
    }


}

public class Explosive
{

    public Dictionary<string, object> Dict { get; set; } = new();

    public Explosive(string key, string val)
    {
        Dict.Add(key, val);
    }

    public Numbers MayExplode()
    {
        string numStr = (Dict["Nums"] as string)!;
        Numbers num = Enum.Parse<Numbers>(numStr);
        return num;

    }

    public Either<string, Numbers> WillNotExplode() =>
        Try(MayExplode).ToEither(ex => "failed");


    public Either<string, Numbers> WillNotExplodeBis() =>
        Try(() =>
        {
            string numStr = (Dict["Nums"] as string)!;
            Numbers num = Enum.Parse<Numbers>(numStr);
            return num;

        })
        .ToEither(ex => "failed");

    public Either<string, Numbers> WillNotExplodeTer()
    {
        Either<string, Numbers> either = 
            GetValueFromDict()
            .Bind(ParseValue);

        return either;

        Either<string, string> GetValueFromDict() =>
            Try(() => (Dict["Nums"] as string)!)
            .ToEither(ex => "Key failed");
        Either<string, Numbers> ParseValue(string numStr) =>
            Try(() => Enum.Parse<Numbers>(numStr))
            .ToEither(ex => "Value failed");
    }


}


public enum Numbers
{
    One, Two, Three
}
