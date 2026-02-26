using System;
using System.Collections.Generic;
using System.Linq;

namespace DevInsight.SOLID.LSP;
// Liskov Substitution Principle (LSP) demo in C#
// The L in SOLID: derived types must be usable through the base type without
// the substituting code knowing the difference.



// Common abstraction: a shape exposes its area.
public interface IShape
{
    double Area { get; }
}

// A rectangle has independent width and height.
public class Rectangle : IShape
{
    public double Width { get; }
    public double Height { get; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double Area => Width * Height;
}

// A square does not inherit from Rectangle because LSP would be broken
// when clients set Width and Height independently. It implements the same
// interface so it can be substituted where IShape is expected.
public class Square : IShape
{
    public double Side { get; }

    public Square(double side)
    {
        Side = side;
    }

    public double Area => Side * Side;
}

// An area calculator operates on the abstraction and thus works for
// any shape that implements IShape.
public static class AreaCalculator
{
    public static double TotalArea(IEnumerable<IShape> shapes)
    {
        return shapes.Sum(s => s.Area);
    }
}

public class Program
{
    public static void Main()
    {
        var shapes = new List<IShape>
            {
                new Rectangle(10, 5),
                new Square(4),
                new Rectangle(3, 2),
                new Square(6)
            };

        double total = AreaCalculator.TotalArea(shapes);
        Console.WriteLine($"Total area: {total}");
    }
}
