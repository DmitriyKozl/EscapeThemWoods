namespace EscapeFromTheWoods; 

public class Boundry {
    public Boundry(short minX, short minY, short maxX, short maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    public int MinX { get; set; }
    public int MinY { get; set; }
    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public int DX { get => MaxX- MinX; }
    public int DY { get => MaxY- MinY; }
    public bool WithinBounds(int x, int y)
    {
        if ((x<MinX) || (x>MaxX) || (y<MinY) || (y>MaxY)) return false;
        return true;
    }
}