using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;

namespace EscapeFromTheWoods {
    public class Wood {
        private const int drawingFactor = 8;
        private string path;
        private DBwriter db;
        private Random _r = new Random(1);
        public int woodID { get; set; }
        public List<Tree> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;

        public Wood(int woodId, List<Tree> trees, Map map, string path, DBwriter db) {
            this.woodID = woodId;
            this.trees = trees;
            this.monkeys = new List<Monkey>();
            this.map = map;
            this.path = path;
            this.db = db;
        }

        #region ToRefactor

        public async Task EscapeAsync(Map map) {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (var m in monkeys) {
                routes.Add(await EscapeMonkeyAsync(m, map));
            }

            WriteEscaperoutesToBitmap(routes);
        }
        
        public void PlaceMonkey(string monkeyName, int monkeyID)
        {
            int treeNr;
            do
            {
                treeNr = _r.Next(0, trees.Count - 1);
            }
            while (trees[treeNr].hasMonkey);
            Monkey m = new Monkey(monkeyID, monkeyName, trees[treeNr]);
            monkeys.Add(m);
            trees[treeNr].hasMonkey = true;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{m.name} placed on x: {trees[treeNr].x}, y: {trees[treeNr].y}");
            Console.ForegroundColor = ConsoleColor.White;
        }


        private async void WriteRouteToDbAsync(Monkey monkey, List<Tree> route) {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");
            List<DBMonkeyRecord> records = new List<DBMonkeyRecord>();
            for (int j = 0; j < route.Count; j++) {
                records.Add(new DBMonkeyRecord(monkey.monkeyID, monkey.name, woodID, j, route[j].treeID, route[j].x,
                    route[j].y));
            }

            await db.WriteMonkeyRecordsAsyncDb(records);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }

        public void WriteEscaperoutesToBitmap(List<List<Tree>> routes) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");
            Color[] cvalues = new Color[]
            { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            Bitmap bm = new Bitmap((map.xmax - map.xmin) * drawingFactor, (map.ymax - map.ymin) * drawingFactor);
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);
            foreach (Tree t in trees) {
                g.DrawEllipse(p, t.x * drawingFactor, t.y * drawingFactor, drawingFactor, drawingFactor);
            }

            int colorN = 0;
            foreach (List<Tree> route in routes) {
                int p1x = route[0].x * drawingFactor + delta;
                int p1y = route[0].y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                for (int i = 1; i < route.Count; i++) {
                    g.DrawLine(pen, p1x, p1y, route[i].x * drawingFactor + delta, route[i].y * drawingFactor + delta);
                    p1x = route[i].x * drawingFactor + delta;
                    p1y = route[i].y * drawingFactor + delta;
                }

                colorN++;
            }

            string directoryPath = Path.GetDirectoryName(Path.Combine(path, woodID + "_escapeRoutes.jpeg"));
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }

            bm.Save(Path.Combine(path, woodID + "_escapeRoutes.jpg"), ImageFormat.Jpeg);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} end");
        }

        public async Task WriteWoodToDbAsync() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} start");
            List<DBWoodRecord> records = new List<DBWoodRecord>();
            foreach (Tree t in trees) {
                records.Add(new DBWoodRecord(woodID, t.treeID, t.x, t.y));
            }

            await db.WriteWoodRecordsAsync(records);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} end");
        }

        public async Task<List<Tree>> EscapeMonkeyAsync(Monkey monkey, Map map) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start {woodID},{monkey.name}");
            Dictionary<int, bool> visited = new Dictionary<int, bool>();
            trees.ForEach(x => visited.Add(x.treeID, false));
            List<Tree> route = new List<Tree>()
            { monkey.tree };
            do {
                visited[monkey.tree.treeID] = true;
                SortedList<double, List<Tree>> distanceToMonkey = new SortedList<double, List<Tree>>();

                //zoek dichtste boom die nog niet is bezocht            
                foreach (Tree t in trees) {
                    if ((!visited[t.treeID]) && (!t.hasMonkey)) {
                        double d = Math.Sqrt(Math.Pow(t.x - monkey.tree.x, 2) + Math.Pow(t.y - monkey.tree.y, 2));
                        if (distanceToMonkey.ContainsKey(d)) distanceToMonkey[d].Add(t);
                        else
                            distanceToMonkey.Add(d, new List<Tree>()
                            { t });
                    }
                }

                //distance to border            
                //noord oost zuid west
                double distanceToBorder = (new List<double>()
                { map.ymax - monkey.tree.y,
                  map.xmax - monkey.tree.x, monkey.tree.y - map.ymin, monkey.tree.x - map.xmin }).Min();
                if (distanceToMonkey.Count == 0) {
                    WriteRouteToDbAsync(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                if (distanceToBorder < distanceToMonkey.First().Key) {
                    WriteRouteToDbAsync(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();
            } while (true);
        }

        #endregion
    }
}