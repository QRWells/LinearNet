using System.Numerics;
using System.Text;

namespace LinearNet;

public readonly struct Matrix<T> :
    IAdditiveIdentity<Matrix<T>, Matrix<T>>,
    IAdditionOperators<Matrix<T>, Matrix<T>, Matrix<T>>,
    ISubtractionOperators<Matrix<T>, Matrix<T>, Matrix<T>>,
    IMultiplicativeIdentity<Matrix<T>, Matrix<T>>,
    IMultiplyOperators<Matrix<T>, Matrix<T>, Matrix<T>>,
    IMultiplyOperators<Matrix<T>, Vector<T>, Matrix<T>>,
    IMultiplyOperators<Matrix<T>, T, Matrix<T>>,
    IDivisionOperators<Matrix<T>, T, Matrix<T>>,
    ICloneable, IEquatable<Matrix<T>>
    where T : struct, INumber<T>
{
    public int Rows { get; }
    public int Columns { get; }

    public bool IsSquare => Rows == Columns;

    private readonly T[] _data;

    public Matrix(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _data = new T[rows * columns];
    }

    public Matrix(Vector<T> vector)
    {
        if (vector.IsColumnVector)
        {
            Rows = vector.Length;
            Columns = 1;
        }
        else
        {
            Rows = 1;
            Columns = vector.Length;
        }

        _data = new T[Rows * Columns];
        Array.Copy(vector.ToArray(), _data, vector.Length);
    }

    public Matrix(params Vector<T>[] columns)
    {
        if (columns.Length == 0) throw new ArgumentException("Must have at least one column", nameof(columns));

        // make sure all columns are the same length
        var length = columns[0].Length;
        for (var i = 1; i < columns.Length; i++)
            if (columns[i].Length != length)
                throw new ArgumentException("All columns must be the same length", nameof(columns));

        Rows = length;
        Columns = columns.Length;

        _data = new T[Rows * Columns];
        for (var i = 0; i < Columns; i++)
        for (var j = 0; j < Rows; j++)
            this[j, i] = columns[i][j];
    }

    public T this[int row, int column]
    {
        get => _data[row * Columns + column];
        set => _data[row * Columns + column] = value;
    }

    private Matrix(int attr)
    {
        switch (attr)
        {
            case 0:
                Rows = 0;
                Columns = 1;
                _data = Array.Empty<T>();
                break;
            case -1:
                Rows = 1;
                Columns = 0;
                _data = Array.Empty<T>();
                break;
            default:
                throw new ArgumentException("Invalid attribute");
        }
    }

    private Matrix(Matrix<T> other)
    {
        Rows = other.Rows;
        Columns = other.Columns;
        _data = new T[Rows * Columns];
        Array.Copy(other._data, _data, _data.Length);
    }

    public static Matrix<T> MultiplicativeIdentity { get; } = new(-1);

    public static Matrix<T> operator *(Matrix<T> left, Vector<T> right)
    {
        if (left.Columns != right.Length)
            throw new ArgumentException("Invalid dimensions");

        var result = new Matrix<T>(left.Rows, 1);
        for (var i = 0; i < left.Rows; i++)
        {
            var sum = T.Zero;
            for (var j = 0; j < left.Columns; j++)
                sum += left[i, j] * right[j];
            result[i, 0] = sum;
        }

        return result;
    }

    public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
    {
        if (left.Columns != right.Rows)
            throw new ArgumentException("Invalid matrix dimensions");

        var result = new Matrix<T>(left.Rows, right.Columns);

        for (var i = 0; i < left.Rows; i++)
        for (var j = 0; j < right.Columns; j++)
        {
            T sum = default;
            for (var k = 0; k < left.Columns; k++) sum += left[i, k] * right[k, j];

            result[i, j] = sum;
        }

        return result;
    }

    public static Matrix<T> AdditiveIdentity { get; } = new(0);

    public static Matrix<T> operator +(Matrix<T> left, Matrix<T> right)
    {
        if (left.Rows != right.Rows || left.Columns != right.Columns)
            throw new ArgumentException("Invalid matrix dimensions");

        var result = new Matrix<T>(left.Rows, left.Columns);
        for (var i = 0; i < left.Rows; i++)
        for (var j = 0; j < left.Columns; j++)
            result[i, j] = left[i, j] + right[i, j];

        return result;
    }

    public static Matrix<T> operator -(Matrix<T> left, Matrix<T> right)
    {
        if (left.Rows != right.Rows || left.Columns != right.Columns)
            throw new ArgumentException("Invalid matrix dimensions");

        var result = new Matrix<T>(left.Rows, left.Columns);
        for (var i = 0; i < left.Rows; i++)
        for (var j = 0; j < left.Columns; j++)
            result[i, j] = left[i, j] - right[i, j];

        return result;
    }

    public static implicit operator Matrix<T>(Vector<T> v)
    {
        return new Matrix<T>(v);
    }

    public static implicit operator Matrix<T>(T[,] array)
    {
        var result = new Matrix<T>(array.GetLength(0), array.GetLength(1));
        for (var i = 0; i < result.Rows; i++)
        for (var j = 0; j < result.Columns; j++)
            result[i, j] = array[i, j];
        return result;
    }

    public T Determinant()
    {
        if (!IsSquare)
            throw new InvalidOperationException("Matrix must be square");

        switch (Rows)
        {
            case 1:
                return this[0, 0];
            case 2:
                return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
        }

        T sum = default;
        for (var i = 0; i < Columns; i++)
        {
            var sign = i % 2 == 0 ? T.One : -T.One;
            sum += sign * this[0, i] * Minor(0, i).Determinant();
        }

        return sum;
    }

    public Matrix<T> Minor(int row, int column)
    {
        if (!IsSquare)
            throw new InvalidOperationException("Matrix must be square");

        var result = new Matrix<T>(Rows - 1, Columns - 1);
        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Columns; j++)
        {
            if (i == row || j == column) continue;
            var r = i < row ? i : i - 1;
            var c = j < column ? j : j - 1;
            result[r, c] = this[i, j];
        }

        return result;
    }

    public int Rank()
    {
        var result = 0;
        var m = this;
        while (m is { Rows: > 0, Columns: > 0 })
        {
            if (m[0, 0].Equals(T.Zero))
            {
                var found = false;
                for (var i = 1; i < m.Rows; i++)
                {
                    if (m[i, 0].Equals(T.Zero)) continue;
                    m = m.SwapRows(0, i);
                    found = true;
                    break;
                }

                if (!found)
                {
                    m = m.SubMatrix(0, 1, m.Rows, m.Columns - 1);
                    continue;
                }
            }

            for (var i = 1; i < m.Rows; i++)
            {
                var factor = m[i, 0] / m[0, 0];
                for (var j = 0; j < m.Columns; j++)
                    m[i, j] -= factor * m[0, j];
            }

            m = m.SubMatrix(0, 1, m.Rows, m.Columns - 1);
            result++;
        }

        return result;
    }

    public Matrix<T> SubMatrix(int row, int column, int rows, int columns)
    {
        var result = new Matrix<T>(rows, columns);
        for (var i = 0; i < rows; i++)
        for (var j = 0; j < columns; j++)
            result[i, j] = this[row + i, column + j];
        return result;
    }

    public Matrix<T> SwapRows(int row1, int row2)
    {
        var result = new Matrix<T>(this);
        for (var i = 0; i < Columns; i++) (result[row1, i], result[row2, i]) = (result[row2, i], result[row1, i]);

        return result;
    }

    public object Clone()
    {
        return new Matrix<T>(this);
    }

    public static Matrix<T> operator *(Matrix<T> left, T right)
    {
        var result = new Matrix<T>(left.Rows, left.Columns);
        for (var i = 0; i < left.Rows; i++)
        for (var j = 0; j < left.Columns; j++)
            result[i, j] = left[i, j] * right;
        return result;
    }

    public Matrix<T> Transpose()
    {
        var result = new Matrix<T>(Columns, Rows);
        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Columns; j++)
            result[j, i] = this[i, j];
        return result;
    }

    public bool Equals(Matrix<T> other)
    {
        return _data.Equals(other._data) && Rows == other.Rows && Columns == other.Columns;
    }

    public override bool Equals(object? obj)
    {
        return obj is Matrix<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_data, Rows, Columns);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
                sb.Append($"{this[i, j]} ");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static Matrix<T> operator /(Matrix<T> left, T right)
    {
        var result = new Matrix<T>(left.Rows, left.Columns);
        for (var i = 0; i < left.Rows; i++)
        for (var j = 0; j < left.Columns; j++)
            result[i, j] = left[i, j] / right;
        return result;
    }
}