namespace LinearNet.Test;

public class VectorTests
{
    [Test]
    public void OperationTest()
    {
        var v1 = new Vector<int>(1, 2, 3);
        var v2 = new Vector<int>(4, 5, 6);
        var v3 = v1 + v2;
        Assert.Multiple(() =>
        {
            Assert.That(v3[0], Is.EqualTo(5));
            Assert.That(v3[1], Is.EqualTo(7));
            Assert.That(v3[2], Is.EqualTo(9));
        });

        var v4 = v1 - v2;
        Assert.Multiple(() =>
        {
            Assert.That(v4[0], Is.EqualTo(-3));
            Assert.That(v4[1], Is.EqualTo(-3));
            Assert.That(v4[2], Is.EqualTo(-3));
        });

        var dot = v1 * v2;
        Assert.That(dot, Is.EqualTo(32));

        var v5 = v1.Cross(v2);
        Assert.Multiple(() =>
        {
            Assert.That(v5[0], Is.EqualTo(-3));
            Assert.That(v5[1], Is.EqualTo(6));
            Assert.That(v5[2], Is.EqualTo(-3));
        });
    }

    [Test]
    public void AttributeTest()
    {
        var v1 = new Vector<double>(1, 2, 3);
        Assert.That(v1, Has.Length.EqualTo(3));
        var norm2 = v1.Norm2();
        Assert.That(norm2, Is.EqualTo(14));
        var norm = v1.Norm();
        Assert.That(norm, Is.EqualTo(Math.Sqrt(14)));
    }
}