using System;
using System.Collections.Generic;
using System.Linq;

// Класс, представляющий запись в R-дереве, которая хранит узел и его индекс в родительском узле
class Entry
{
    public Node Child { get; }
    public int Index { get; }

    public Entry(Node child, int index)
    {
        Child = child;
        Index = index;
    }
}

// Класс, представляющий точку в n-мерном пространстве
class Point
{
    public double[] Coordinates { get; }

    public Point(params double[] coordinates)
    {
        Coordinates = coordinates;
    }
}

// Класс, представляющий прямоугольник в n-мерном пространстве
class Rectangle
{
    public Point MinPoint { get; }
    public Point MaxPoint { get; }

    public Rectangle(Point minPoint, Point maxPoint)
    {
        MinPoint = minPoint;
        MaxPoint = maxPoint;
    }

    // Проверяет, пересекается ли данный прямоугольник с другим
    public bool Intersects(Rectangle other)
    {
        for (int i = 0; i < MinPoint.Coordinates.Length; i++)
        {
            if (MinPoint.Coordinates[i] > other.MaxPoint.Coordinates[i] || MaxPoint.Coordinates[i] < other.MinPoint.Coordinates[i])
                return false;
        }
        return true;
    }
}

// Класс, представляющий узел R-дерева
class Node
{
    public Rectangle Mbr { get; }
    public List<Node> Children { get; }

    public Node(Rectangle mbr)
    {
        Mbr = mbr;
        Children = new List<Node>();
    }

    // Добавляет дочерний узел
    public void AddChild(Node child)
    {
        Children.Add(child);
    }
}

// Класс, представляющий R-дерево
class RTree
{
    private readonly int _maxChildren;
    private Node _root;
    public Node Root { get { return _root; } }

    public RTree(int maxChildren)
    {
        _maxChildren = maxChildren;
        _root = null;
    }

    // Метод для добавления объекта в R-дерево
    // Метод для добавления объекта в R-дерево
    public void Insert(Point point)
    {
        if (_root == null)
        {
            _root = new Node(new Rectangle(point, point));
            return;
        }

        InsertRecursive(_root, point);
    }

    // Рекурсивный метод для вставки объекта в R-дерево
    private void InsertRecursive(Node node, Point point)
    {
        if (node.Children.Count == 0)
        {
            node.AddChild(new Node(new Rectangle(point, point)));
            if (node.Children.Count > _maxChildren)
            {
                SplitNode(node);
            }
            return;
        }

        double minEnlargement = double.MaxValue;
        Node selectedChild = null;

        foreach (var child in node.Children)
        {
            Rectangle enlarged = EnlargeRectangle(child.Mbr, point);
            double enlargement = CalculateEnlargement(child.Mbr, enlarged);
            if (enlargement < minEnlargement)
            {
                minEnlargement = enlargement;
                selectedChild = child;
            }
        }

        InsertRecursive(selectedChild, point);
    }

    // Выбор листового узла для вставки объекта
    private Node ChooseLeaf(Node node, Point point)
    {
        if (node.Children.Count == 0)
            return node;

        double minEnlargement = double.MaxValue;
        Node selectedChild = null;

        foreach (var child in node.Children)
        {
            Rectangle enlarged = EnlargeRectangle(child.Mbr, point);
            double enlargement = CalculateEnlargement(child.Mbr, enlarged);
            if (enlargement < minEnlargement)
            {
                minEnlargement = enlargement;
                selectedChild = child;
            }
        }

        return ChooseLeaf(selectedChild, point);
    }

    // Разделение узла
    private void SplitNode(Node node)
    {
        // Implementation of node split goes here
    }

    // Вычисление увеличения прямоугольника
    private double CalculateEnlargement(Rectangle original, Rectangle enlarged)
    {
        double originalArea = CalculateArea(original);
        double enlargedArea = CalculateArea(enlarged);
        return enlargedArea - originalArea;
    }

    // Вычисление площади прямоугольника
    private double CalculateArea(Rectangle rect)
    {
        double area = 1.0;
        for (int i = 0; i < rect.MinPoint.Coordinates.Length; i++)
        {
            area *= rect.MaxPoint.Coordinates[i] - rect.MinPoint.Coordinates[i];
        }
        return area;
    }

    // Увеличение прямоугольника, чтобы вместить точку
    private Rectangle EnlargeRectangle(Rectangle rect, Point point)
    {
        double[] minCoords = new double[rect.MinPoint.Coordinates.Length];
        double[] maxCoords = new double[rect.MaxPoint.Coordinates.Length];

        for (int i = 0; i < minCoords.Length; i++)
        {
            minCoords[i] = Math.Min(rect.MinPoint.Coordinates[i], point.Coordinates[i]);
            maxCoords[i] = Math.Max(rect.MaxPoint.Coordinates[i], point.Coordinates[i]);
        }

        return new Rectangle(new Point(minCoords), new Point(maxCoords));
    }

    // Метод для удаления объекта из R-дерева
    public void Remove(Point point)
    {
        RemoveRecursive(_root, point);
    }

