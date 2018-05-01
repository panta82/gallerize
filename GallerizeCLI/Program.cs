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

			var items = args.Select(path => GalleryItem.FromFilePath(path)).ToList();
			var gallerize = new Gallerize.Gallerize(items, false);
			var html = gallerize.GenerateHTML();
			Console.WriteLine(html);
			Console.ReadLine();
		}
	}
}
