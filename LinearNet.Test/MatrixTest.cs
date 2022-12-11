namespace LinearNet.Test;

public class MatrixTests
{
    [Test]
    public void OperationTest()
    {
        var matrix1 = new Matrix<int>(3, 3)
        {
            [0, 0] = 1, [0, 1] = 2, [0, 2] = 3,
            [1, 0] = 4, [1, 1] = 5, [1, 2] = 6,
            [2, 0] = 7, [2, 1] = 8, [2, 2] = 9
        };

        var matrix2 = new Matrix<int>(3, 3)
        {
            [0, 0] = 1, [0, 1] = 2, [0, 2] = 3,
            [1, 0] = 4, [1, 1] = 5, [1, 2] = 6,
            [2, 0] = 7, [2, 1] = 8, [2, 2] = 9
        };

        var matrix3 = matrix1 + matrix2;

        for (var i = 0; i < matrix3.Rows; i++)
        {
            for (var j = 0; j < matrix3.Columns; j++)
            {
                Assert.That(matrix3[i, j], Is.EqualTo(i * 3 + j + 1 + i * 3 + j + 1));
            }
        }

        var matrix4 = matrix1 - matrix2;
        for (var i = 0; i < matrix4.Rows; i++)
        {
            for (var j = 0; j < matrix4.Columns; j++)
            {
                Assert.That(matrix4[i, j], Is.EqualTo(0));
            }
        }

        var matrix5 = matrix1 * matrix2;
        Assert.Multiple(() =>
        {
            Assert.That(matrix5[0, 0], Is.EqualTo(30));
            Assert.That(matrix5[0, 1], Is.EqualTo(36));
            Assert.That(matrix5[0, 2], Is.EqualTo(42));
            Assert.That(matrix5[1, 0], Is.EqualTo(66));
            Assert.That(matrix5[1, 1], Is.EqualTo(81));
            Assert.That(matrix5[1, 2], Is.EqualTo(96));
            Assert.That(matrix5[2, 0], Is.EqualTo(102));
            Assert.That(matrix5[2, 1], Is.EqualTo(126));
            Assert.That(matrix5[2, 2], Is.EqualTo(150));
        });

        var matrix6 = matrix1 * 2;
        for (var i = 0; i < matrix6.Rows; i++)
        {
            for (var j = 0; j < matrix6.Columns; j++)
            {
                Assert.That(matrix6[i, j], Is.EqualTo((i * 3 + j + 1) * 2));
            }
        }

        var matrix7 = matrix1 / 2;
        for (var i = 0; i < matrix7.Rows; i++)
        {
            for (var j = 0; j < matrix7.Columns; j++)
            {
                Assert.That(matrix7[i, j], Is.EqualTo((i * 3 + j + 1) / 2));
            }
        }

        var matrix8 = matrix1.Transpose();
        Assert.Multiple(() =>
        {
            Assert.That(matrix8[0, 0], Is.EqualTo(1));
            Assert.That(matrix8[0, 1], Is.EqualTo(4));
            Assert.That(matrix8[0, 2], Is.EqualTo(7));
            Assert.That(matrix8[1, 0], Is.EqualTo(2));
            Assert.That(matrix8[1, 1], Is.EqualTo(5));
            Assert.That(matrix8[1, 2], Is.EqualTo(8));
            Assert.That(matrix8[2, 0], Is.EqualTo(3));
            Assert.That(matrix8[2, 1], Is.EqualTo(6));
            Assert.That(matrix8[2, 2], Is.EqualTo(9));
        });
    }
}