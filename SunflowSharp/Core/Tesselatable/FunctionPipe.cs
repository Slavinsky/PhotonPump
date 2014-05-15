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
using System.Collections.Generic;

using SunflowSharp.Maths;
using SunflowSharp.Core.Primitive;

namespace SunflowSharp.Core.Tesselatable
{
	/// <summary>
	/// Description of FunctionPipe.
	/// </summary>
	public class FunctionPipe : QuadMesh, ITesselatable
	{


		protected int pipeSegments = 10000;
		protected int circleSegments = 8;
		protected int knotsPerPipeSegment = 12;
		protected float pipeRadius = 0.05f;
		protected Vector3 startPosition = new Vector3(1.0f, 0.0f, 0.0f);

		protected interface IFunctionPipeFunction {
			 
			void InitParameters();
			void GetNextPosition(Vector3 p) ;
			void GetStartPosition(Vector3 p) ;
		}

		protected IFunctionPipeFunction pipeFunction;

		BoundingBox bb  = new BoundingBox();
		
		
		/* Coefficients for Matrix M */
		const double M11 	= 0.0;
		const double M12	=  1.0;
		const double M21 	= -0.5;
		const double M23	=  0.5;
		const double M31	=  1.0;
		const double M32 	= -2.5;
		const double M33	=  2.0;
		const double M34	= -0.5;
		const double M41	= -0.5;
		const double M42	=  1.5;
		const double M43	= -1.5;
		const double M44	=  0.5;		
		
		public FunctionPipe()
		{
			Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
			pipeFunction = new NullAttractor();
		}


		class NullAttractor : IFunctionPipeFunction {
		// initialize attractor parameters

			public void GetStartPosition(Vector3 p) {
			}

			public void InitParameters()
			{
			}
		
			// calculate next attractor position
			public void GetNextPosition(Vector3 p) 
			{
			}
		
		}
		
