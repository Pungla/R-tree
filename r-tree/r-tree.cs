using System;
using System.Collections.Generic;

// Класс, представляющий прямоугольник (область)
public class Rectangle
{
    public double MinX, MinY, MaxX, MaxY;

    public Rectangle(double minX, double minY, double maxX, double maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    // Метод для проверки пересечения двух прямоугольников
    public bool Intersects(Rectangle other)
    {
        return !(MinX > other.MaxX || MaxX < other.MinX || MinY > other.MaxY || MaxY < other.MinY);
    }

    // Метод для проверки включения одной области в другую
    public bool Contains(Rectangle other)
    {
        return MinX <= other.MinX && MaxX >= other.MaxX && MinY <= other.MinY && MaxY >= other.MaxY;
    }

    // Метод для проверки на равенство двух прямоугольников
    public override bool Equals(object obj)
    {
        if (obj is Rectangle other)
        {
            return MinX == other.MinX && MinY == other.MinY && MaxX == other.MaxX && MaxY == other.MaxY;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MinX, MinY, MaxX, MaxY);
    }
}

// Узел R-Tree
public class RTreeNode
{
    public Rectangle Bounds;
    public List<RTreeNode> Children = new List<RTreeNode>();
    public bool IsLeaf => Children.Count == 0;

    public RTreeNode(Rectangle bounds)
    {
        Bounds = bounds;
    }
}

// Класс R-Tree
public class RTree
{
    private RTreeNode root;

    public RTree(Rectangle rootBounds)
    {
        root = new RTreeNode(rootBounds);
    }

    // Добавление объекта
    public void Insert(Rectangle rect)
    {
        Insert(root, rect);
    }

    private void Insert(RTreeNode node, Rectangle rect)
    {
        if (node.IsLeaf)
        {
            node.Children.Add(new RTreeNode(rect));
        }
        else
        {
            // Простейший алгоритм: вставка в первый дочерний элемент, который полностью содержит прямоугольник
            foreach (var child in node.Children)
            {
                if (child.Bounds.Contains(rect))
                {
                    Insert(child, rect);
                    return;
                }
            }

            // Если не нашли подходящего дочернего элемента, добавляем новый узел
            node.Children.Add(new RTreeNode(rect));
        }
    }

    // Удаление объекта
    public void Delete(Rectangle rect)
    {
        Delete(root, rect, null);
    }

