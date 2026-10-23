using NUnit.Framework;

public class LevelTests
{
    LevelProgress 레벨_진행도_생성(int level = 1, int currentExp = 0) => new(level, currentExp);

    int[] 기본_경험치_테이블() => new[] { 100, 200, 300 };

    [Test]
    public void 경험치_증가()
    {
        var progress = 레벨_진행도_생성(1, 0);
        var result = LevelSystem.AddExp(progress, 50, 기본_경험치_테이블());

        Assert.AreEqual(1, result.Level);
        Assert.AreEqual(50, result.CurrentExp);
    }

    [Test]
    public void 레벨당_필요_경험치_달성_시_레벨업()
    {
        var progress = 레벨_진행도_생성(1, 0);
        var result = LevelSystem.AddExp(progress, 120, 기본_경험치_테이블());

        Assert.AreEqual(2, result.Level);
        Assert.AreEqual(20, result.CurrentExp);
    }

    [Test]
    public void 경험치가_많으면_연속으로_레벨업한다()
    {
        var progress = 레벨_진행도_생성(1, 0);
        var result = LevelSystem.AddExp(progress, 350, 기본_경험치_테이블());

        Assert.AreEqual(3, result.Level);
        Assert.AreEqual(50, result.CurrentExp);
    }

    [Test]
    public void 최대_레벨에_도달하면_증가하지_않는다()
    {
        var progress = 레벨_진행도_생성(4, 0);
        var result = LevelSystem.AddExp(progress, 500, 기본_경험치_테이블());

        Assert.AreEqual(4, result.Level);
        Assert.AreEqual(0, result.CurrentExp);
    }
}