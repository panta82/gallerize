using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gallerize.Models {
	public enum GalleryItemType {
		Image,
		Directory,
		Archive,
		Other
	}

	public class GalleryItem {
		static readonly HashSet<string> ImageFormats = new HashSet<string> {
			"jpg",
			"jpeg",
			"png",
			"bmp",
			"tiff",
			"gif"
		};

		static readonly HashSet<string> ArchiveFormats = new HashSet<string> {
			"zip",
			"rar",
			"7z",
			"7zip",
			"tar",
			"gz",
			"cbr",
			"cbz",
			"cb7z"
		};

		public string Path { get; set; }
		public string Name { get; set; }
		public GalleryItemType Type { get; set; }
		public bool IsValid {
			get {
				return this.Type != GalleryItemType.Other;
			}
		}

		/// <summary>
		/// Generate GalleryItem from path.
		/// </summary>
		/// <param name="path">Path to HDD where the file or directory is</param>
		/// <param name="isDirectory">If you know whether the item is file or directory, you can supply it here, to save some IO work</param>
		/// <returns></returns>
		public static GalleryItem FromPath(string path, bool? isDirectory = null) {
			var item = new GalleryItem {
				Path = path,
				Name = System.IO.Path.GetFileName(path.TrimEnd(System.IO.Path.DirectorySeparatorChar))
			};

			if (!isDirectory.HasValue) {
				var attributes = File.GetAttributes(path);
				isDirectory = attributes.HasFlag(FileAttributes.Directory);
			}
			if (isDirectory.Value) {
				item.Type = GalleryItemType.Directory;
			}
			else {
				var extension = System.IO.Path.GetExtension(path);
				if (extension.Length > 1) {
					extension = extension.Substring(1);
				}
				if (ImageFormats.Contains(extension)) {
					item.Type = GalleryItemType.Image;
				}
				else if (ArchiveFormats.Contains(extension)) {
					item.Type = GalleryItemType.Archive;
				}
				else {
					item.Type = GalleryItemType.Other;
				}
			}

			return item;
		}
	}
}