		private void GenerateMesh() {
			
			List<Vector3> curvePoints = new List<Vector3>();
			List<Vector3> attractorPoints = new List<Vector3>();
			Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
			
			pipeFunction.InitParameters();

			attractorPoints.Capacity = pipeSegments + 2;
			curvePoints.Capacity = (pipeSegments + 2) * 5;
			double thetaDelta = Math.PI * 2.0 / circleSegments;
			float tDelta = 1.0f/(knotsPerPipeSegment-1);
			
			attractorPoints.Add(startPosition);
			Vector3 temp = new Vector3();
			temp.set(startPosition);
			
			//    		Console.WriteLine(temp);
			
			for (int i =0; i<pipeSegments; i++) {
				
				pipeFunction.GetNextPosition(temp); 
				Vector3 nextPoint = new Vector3();
				nextPoint.set(temp);
				attractorPoints.Add(nextPoint);
				
			}
			
			// special case the first point
			int lastItem = attractorPoints.Count-1;
			if (attractorPoints[0] == attractorPoints[lastItem]) {
				// it loops
				attractorPoints.RemoveAt(lastItem);
				Vector3 tmp = attractorPoints[ attractorPoints.Count-1];
				attractorPoints.Add(attractorPoints[0]);
				attractorPoints.Insert(0, tmp);
			} else {
				// it does not loop
				
				Vector3 diff=  new Vector3();
				diff = Vector3.sub(attractorPoints[0], attractorPoints[1], diff);
				diff =  Vector3.add(attractorPoints[0], diff, diff);
				
				Vector3 diff2 =  new Vector3();
				diff2 = Vector3.sub(attractorPoints[lastItem], attractorPoints[lastItem-1], diff2);
				diff2 =  Vector3.add(attractorPoints[lastItem], diff2, diff2);
				
				attractorPoints.Add(diff2);
				attractorPoints.Insert(0,diff );
				
			}
			
			//  	    	Console.WriteLine("attractorPoints: {0}" , attractorPoints.Count);
			
			int knotIndex = 0;
			Vector3 splinePoint = new Vector3();
			Vector3 tangent = new Vector3();
			Vector3 normaly = new Vector3();
			Vector3 up = new Vector3(0f,1f,0f);

			int pipeIndex = 0;
			int quadIndex = 0;
			int circleIndex = 0;
			
			float t=0f;
			
			foreach ( Vector3 tempv in attractorPoints) {
				tempv.mul(4.0f);
				// 	    		Console.WriteLine("attractorPoint: {0}" , tempv);
			}
			
			Vector3 oldSplinePoint = new  Vector3();
			oldSplinePoint.set(attractorPoints[0]);
			
			for (int i=0; i<=pipeSegments-1; i++) {
				
				while (t <= 1.0f) {
					//				Console.WriteLine("t : {0}", t); 
					//				Console.WriteLine("knotIndex : {0} - {1}", knotIndex, knotIndex+3); 
					//				Console.WriteLine(attractorPoints[knotIndex+1]);
					//				Console.WriteLine(attractorPoints[knotIndex+2]);
					
					splinePoint.x = CatmullRomSpline(t,
					                                 attractorPoints[knotIndex].x,
					                                 attractorPoints[knotIndex+1].x,
					                                 attractorPoints[knotIndex+2].x,
					                                 attractorPoints[knotIndex+3].x);
					splinePoint.y = CatmullRomSpline(t, 
					                                 attractorPoints[knotIndex].y,
					                                 attractorPoints[knotIndex+1].y,
					                                 attractorPoints[knotIndex+2].y,
					                                 attractorPoints[knotIndex+3].y);
					splinePoint.z = CatmullRomSpline(t, 
					                                 attractorPoints[knotIndex].z,
					                                 attractorPoints[knotIndex+1].z,
					                                 attractorPoints[knotIndex+2].z,
					                                 attractorPoints[knotIndex+3].z);
					
					t += tDelta;
					
					tangent = Vector3.sub(splinePoint, oldSplinePoint, tangent).normalize();
					
					normaly =  Vector3.cross(tangent,up,normaly).normalize().mul(pipeRadius);
					
					oldSplinePoint.set(splinePoint);
					
					double theta = 0;
					
					Vector3 A = new Vector3();
					
					
					Matrix4 rotateAlongTangent = Matrix4.rotate(tangent.x, tangent.y, tangent.z, (float)thetaDelta);
					A.set(normaly);
					
					
					if(circleIndex == 0) 
					{
						
						for (int circleSegement	= 0; circleSegement < circleSegments ; circleSegement++) {
							
							A =  rotateAlongTangent.transformV(A);
							
							bb.include(A.x + splinePoint.x, A.y + splinePoint.y,  A.z + splinePoint.z);
							
							points[pipeIndex++] = A.x + splinePoint.x;
							points[pipeIndex++] = A.y + splinePoint.y;
							points[pipeIndex++] = A.z + splinePoint.z;
							
						}
					} 
					else
					{  	    				
						// go round it a circle.
						int circleSegement;
						for (circleSegement = 0; circleSegement < circleSegments; circleSegement++) {
							
							
							A =  rotateAlongTangent.transformV(A);
							
							//Console.WriteLine("A.tangent : {0}", Vector3.dot(A, tangent)); 
							
							//  	    						Console.WriteLine("pipeIndex : {0}", pipeIndex); 
							bb.include(A.x + splinePoint.x, A.y + splinePoint.y,  A.z + splinePoint.z);
							
							points[pipeIndex++] = A.x + splinePoint.x;
							points[pipeIndex++] = A.y + splinePoint.y;
							points[pipeIndex++] = A.z + splinePoint.z;
							
							if (circleSegement + 1 < circleSegments)
							{
								//  	    						Console.WriteLine("quadIndex : {0}", quadIndex); 
								quads[quadIndex++] = circleSegement + ((circleIndex - 1) * circleSegments)  ;
								quads[quadIndex++] = circleSegement + ((circleIndex - 1) * circleSegments) + 1;
								quads[quadIndex++] = circleSegement + (circleIndex * circleSegments) + 1;
								quads[quadIndex++] = circleSegement + (circleIndex * circleSegments);
								
							} 
							
						}
						
						// joint it back to first points
						quads[quadIndex++] = (circleSegement - 1)  + ((circleIndex - 1) * circleSegments)  ;
						quads[quadIndex++] = ((circleIndex - 1) * circleSegments);
						quads[quadIndex++] = (circleIndex * circleSegments) ;
						quads[quadIndex++] = (circleSegement - 1) + (circleIndex * circleSegments);
						
					}
					circleIndex++;
					
				}
				
				t = tDelta;
				knotIndex++;
				
			}
			
		}
		
		public PrimitiveList Tesselate() {
			return this;
		}
		
		float CatmullRomSpline(float x, float v0,float v1, float v2,float v3) {
			
			double c1,c2,c3,c4;
			
			c1 =  	      M12*v1;
			c2 = M21*v0          + M23*v2;
			c3 = M31*v0 + M32*v1 + M33*v2 + M34*v3;
			c4 = M41*v0 + M42*v1 + M43*v2 + M44*v3;
			
			return (float)(((c4*x + c3)*x +c2)*x + c1);
			
		}
		
		float CatmullRomSplineTangent(float t, float v0,float v1, float v2,float v3) {
			
			
			double tangent =  0.5 * ( 
			                         (2.0 * v1) + (-v0 + v2) + 
			                         2.0 * (2.0*v0 - 5.0*v1 + 4.0*v2 - v3) * t +
			                         3.0 * (-v0 + 3.0*v1- 3.0*v2 + v3) * t * t 
			                         );
			
			return (float)tangent;
			
		}
		
		new public bool Update(ParameterList pl, SunflowAPI api)
		{
			if (points == null) {
				points = new float[ ((circleSegments * pipeSegments  * knotsPerPipeSegment) + circleSegments) * 3];
				quads = new int[circleSegments * pipeSegments * (knotsPerPipeSegment-1) * 4];
				//  	    	Console.WriteLine("mp: {0}", (circleSegments * (pipeSegments  * (knotsPerPipeSegment-1)) + circleSegments) * 3);
				//  	    	Console.WriteLine("mq: {0}", circleSegments * pipeSegments * (knotsPerPipeSegment-1) * 4 );
			}
			GenerateMesh();
			return true;
		}
		
		public BoundingBox GetWorldBounds(Matrix4 o2w) {
			
			Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod());
			return bb;
			
		}
		
	}
	
}



