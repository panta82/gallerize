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
				Environment.Exit(1);
				return;
			}

			var items = args.Select(path => GalleryItem.FromPath(path)).ToList();
			var gallerize = new Gallerize.Gallerize();
			Gallerize.Gallerize.ExecuteResult result = null;
			try {
				result = gallerize.Execute(items, true);
			}
			catch (Gallerize.Gallerize.GallerizeException ex) {
				Console.Error.WriteLine("ERROR: " + ex.Message);
				Environment.Exit(1);
				return;
			}
			
			Console.WriteLine(result.HTML);
			Console.WriteLine("========================================================================================");
			Console.WriteLine("Written to " + result.TempFilename);

			Console.ReadLine();
		}
	}
}