    private bool Delete(RTreeNode node, Rectangle rect, RTreeNode parent)
    {
        // Проверяем сам узел
        if (node.Bounds.Equals(rect))
        {
            if (parent != null)
            {
                parent.Children.Remove(node);
                return true;
            }
            return false; // Не можем удалить корень
        }

        // Если узел листовой, проверяем его дочерние узлы
        if (node.IsLeaf)
        {
            foreach (var child in node.Children)
            {
                if (child.Bounds.Equals(rect))
                {
                    node.Children.Remove(child);
                    return true;
                }
            }
            return false;
        }
        else
        {
            // Иначе проверяем дочерние узлы рекурсивно
            foreach (var child in node.Children)
            {
                if (child.Bounds.Intersects(rect))
                {
                    if (Delete(child, rect, node))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    // Поиск по точному совпадению
    public RTreeNode SearchExact(Rectangle rect)
    {
        return SearchExact(root, rect);
    }

    private RTreeNode SearchExact(RTreeNode node, Rectangle rect)
    {
        if (node.IsLeaf)
        {
            if (node.Children.Count == 0 && node.Bounds.Equals(rect))
            {
                return node;
            }
            else
            {
                foreach (var child in node.Children)
                {
                    if (child.Bounds.Equals(rect))
                    {
                        return child;
                    }
                }
            }
            return null;
        }
        else
        {
            foreach (var child in node.Children)
            {
                if (child.Bounds.Contains(rect))
                {
                    var result = SearchExact(child, rect);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }

    // Поиск по региону (диапазонный поиск)
    public List<RTreeNode> SearchRegion(Rectangle region)
    {
        var results = new List<RTreeNode>();
        SearchRegion(root, region, results);
        return results;
    }

    private void SearchRegion(RTreeNode node, Rectangle region, List<RTreeNode> results)
    {
        if (node.Bounds.Intersects(region))
        {
            if (node.IsLeaf)
            {
                if (node.Children.Count == 0 && region.Contains(node.Bounds))
                {
                    results.Add(node);
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        if (region.Contains(child.Bounds))
                        {
                            results.Add(child);
                        }
                    }
                }
            }
            else
            {
                foreach (var child in node.Children)
                {
                    SearchRegion(child, region, results);
                }
            }
        }
    }

    // Поиск ближайшего соседа
    public RTreeNode NearestNeighbor(Rectangle point)
    {
        return NearestNeighbor(root, point, null, double.MaxValue);
    }

    private RTreeNode NearestNeighbor(RTreeNode node, Rectangle point, RTreeNode best, double bestDist)
    {
        if (node.IsLeaf)
        {
            if (node.Children.Count == 0 && Distance(node.Bounds, point) < bestDist)
            {
                best = node;
                bestDist = Distance(node.Bounds, point);
            }
            else
            {
                foreach (var child in node.Children)
                {
                    double dist = Distance(child.Bounds, point);
                    if (dist < bestDist)
                    {
                        best = child;
                        bestDist = dist;
                    }
                }
            }
        }
        else
        {
            foreach (var child in node.Children)
            {
                double dist = Distance(child.Bounds, point);
                if (dist < bestDist)
                {
                    var result = NearestNeighbor(child, point, best, bestDist);
                    double newDist = Distance(result.Bounds, point);
                    if (newDist < bestDist)
                    {
                        best = result;
                        bestDist = newDist;
                    }
                }
            }
        }
        return best;
    }

    // Расчет евклидова расстояния между двумя прямоугольниками
    private double Distance(Rectangle r1, Rectangle r2)
    {
        double dx = Math.Max(0, Math.Max(r1.MinX - r2.MaxX, r2.MinX - r1.MaxX));
        double dy = Math.Max(0, Math.Max(r1.MinY - r2.MaxY, r2.MinY - r1.MaxY));
        return Math.Sqrt(dx * dx + dy * dy);
    }

}

public class Program
{
    public static void Main()
    {
        // Создаем R-Tree с корневым узлом, охватывающим всю область
        var rtree = new RTree(new Rectangle(0, 0, 100, 100));

        // Добавляем объекты в R-Tree
        rtree.Insert(new Rectangle(10, 10, 20, 20));
        rtree.Insert(new Rectangle(30, 30, 40, 40));
        rtree.Insert(new Rectangle(50, 50, 60, 60));
        rtree.Insert(new Rectangle(70, 70, 80, 80));

        // Поиск по точному совпадению
        var foundNode = rtree.SearchExact(new Rectangle(10, 10, 20, 20));
        Console.WriteLine(foundNode != null ? "Объект (10, 10, 20, 20) найден" : "Объект (10, 10, 20, 20) не найден");

        foundNode = rtree.SearchExact(new Rectangle(25, 25, 35, 35));
        Console.WriteLine(foundNode != null ? "Объект (25, 25, 35, 35) найден" : "Объект (25, 25, 35, 35) не найден");

        // Поиск по региону
        var results = rtree.SearchRegion(new Rectangle(0, 0, 25, 25));
        Console.WriteLine($"Найдено объектов в регионе (0, 0, 25, 25): {results.Count}");

        results = rtree.SearchRegion(new Rectangle(0, 0, 100, 100));
        Console.WriteLine($"Найдено объектов в регионе (0, 0, 100, 100): {results.Count}");

        // Поиск ближайшего соседа
        var nearest = rtree.NearestNeighbor(new Rectangle(15, 15, 15, 15));
        Console.WriteLine(nearest != null ? "Ближайший сосед для (15, 15, 15, 15) найден" : "Ближайший сосед для (15, 15, 15, 15) не найден");

        nearest = rtree.NearestNeighbor(new Rectangle(65, 65, 65, 65));
        Console.WriteLine(nearest != null ? "Ближайший сосед для (65, 65, 65, 65) найден" : "Ближайший сосед для (65, 65, 65, 65) не найден");

        // Удаление объекта
        rtree.Delete(new Rectangle(10, 10, 20, 20));
        foundNode = rtree.SearchExact(new Rectangle(10, 10, 20, 20));
        Console.WriteLine(foundNode != null ? "Объект (10, 10, 20, 20) найден после удаления" : "Объект (10, 10, 20, 20) не найден после удаления");
        // Добавление и удаление нескольких объектов
        var rectsToInsert = new List<Rectangle>
        {
            new Rectangle(5, 5, 15, 15),
            new Rectangle(20, 20, 30, 30),
            new Rectangle(25, 25, 35, 35),
            new Rectangle(45, 45, 55, 55),
            new Rectangle(60, 60, 70, 70)
        };

        foreach (var rect in rectsToInsert)
        {
            rtree.Insert(rect);
        }

        // Поиск всех вновь добавленных объектов
        foreach (var rect in rectsToInsert)
        {
            foundNode = rtree.SearchExact(rect);
            Console.WriteLine(foundNode != null ? $"Объект {rect.MinX}, {rect.MinY}, {rect.MaxX}, {rect.MaxY} найден" : $"Объект {rect.MinX}, {rect.MinY}, {rect.MaxX}, {rect.MaxY} не найден");
        }

        // Удаление всех вновь добавленных объектов
        foreach (var rect in rectsToInsert)
        {
            rtree.Delete(rect);
        }

        // Проверка, что все вновь добавленные объекты удалены
        foreach (var rect in rectsToInsert)
        {
            foundNode = rtree.SearchExact(rect);
            Console.WriteLine(foundNode != null ? $"Объект {rect.MinX}, {rect.MinY}, {rect.MaxX}, {rect.MaxY} найден после удаления" : $"Объект {rect.MinX}, {rect.MinY}, {rect.MaxX}, {rect.MaxY} не найден после удаления");
        }
    }
}

