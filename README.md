# R-Tree Implementation on C#

This project is an implementation of an R-Tree, a data structure used for indexing multi-dimensional information such as geographical coordinates, rectangles, or other geometric shapes. The R-Tree structure is particularly useful for spatial access methods, allowing efficient querying of spatial data.

## Implemented Functions

The following functions have been implemented in this R-Tree project:

1. **Insert**: Adds a new rectangle to the R-Tree.
2. **Delete**: Removes a rectangle from the R-Tree.
3. **SearchExact**: Searches for a rectangle that exactly matches the specified rectangle.
4. **SearchRegion**: Searches for rectangles within a specified region.
5. **NearestNeighbor**: Finds the nearest neighbor to a specified point.

## Project Structure

The main components of the project include:
- `Rectangle`: A class representing a rectangle, which is used as the basic unit of data in the R-Tree.
- `RTreeNode`: A class representing a node in the R-Tree.
- `RTree`: A class implementing the R-Tree structure and operations.
- `Program`: The main entry point of the application, containing various test cases to demonstrate the functionality of the R-Tree.

## Classes and Methods

### Rectangle

The `Rectangle` class represents a rectangle defined by its minimum and maximum X and Y coordinates.

- **Constructor**
  ```csharp
  public Rectangle(double minX, double minY, double maxX, double maxY)
  ```
  Initializes a new instance of the `Rectangle` class with the specified coordinates.

- **Intersects**
  ```csharp
  public bool Intersects(Rectangle other)
  ```
  Checks if this rectangle intersects with another rectangle.

- **Contains**
  ```csharp
  public bool Contains(Rectangle other)
  ```
  Checks if this rectangle contains another rectangle.

- **Equals**
  ```csharp
  public override bool Equals(object obj)
  ```
  Checks if this rectangle is equal to another rectangle.

- **GetHashCode**
  ```csharp
  public override int GetHashCode()
  ```
  Returns the hash code for this rectangle.

### RTreeNode

The `RTreeNode` class represents a node in the R-Tree.

- **Constructor**
  ```csharp
  public RTreeNode(Rectangle bounds)
  ```
  Initializes a new instance of the `RTreeNode` class with the specified bounds.

- **Properties**
  - `Bounds`: The bounding rectangle of the node.
  - `Children`: A list of child nodes.
  - `IsLeaf`: A boolean indicating if the node is a leaf node (i.e., it has no children).

### RTree

The `RTree` class implements the R-Tree data structure.

- **Constructor**
  ```csharp
  public RTree(Rectangle rootBounds)
  ```
  Initializes a new instance of the `RTree` class with the specified root bounds.

- **Insert**
  ```csharp
  public void Insert(Rectangle rect)
  ```
  Inserts a new rectangle into the R-Tree.

- **Delete**
  ```csharp
  public void Delete(Rectangle rect)
  ```
  Deletes a rectangle from the R-Tree.

- **SearchExact**
  ```csharp
  public RTreeNode SearchExact(Rectangle rect)
  ```
  Searches for a rectangle that exactly matches the specified rectangle.

- **SearchRegion**
  ```csharp
  public List<RTreeNode> SearchRegion(Rectangle region)
  ```
  Searches for rectangles within the specified region.

- **NearestNeighbor**
  ```csharp
  public RTreeNode NearestNeighbor(Rectangle point)
  ```
  Finds the nearest neighbor to the specified point.

- **Distance**
  ```csharp
  private double Distance(Rectangle r1, Rectangle r2)
  ```
  Calculates the Euclidean distance between two rectangles.

### Program

The `Program` class is the main entry point of the application.

- **Main**
  ```csharp
  public static void Main()
  ```
  Contains test cases demonstrating the functionality of the R-Tree, including insertion, deletion, exact search, region search, and nearest neighbor search.

## Usage

To run the program, simply compile and execute the `Program` class. The `Main` method contains various test cases to demonstrate the functionality of the R-Tree.

You can modify the test cases in the `Main` method to further explore the capabilities of the R-Tree implementation.

## Examples

Here are some examples of operations performed by the R-Tree:

- **Insert**
  ```csharp
  rtree.Insert(new Rectangle(10, 10, 20, 20));
  ```

- **Delete**
  ```csharp
  rtree.Delete(new Rectangle(10, 10, 20, 20));
  ```

- **Search Exact**
  ```csharp
  var foundNode = rtree.SearchExact(new Rectangle(10, 10, 20, 20));
  ```

- **Search Region**
  ```csharp
  var results = rtree.SearchRegion(new Rectangle(0, 0, 25, 25));
  ```

- **Nearest Neighbor**
  ```csharp
  var nearest = rtree.NearestNeighbor(new Rectangle(15, 15, 15, 15));
  ```
