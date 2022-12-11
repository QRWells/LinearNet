using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace LinearNet;

public readonly struct Vector<T> :
    IAdditionOperators<Vector<T>, Vector<T>, Vector<T>>,
    ISubtractionOperators<Vector<T>, Vector<T>, Vector<T>>,
    IMultiplyOperators<Vector<T>, Vector<T>, T>,
    IMultiplyOperators<Vector<T>, T, Vector<T>>,
    IDivisionOperators<Vector<T>, T, Vector<T>>,
    ICloneable,
    IEquatable<Vector<T>>,
    IEnumerable<T> where T : struct, INumber<T>
{
    public int Size { get; }
    public int Length => Size;

    private readonly T[] _values;

    public bool RowVector { get; } = false;

    public bool IsColumnVector => !RowVector;

    public Vector(int size, bool rowVector = false)
    {
        Size = size;
        _values = new T[size];
        RowVector = rowVector;
    }

    public Vector(T[] values, bool rowVector = false)
    {
        Size = values.Length;
        _values = values;
        RowVector = rowVector;
    }

    public Vector(Vector<T> other)
    {
        Size = other.Size;
        _values = new T[Size];
        Array.Copy(other._values, _values, Size);
        RowVector = other.RowVector;
    }

    /// <summary>
    ///     Creates a column vector.
    /// </summary>
    /// <param name="values"></param>
    public Vector(params T[] values)
    {
        Size = values.Length;
        _values = values;
        RowVector = false;
    }

    public T this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    public static Vector<T> operator +(Vector<T> left, Vector<T> right)
    {
        if (left.Size != right.Size)
            throw new ArgumentException("Vectors must be of the same size");

        var result = new Vector<T>(left.Size);
        for (var i = 0; i < left.Size; i++) result[i] = left[i] + right[i];

        return result;
    }

    public static Vector<T> operator -(Vector<T> left, Vector<T> right)
    {
        if (left.Size != right.Size)
            throw new ArgumentException("Vectors must be of the same size");

        var result = new Vector<T>(left.Size);
        for (var i = 0; i < left.Size; i++) result[i] = left[i] - right[i];

        return result;
    }

    public static T operator *(Vector<T> left, Vector<T> right)
    {
        if (left.Size != right.Size)
            throw new ArgumentException("Vectors must be of the same size");

        var result = default(T);
        for (var i = 0; i < left.Size; i++) result += left[i] * right[i];

        return result;
    }

    public static Vector<T> operator *(Vector<T> left, T right)
    {
        var result = new Vector<T>(left.Size);
        for (var i = 0; i < left.Size; i++) result[i] = left[i] * right;

        return result;
    }

    public T Dot(Vector<T> other)
    {
        return this * other;
    }

    /// <summary>
    /// Normalizes the vector.
    /// </summary>
    /// <returns></returns>
    public Vector<T> Normalized()
    {
        var result = new Vector<T>(this);
        var norm = Norm();
        for (var i = 0; i < Size; i++) result[i] /= norm;

        return result;
    }

    public void Normalize()
    {
        var norm = Norm();
        for (var i = 0; i < Size; i++) this[i] /= norm;
    }

    public T Norm()
    {
        var result = T.Zero;
        for (var i = 0; i < Size; i++) result += _values[i] * _values[i];

        return result.Sqrt();
    }

    public T Norm2()
    {
        var result = T.Zero;
        for (var i = 0; i < Size; i++) result += _values[i] * _values[i];

        return result;
    }

    public Vector<T> AsRowVector()
    {
        return RowVector ? this : new Vector<T>(Size, true);
    }

    public Vector<T> AsColumnVector()
    {
        return IsColumnVector ? this : new Vector<T>(Size);
    }

    public T[] ToArray()
    {
        return _values;
    }

    public object Clone()
    {
        return new Vector<T>(this);
    }


    public Vector<T> Cross(Vector<T> other)
    {
        if (Size != 3 || other.Size != 3)
            throw new ArgumentException("Cross product is only defined for 3D vectors");

        var result = new Vector<T>(3)
        {
            [0] = this[1] * other[2] - this[2] * other[1],
            [1] = this[2] * other[0] - this[0] * other[2],
            [2] = this[0] * other[1] - this[1] * other[0]
        };

        return result;
    }

    public bool Equals(Vector<T> other)
    {
        return _values.Equals(other._values) && Size == other.Size && RowVector == other.RowVector;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Size; i++) yield return this[i];
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_values, Size, RowVector);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(RowVector ? "[" : "[[");
        for (var i = 0; i < Size; i++)
        {
            sb.Append(_values[i].ToString());
            if (i != Size - 1) sb.Append(", ");
        }

        sb.Append(RowVector ? "]" : "]]");
        return sb.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static Vector<T> operator /(Vector<T> left, T right)
    {
        var result = new Vector<T>(left.Size);
        for (var i = 0; i < left.Size; i++) result[i] = left[i] / right;

        return result;
    }
}