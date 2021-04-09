using System;
using System.Runtime.InteropServices;
using CoreLocation;
using MapKit;

namespace AnnotationClustering
{
	public struct FBBoundingBox {
		public nfloat x0;
		public nfloat y0;
		public nfloat xf;
		public nfloat yf;
	}

	static class CFunctions
	{
		// extern FBBoundingBox FBBoundingBoxMake (CGFloat x0, CGFloat y0, CGFloat xf, CGFloat yf);
		[DllImport ("__Internal")]
		//[Verify (PlatformInvoke)]
		static extern FBBoundingBox FBBoundingBoxMake (nfloat x0, nfloat y0, nfloat xf, nfloat yf);

		// extern FBBoundingBox FBBoundingBoxForMapRect (MKMapRect mapRect);
		[DllImport ("__Internal")]
		//[Verify (PlatformInvoke)]
		static extern FBBoundingBox FBBoundingBoxForMapRect (MKMapRect mapRect);

		// extern MKMapRect FBMapRectForBoundingBox (FBBoundingBox boundingBox);
		[DllImport ("__Internal")]
		//[Verify (PlatformInvoke)]
		static extern MKMapRect FBMapRectForBoundingBox (FBBoundingBox boundingBox);

		// extern BOOL FBBoundingBoxContainsCoordinate (FBBoundingBox box, CLLocationCoordinate2D coordinate);
		[DllImport ("__Internal")]
		//[Verify (PlatformInvoke)]
		static extern bool FBBoundingBoxContainsCoordinate (FBBoundingBox box, CLLocationCoordinate2D coordinate);

		// extern BOOL FBBoundingBoxIntersectsBoundingBox (FBBoundingBox box1, FBBoundingBox box2);
		[DllImport ("__Internal")]
		//[Verify (PlatformInvoke)]
		static extern bool FBBoundingBoxIntersectsBoundingBox (FBBoundingBox box1, FBBoundingBox box2);
	}
}