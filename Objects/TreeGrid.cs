using System;
using System.Collections.Generic;

namespace EscapeFromTheWoods;

public class TreeGrid {
    public int Delta { get; set; }
    public Boundry Boundary { get; set; }
    public int NX { get; set; }
    public int NY { get; set; }
    public List<Tree>[][] Trees { get; set; }

    public TreeGrid(int delta, Boundry xyb) {
        Delta = delta;
        Boundary = xyb;
        NX = ((Boundary.DX / delta) + 1);
        NY = ((Boundary.DY / delta) + 1);
        Trees = new List<Tree>[NX][];
        for (int i = 0; i < NX; i++) {
            Trees[i] = new List<Tree>[NY];
            for (int j = 0; j < NY; j++) {
                Trees[i][j] = new List<Tree>();
            }
        }
    }

    public TreeGrid(int delta, Boundry xyb, List<Tree> data) : this(delta, xyb) {
        foreach (Tree t in data) {
            AddTree(t);
        }
    }

    public void AddTree(Tree tree) {
        if ((tree.x < Boundary.MinX) || (tree.y < Boundary.MinY) || (tree.x > Boundary.MaxX) ||
            (tree.y > Boundary.MaxY))
            throw new ArgumentOutOfRangeException("out of bounds");
        int i = ((tree.x - Boundary.MinX) / Delta);
        int j = ((tree.y - Boundary.MinY) / Delta);
        if (i == NX) i--;
        if (j == NY) j--;
        Trees[i][j].Add(tree);
    }
}