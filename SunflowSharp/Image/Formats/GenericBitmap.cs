// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System.IO;
using System;

using SunflowSharp.Image;
using SunflowSharp.Systems;

namespace SunflowSharp.Image.Formats
{
	public class GenericBitmap : Bitmap {
		private int w, h;
		private Color[] color;
		private float[] alpha;
		
		public GenericBitmap(int w, int h) {
			this.w = w;
			this.h = h;
			color = new Color[w * h];
			alpha = new float[w * h];
		}
		
		public override int getWidth() {
			return w;
		}
		
		public override int getHeight() {
			return h;
		}
		
		public override Color readColor(int x, int y) {
			return color[x + y * w];
		}
		
		public override float readAlpha(int x, int y) {
			return alpha[x + y * w];
		}

		public void writePixel(int x, int y, Color c, float a) {
			color[x + y * w] = c;
			alpha[x + y * w] = a;
		}
		
		public void save(string filename) {
			string extension = FileUtils.getExtension(filename);
			BitmapWriter writer = PluginRegistry.bitmapWriterPlugins.createObject(extension);
			if (writer == null) {
				UI.printError(UI.Module.IMG, "Unable to save file \"{0}\" - unknown file format: {1}", filename, extension);
				return;
			}
			try {
				writer.openFile(filename);
				writer.writeHeader(w, h, Math.Max(w, h));
				writer.writeTile(0, 0, w, h, color, alpha);
				writer.closeFile();
			} catch (IOException e) {
				UI.printError(UI.Module.IMG, "Unable to save file \"{0}\" - {1}", filename, e.Message);
			}
		}
	}
}

