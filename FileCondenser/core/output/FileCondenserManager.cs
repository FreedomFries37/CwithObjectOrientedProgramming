using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCondenser.core.output {
	public class FileCondenserManager {
		private const string EXTENSION = ".rd";

		private readonly MultiCondenser _multiCondenser;
		private readonly List<string> dirPaths = new List<string>();

		private string fileName = "output";
		private readonly List<string> filePaths = new List<string>();
		private readonly Dictionary<string, int> filesizes = new Dictionary<string, int>();

		public FileCondenserManager(params string[] paths) : this(paths.ToList()) { }

		public FileCondenserManager(List<string> paths) {
			_multiCondenser = new MultiCondenser();

			foreach (var path in paths)
				if (Directory.Exists(path))
					AddDir(path);
				else if (File.Exists(path))
					AddFile(path);
				else
					throw new IOException();
		}

		public string OutputFileName {
			get => fileName + EXTENSION;
			set => fileName = value;
		}

		public void AddFile(string path) {
			if (!File.Exists(path)) throw new FileNotFoundException();
			filePaths.Add(path);
			_multiCondenser.Add(path);
			filesizes.Add(path, File.ReadAllText(path).Length);
		}

		public void AddDir(string path) {
			if (!Directory.Exists(path)) throw new DirectoryNotFoundException();
			dirPaths.Add(path);
		}


		private static string CreateFileHeader(string filename, int length, HuffmanChain chain) {
			return $"<fc:{filename}[{length}]{chain}>";
		}

		private static string CreateFileHeader(string filename, int length, HuffmanTree tree) {
			return $"<ft:{filename}[{length}]{tree}>";
		}

		private static string CreateFileEnd(string filename) {
			return $"</f:{filename}>";
		}

		public string GetCondensed(string path) {
			return _multiCondenser.GetCondensed(path);
		}

		public HuffmanChain GetChain(string path) {
			return _multiCondenser.GetChain(path);
		}

		public void CreateFile() {
			var fileStream = File.Create(OutputFileName);
			var streamWriter = new StreamWriter(fileStream, Encoding.Unicode);

			// TODO: implement directory stuff


			// Create file info
			foreach (var filePath in filePaths) {
				var chain = GetChain(filePath);
				var condensed = GetCondensed(filePath);
				var addition =
					$"{CreateFileHeader(filePath, filesizes[filePath], chain)}\n{condensed}{CreateFileEnd(filePath)}\n";

				if (addition.Length > filesizes[filePath]) {
					var tree = HuffmanTree.AssembleTree();
				}

				streamWriter.Write(addition);
			}

			streamWriter.Flush();
			streamWriter.Close();
		}
	}
}