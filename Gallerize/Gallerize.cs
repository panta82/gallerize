using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Gallerize.Models;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SharpCompress.Readers;

namespace Gallerize {
	public class Gallerize {
		const string TEMPLATE_NAME = "template.cshtml";
		const string ASSETS_DIRECTORY_NAME = "Assets";

		public class GallerizeException : Exception {
			public GallerizeException(string message): base(message) {
			}
		}

		public class NoSuitableFilesFoundException: GallerizeException {
			public NoSuitableFilesFoundException(): base("No suitable image files were found") {
			}
		}

		public class ExecuteResult {
			public string TempFilename { get; set; }
			public string HTML { get; set; }
			public IList<GalleryGroup> Groups { get; set; }
		}

		public ExecuteResult Execute(IList<GalleryItem> items, bool recurse) {
			var unpackedItems = this.UnpackItems(items);
			var groups = this.GenerateGroups(unpackedItems, recurse);
			if (groups.Count == 0) {
				// Nothing is found. Tell the caller we will not generate an empty HTML
				throw new NoSuitableFilesFoundException();
			}
			var html = this.GenerateHTML(groups);
			var filename = this.SaveToTempFile(html);
			this.OpenFile(filename);

			return new ExecuteResult {
				HTML = html,
				Groups = groups,
				TempFilename = filename
			};
		}

		public string GetEmbeddedTemplate() {
			var assembly = Assembly.GetCallingAssembly();
			var stream = assembly.GetManifestResourceStream($"Gallerize.{ASSETS_DIRECTORY_NAME}.{TEMPLATE_NAME}");
			var template = new StreamReader(stream).ReadToEnd();
			return template;
		}

		public IList<GalleryGroup> GenerateGroups(IList<GalleryItem> items, bool recurse) {
			var results = new List<GalleryGroup>();
			var currentGroup = new GalleryGroup {
				Name = null,
				Path = null
			};
			var firstPass = true;

			var pendingDirs = new Queue<GalleryItem>();

			while (true) {
				// Order items, so that all files are nicely sorted
				items.OrderBy(item => item.Name);

				// Separate directories from files
				foreach (var item in items) {
					if (item.Type == GalleryItemType.Directory) {
						pendingDirs.Enqueue(item);
					}
					else if (item.Type == GalleryItemType.Image) {
						currentGroup.AddItem(item);
					}
					// Ignore archives and others
				}

				// Only add groups with files we care about
				if (currentGroup.HasItems) {
					results.Add(currentGroup);
					currentGroup = new GalleryGroup();
				}

				if (pendingDirs.Count == 0) {
					// We are done
					break;
				}

				// Load next dir
				var nextDir = pendingDirs.Dequeue();
				currentGroup.Name = nextDir.Name;
				currentGroup.Path = nextDir.Path;

				items = new List<GalleryItem>();
				if (firstPass || recurse) {
					// Only dig into directories the first time or if we are recursing
					foreach (var path in Directory.GetDirectories(nextDir.Path)) {
						items.Add(GalleryItem.FromPath(path, true));
					}
				}

				foreach (var path in Directory.GetFiles(nextDir.Path)) {
					var item = GalleryItem.FromPath(path, false);
					if (item.IsValid) {
						items.Add(item);
					}
				}

				firstPass = false;
			}

			return results;
		}

		public string GenerateHTML(IList<GalleryGroup> groups) {
			// Note: this will be used only for debugging. Actual content will be read as an embedded resource.
			var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ASSETS_DIRECTORY_NAME, TEMPLATE_NAME);
			string template = this.GetEmbeddedTemplate();
			var data = new ViewData {
				Groups = groups
			};

			var config = new TemplateServiceConfiguration {
				// This will make it so that there are no staggler directories left in the TMP directory.
				// None of the tradeoffs they warn about are super relevant in this app.
				DisableTempFileLocking = true,
			};
			var service = RazorEngineService.Create(config);
			var result = service.RunCompile(new LoadedTemplateSource(template, templatePath), "index", typeof(ViewData), data);

			return result;
		}

		public string SaveToTempFile(string html) {
			var targetFileName = Path.GetTempFileName() + ".html";
			File.WriteAllText(targetFileName, html);
			return targetFileName;
		}

		public void OpenFile(string filename) {
			System.Diagnostics.Process.Start(filename);
		}

		public IList<GalleryItem> UnpackItems(IList<GalleryItem> items) {
			var resultItems = items.Select(item => {
				if (item.Type != GalleryItemType.Archive) {
					return item;
				}

				var decompressedDirectory = DecompressArchiveToTempDirectory(item.Path);
				var newItem = new GalleryItem {
					Name = item.Name,
					Path = decompressedDirectory,
					Type = GalleryItemType.Directory
				};
				return newItem;
			}).ToList();

			return resultItems;
		}

		private string DecompressArchiveToTempDirectory(string filename) {
			var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempDirectory);

			using (Stream stream = File.OpenRead(filename))
			using (var reader = ReaderFactory.Open(stream)) {
				while (reader.MoveToNextEntry()) {
					if (!reader.Entry.IsDirectory) {
						reader.WriteEntryToDirectory(tempDirectory, new ExtractionOptions() {
							ExtractFullPath = true,
							Overwrite = true
						});
					}
				}
			}

			return tempDirectory;
		}
	}
}
