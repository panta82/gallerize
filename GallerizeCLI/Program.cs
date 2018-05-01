using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gallerize;
using Gallerize.Models;

namespace GallerizeCLI {
	class Program {
		static void Main(string[] args) {
			if (args.Length < 1) {
				Console.Error.WriteLine("You must provide at least one path to load");
				System.Environment.Exit(1);
			}

			var items = args.Select(path => GalleryItem.FromPath(path)).ToList();
			var gallerize = new Gallerize.Gallerize();
			var result = gallerize.Execute(items, true);

			Console.WriteLine(result.HTML);
			Console.WriteLine("========================================================================================");
			Console.WriteLine("Written to " + result.TempFilename);

			Console.ReadLine();
		}
	}
}
