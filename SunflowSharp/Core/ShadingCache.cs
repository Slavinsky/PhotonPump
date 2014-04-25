// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using SunflowSharp.Image;

namespace SunflowSharp.Core
{

	public class ShadingCache {

		private Sample first;
		private int depth;
		// stats
		public Int64 hits;
		public Int64 misses;
		public Int64 sumDepth;
		public Int64 numCaches;
		
		private class Sample {
			public Instance i;
			public IShader s;
			public float nx, ny, nz;
			public float dx, dy, dz;
			public Color c;
			public Sample next; // linked list
		}
		
		public ShadingCache() {
			reset();
			hits = 0;
			misses = 0;
		}
		
		public void reset() {
			sumDepth += depth;
			if (depth > 0)
				numCaches++;
			first = null;
			depth = 0;
		}
		
		public Color lookup(ShadingState state, IShader shader) {
			if (state.getNormal() == null)
				return null;
			// search further
			for (Sample s = first; s != null; s = s.next) {
				if (s.i != state.getInstance())
					continue;
				if (s.s != shader)
					continue;
				if (state.getRay().dot(s.dx, s.dy, s.dz) < 0.999f)
					continue;
				if (state.getNormal().dot(s.nx, s.ny, s.nz) < 0.99f)
					continue;
				// we have a match
				hits++;
				return s.c;
			}
			misses++;
			return null;
		}
		
		public void add(ShadingState state, IShader shader, Color c) {
			if (state.getNormal() == null)
				return;
			depth++;
			Sample s = new Sample();
			s.i = state.getInstance();
			s.s = shader;
			s.c = c;
			s.dx = state.getRay().dx;
			s.dy = state.getRay().dy;
			s.dz = state.getRay().dz;
			s.nx = state.getNormal().x;
			s.ny = state.getNormal().y;
			s.nz = state.getNormal().z;
			s.next = first;
			first = s;
		}
	}
}