    private bool RemoveRecursive(Node node, Point point)
    {
        // Если узел - лист
        if (node.Children.Count == 0)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                if (node.Children[i].Mbr.MinPoint.Equals(point))
                {
                    node.Children.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        // Если узел - не лист
        foreach (var child in node.Children)
        {
            if (RemoveRecursive(child, point))
            {
                // После удаления, проверяем, нужно ли объединить дочерние узлы
                if (child.Children.Count < _maxChildren / 2)
                {
                    // Реализация объединения дочерних узлов
                }
                return true;
            }
        }

        return false;
    }

    // Метод для поиска по точному совпадению
    public bool SearchExact(Point point)
    {
        return SearchExactRecursive(_root, point);
    }

    private bool SearchExactRecursive(Node node, Point point)
    {
        foreach (var child in node.Children)
        {
            if (child.Mbr.MinPoint.Equals(point))
                return true;

            if (SearchExactRecursive(child, point))
                return true;
        }

        return false;
    }

    // Метод для поиска по региону (диапазонный поиск)
    public List<Point> SearchRegion(Rectangle region)
    {
        List<Point> result = new List<Point>();
        SearchRegionRecursive(_root, region, result);
        return result;
    }

    private void SearchRegionRecursive(Node node, Rectangle region, List<Point> result)
    {
        foreach (var child in node.Children)
        {
            if (child.Mbr.Intersects(region))
            {
                if (child.Children.Count == 0)
                {
                    foreach (var point in child.Children.Select(c => c.Mbr.MinPoint))
                    {
                        if (region.Intersects(new Rectangle(point, point)))
                            result.Add(point);
                    }
                }
                else
                {
                    SearchRegionRecursive(child, region, result);
                }
            }
        }
    }

    // Метод для вычисления расстояния между двумя точками
    private double CalculateDistance(Point point1, Point point2)
    {
        double sum = 0;
        for (int i = 0; i < point1.Coordinates.Length; i++)
        {
            sum += Math.Pow(point1.Coordinates[i] - point2.Coordinates[i], 2);
        }
        return Math.Sqrt(sum);
    }

    // Метод для поиска ближайшего соседа
    public Point FindNearestNeighbor(Point point)
    {
        return FindNearestNeighborRecursive(_root, point, null, double.MaxValue);
    }

    private Point FindNearestNeighborRecursive(Node node, Point point, Point bestPoint, double bestDistance)
    {
        foreach (var child in node.Children)
        {
            double currentDistance = CalculateDistance(child.Mbr.MinPoint, point);
            if (currentDistance < bestDistance)
            {
                bestDistance = currentDistance;
                bestPoint = child.Mbr.MinPoint;
            }

            if (node.Children.Count == 0)
                continue;

            double childDistance = CalculateDistance(child.Mbr.MinPoint, point);
            if (childDistance < bestDistance)
            {
                bestPoint = FindNearestNeighborRecursive(child, point, bestPoint, bestDistance);
                bestDistance = CalculateDistance(bestPoint, point);
            }
        }

        return bestPoint;
    }
}

class Program
{
    static void Main(string[] args)
    {
        RTree rTree = new RTree(maxChildren: 4);

        // Добавляем точки
        rTree.Insert(new Point(2, 3));
        rTree.Insert(new Point(4, 2));
        rTree.Insert(new Point(1, 1));
        rTree.Insert(new Point(5, 4));
        rTree.Insert(new Point(3, 5));

        // Поиск по точному совпадению
        Point searchPoint = new Point(3, 5);
        bool found = rTree.SearchExact(searchPoint);
        Console.WriteLine($"Точка {searchPoint.Coordinates[0]}, {searchPoint.Coordinates[1]} найдена: {found}");

        // Поиск по региону
        Rectangle searchRegion = new Rectangle(new Point(1, 1), new Point(3, 3));
        List<Point> pointsInRegion = rTree.SearchRegion(searchRegion);
        Console.WriteLine($"Точки внутри региона {searchRegion.MinPoint.Coordinates[0]}, {searchRegion.MinPoint.Coordinates[1]} - {searchRegion.MaxPoint.Coordinates[0]}, {searchRegion.MaxPoint.Coordinates[1]}:");
        foreach (var point in pointsInRegion)
        {
            Console.WriteLine($"- {point.Coordinates[0]}, {point.Coordinates[1]}");
        }

        // Поиск ближайшего соседа
        Point nearestNeighbor = rTree.FindNearestNeighbor(new Point(3, 3));
        Console.WriteLine($"Ближайший сосед к точке 3, 3: {nearestNeighbor.Coordinates[0]}, {nearestNeighbor.Coordinates[1]}");
    }

    static void PrintTree(Node node, int level)
    {
        if (node == null)
            return;

        for (int i = 0; i < level; i++)
            Console.Write("  ");

        if (node.Children.Count == 0)
        {
            Console.WriteLine($"Точка: ({node.Mbr.MinPoint.Coordinates[0]}, {node.Mbr.MinPoint.Coordinates[1]})");
        }
        else
        {
            Console.WriteLine("Узел:");
            foreach (var child in node.Children)
            {
                PrintTree(child, level + 1);
            }
        }
    }
}
