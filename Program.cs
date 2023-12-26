using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EscapeFromTheWoods {
    class Program {
        static async Task Main(string[] args) {
            // TextWriter originalConsoleOut = Console.Out;
            // try {
            //     await using (StreamWriter writer =
            //                  new StreamWriter(
            //                      "D:/CursusPB/Alle_opdrachte/EscapeFromTheWoods/EscapeFromTheWoodsToRefactor/EscapeFromTheWoodsToRefactor/NET/console/consoleoutput.txt")) {
            //         Console.SetOut(writer);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // string connectionString =
            //     @"Data Source=.\SQLEXPRESS;Initial Catalog=ETW;Integrated Security=True;TrustServerCertificate=True";
            string connectionString = "mongodb://localhost:27017";
            DBwriter db = new DBwriter(connectionString);

            string path =
                @"D:\CursusPB\Alle_opdrachte\EscapeFromTheWoods\EscapeFromTheWoodsToRefactor\EscapeFromTheWoodsToRefactor\NET\monkeys";
            Map m1 = new Map(0, 500, 0, 500);
            Wood w1 = WoodBuilder.GetWood(500, m1, path, db);
            w1.PlaceMonkey("Alice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Janice", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Toby", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Mindy", IDgenerator.GetMonkeyID());
            w1.PlaceMonkey("Jos", IDgenerator.GetMonkeyID());

            Map m2 = new Map(0, 200, 0, 400);
            Wood w2 = WoodBuilder.GetWood(2500, m2, path, db);
            w2.PlaceMonkey("Tom", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jerry", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Tiffany", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Mozes", IDgenerator.GetMonkeyID());
            w2.PlaceMonkey("Jebus", IDgenerator.GetMonkeyID());

            Map m3 = new Map(0, 400, 0, 400);
            Wood w3 = WoodBuilder.GetWood(20000, m3, path, db);
            w3.PlaceMonkey("Kelly", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kenji", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kobe", IDgenerator.GetMonkeyID());
            w3.PlaceMonkey("Kendra", IDgenerator.GetMonkeyID());


            await Task.WhenAll(new[]
            { w1.WriteWoodToDbAsync(),
              w2.WriteWoodToDbAsync(),
              w3.WriteWoodToDbAsync() });

            await Task.WhenAll(new[]
            { w1.EscapeAsync(m1),
              w2.EscapeAsync(m2),
              w3.EscapeAsync(m3), });


            stopwatch.Stop();
            // Write result.
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");
        }
    }
}
//             finally {
//                 Console.SetOut(originalConsoleOut);
//             }
//
//             string content =
//                 File.ReadAllText(
//                     "D:/CursusPB/Alle_opdrachte/EscapeFromTheWoods/EscapeFromTheWoodsToRefactor/EscapeFromTheWoodsToRefactor/NET/console/consoleoutput.txt");
//             Console.WriteLine(content);
//         }
//     }
// }